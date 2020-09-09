using UnityEngine;
using System.Collections;

public class ControladorJugador : MonoBehaviour {

    public int gunDamage = 1;                                           // Set the number of hitpoints that this gun will take away from shot objects with a health script
    public float fireRate = 0.25f;                                      // Number in seconds which controls how often the player can fire
    public float weaponRange = 100f;                                     // Distance in Unity units over which the player can fire
    public float hitForce = 100f;                                       // Amount of force which will be added to objects with a rigidbody shot by the player
    public Transform gunEnd;                                            // Holds a reference to the gun end object, marking the muzzle location of the gun

    private Camera fpsCam;                                              // Holds a reference to the first person camera
    private WaitForSeconds shotDuration = new WaitForSeconds(0.07f);    // WaitForSeconds object used by our ShotEffect coroutine, determines time laser line will remain visible
    private AudioSource gunAudio;                                       // Reference to the audio source which will play our shooting sound effect
    private LineRenderer laserLine;                                     // Reference to the LineRenderer component which will display our laserline
    private float nextFire;                                             // Float to store the time the player will be allowed to fire again, after firing

    private LineRenderer laser;

    public Tablero tablero;

    private Casilla casillaOrigen;
    private bool casillaEstaSelecionada = false;
    private Casilla casillaDestino;
    private bool[,] movsPosibles;

    void Start() {
        Debug.Log("hola");
        // Get and store a reference to our LineRenderer component
        laserLine = GetComponent<LineRenderer>();

        // Get and store a reference to our AudioSource component
        gunAudio = GetComponent<AudioSource>();

        // Get and store a reference to our Camera by searching this GameObject and its parents
        fpsCam = GetComponentInParent<Camera>();

        laser = GetComponent<LineRenderer>();
    }


    void Update() {
        pintarCasillaApuntada();
        pintarLaserCasillaSelecionada();

        if (Input.GetButtonDown("Fire2") && tablero.getTurnoBlanco()) {
            volverElegir();
        }

        // Check if the player has pressed the fire button and if enough time has elapsed since they last fired
        if (Input.GetButtonDown("Fire1") && Time.time > nextFire) {

            if (tablero.getTurnoBlanco() == true) {
                moverPiezaTurnoBlanco();
            } else {
                moverPiezaTurnoNegro();
            }




            // Update the time when our player can fire next
            nextFire = Time.time + fireRate;

            // Start our ShotEffect coroutine to turn our laser line on and off
            StartCoroutine(ShotEffect());

            // Create a vector at the center of our camera's viewport
            Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));

            // Declare a raycast hit to store information about what our raycast has hit
            RaycastHit hit;

            // Set the start position for our visual effect for our laser to the position of gunEnd
            laserLine.SetPosition(0, gunEnd.position);

            // Check if our raycast has hit anything
            if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, weaponRange)) {
                // Set the end position for our laser line 
                laserLine.SetPosition(1, hit.point);

                // Get a reference to a health script attached to the collider we hit
                ShootableBox health = hit.collider.GetComponent<ShootableBox>();

                // If there was a health script attached
                if (health != null) {
                    // Call the damage function of that script, passing in our gunDamage variable
                    health.Damage(gunDamage);
                }

                // Check if the object we hit has a rigidbody attached
                if (hit.rigidbody != null) {
                    // Add force to the rigidbody we hit, in the direction from which it was hit
                    hit.rigidbody.AddForce(-hit.normal * hitForce);
                }
            } else {
                // If we did not hit anything, set the end of the line to a position directly in front of the camera at the distance of weaponRange
                laserLine.SetPosition(1, rayOrigin + (fpsCam.transform.forward * weaponRange));
            }
        }
    }

    private void moverPiezaTurnoBlanco() {
        //Se coge pieza, primer disparo
        if (casillaEstaSelecionada == false) {
            //Se mira si hay pieza en la casilla y si esta es BLANCA, si es así se seleciona
            if (hayPieza() != null && hayPieza().GetComponent<Pieza>().getEsBlanca()) {
                casillaOrigen = selecionarCasilla();
                casillaOrigen.casillaSelecionada = true;
                casillaOrigen.colorSelecion = new Color(0, 1, 0);
                casillaEstaSelecionada = true;
                Pieza p = hayPieza().GetComponent<Pieza>();
                tablero.pintarPosiblesMovimientos(p.posiblesMovimientos(this.tablero));
                this.movsPosibles = p.posiblesMovimientos(this.tablero);
            }

            //Se coloca pieza, segundo disparo
        } else {
            //Se coloca la pieza si el tablero tiene algun 'true' en los posibles movimientos
            casillaDestino = selecionarCasilla();
            if (casillaDestino != null) { //Se tiene que disparar a una casilla, no al cielo ni así...
                //if (this.movsPosibles[casillaDestino.getx(), casillaDestino.gety()]) {
                Pieza pieza = casillaOrigen.getPieza();
                Debug.Log(pieza);
                Debug.Log(casillaDestino.getPieza().esBlanca);
                //Si en el movimiento posible hay una pieza negra, se come
                if (casillaDestino.getPieza().esBlanca == false) {
                    Debug.Log("Entra aqui pa matar");
                    tablero.comerPiezaNegra(casillaDestino.getx(), casillaDestino.gety());
                }

                //Colocar la pieza selecionada en su nuevo destino
                tablero.moverPieza(pieza, casillaDestino.getx(), casillaDestino.gety());
                casillaEstaSelecionada = false;

                tablero.turnoBlanco = false;


                //pintar la casilla como estaba originalmente
                casillaOrigen.casillaSelecionada = false;
                tablero.ponerColorCasillasOriginales();
                //}
            }
        }
    }

    /** Por si se quiere deselecionar pieza y coger otra*/
    private void volverElegir() {
        tablero.ponerColorCasillasOriginales();
        casillaEstaSelecionada = false;
        casillaOrigen.casillaSelecionada = false;
    }

    private void moverPiezaTurnoNegro() {
        if (casillaEstaSelecionada == false) {
            //Se mira si hay pieza en la casilla y si esta es NEGRA, si es así se seleciona
            casillaOrigen = selecionarCasilla();
            if (casillaOrigen != null) {
                if (hayPieza() != null && hayPieza().GetComponent<Pieza>().getEsBlanca() == false) {
                    casillaOrigen.casillaSelecionada = true;
                    casillaOrigen.colorSelecion = new Color(0, 1, 0);
                    casillaEstaSelecionada = true;
                }
            }
            //Se coloca pieza, segundo disparo
        } else {
            if (hayPieza() == null) {
                casillaDestino = selecionarCasilla();
                Pieza pieza = casillaOrigen.getPieza();
                Debug.Log(pieza);
                //Colocar la pieza selecionada en su nuevo destino
                tablero.moverPieza(pieza, casillaDestino.getx(), casillaDestino.gety());
                casillaEstaSelecionada = false;

                tablero.turnoBlanco = true;
                //pintar la casilla como estaba originalmente
                casillaOrigen.casillaSelecionada = false;
                casillaOrigen.colorSelecion = casillaOrigen.colorOriginal;
            }
        }
    }


    private Casilla selecionarCasilla() {
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, weaponRange)) {
            if (hit.collider.CompareTag("Casilla")) {
                Debug.Log("Casilla selecionada: " + hit.collider.GetComponent<Casilla>().getx() + " " + hit.collider.GetComponent<Casilla>().gety() + " " + hit.collider.GetComponent<Casilla>().getPieza());
                return hit.collider.GetComponent<Casilla>();
            }
        }
        return null; //por si acaso
    }

    private Pieza hayPieza() {
        Casilla c = selecionarCasilla();
        if (c == null) {
            return null;
        } else {
            return c.getPieza();
        }
    }

    private void pintarCasillaApuntada() {
        RaycastHit hit;
        tablero.pintarCasillas();

        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, weaponRange)) {
            if (hit.collider.CompareTag("Casilla")) {
                if (hit.collider.GetComponent<Casilla>().existePieza()) {
                    Pieza pieza = hit.collider.GetComponent<Casilla>().getPieza();
                    if (pieza.GetComponent<Pieza>().esBlanca) {
                        hit.collider.GetComponent<Renderer>().material.color = new Color(0.1f, 0.7f, 0.1f);
                        //Debug.Log(hit2.collider);
                        //Debug.Log(hit2.collider.GetComponent<Casilla>().getx());
                    }
                } else {
                    hit.collider.GetComponent<Renderer>().material.color = new Color(0.8f, 0, 0);
                }
            }
        }
    }

    private void pintarLaserCasillaSelecionada() {
        if (this.casillaEstaSelecionada) {
            laser.SetPosition(0, gunEnd.position);
            laser.SetPosition(1, fpsCam.transform.forward * 10000000);
            laser.startColor = Color.blue;
            laser.endColor = Color.blue;
            laser.enabled = true;
        } else {
            laser.enabled = false;
        }
    }

    private IEnumerator ShotEffect() {
        // Play the shooting sound effect
        gunAudio.Play();

        // Turn on our line renderer
        laserLine.enabled = true;

        //Wait for .07 seconds
        yield return shotDuration;

        // Deactivate our line renderer after waiting
        laserLine.enabled = false;
    }
}