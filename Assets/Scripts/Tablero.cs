using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Tablero : MonoBehaviour {

    public GameObject prefabCasilla;

    public GameObject prefabPeonBlanco;
    public GameObject prefabTorreBlanco;
    public GameObject prefabCaballoBlanco;
    public GameObject prefabAlfilBlanco;
    public GameObject prefabReinaBlanco;
    public GameObject prefabReyBlanco;

    public GameObject prefabPeonNegro;
    public GameObject prefabTorreNegro;
    public GameObject prefabCaballoNegro;
    public GameObject prefabAlfilNegro;
    public GameObject prefabReinaNegro;
    public GameObject prefabReyNegro;

    //Array bidimensional para guardar los cubos (casillas) del tablero
    private Casilla[,] casillas = new Casilla[8, 8];

    private LinkedList<Pieza> piezasBlancas = new LinkedList<Pieza>();
    private LinkedList<Pieza> piezasNegras = new LinkedList<Pieza>();
    private LinkedList<Pieza> piezasBlancasEliminadas = new LinkedList<Pieza>();
    private LinkedList<Pieza> piezasNegrasEliminadas = new LinkedList<Pieza>();

    public bool turnoBlanco = true;

    public int tamanoPiezaAncho = 15;
    public int tamanoPiezaAlto = 120;

    public Text infoBlancas;
    public Text infoNegras;
    public Text infoGeneral;

    void Start() {
        this.crearCasillas();
        this.pintarCasillas();
        crearPiezasBlancas();
        crearPiezasNegras();
    }

    // Update is called once per frame
    void Update() {
        infoBlancas.text = getInfoBlancas();
        infoNegras.text = getInfoNegras();
        infoGeneral.text = getInfoGeneral();
    }

    private string getInfoBlancas() {
        string info = "Numero de piezas blancas: ";
        string numBlancas = piezasBlancas.Count.ToString();
        info = info + numBlancas;
        return info;
    }

    private string getInfoNegras() {
        string info = "Numero de piezas negras: ";
        string numNegras = piezasNegras.Count.ToString();
        info = info + numNegras;
        return info;
    }

    private string getInfoGeneral() {
        string info = "Turno de las ";
        if (turnoBlanco) return info + " blancas";
        else return info + "negras";
    }

    public Casilla[,] getCasillas() {
        return this.casillas;
    }

    private void crearCasillas() {
        //Crear 64 casillas que serán cubos
        for (int y = 0; y < 8; y++) {
            for (int x = 0; x < 8; x++) {

                GameObject cas = Instantiate(this.prefabCasilla);
                //Crear el tag "Casilla" en el editor
                //cas.tag = "Casilla";

                //Hacer que el tablero sea padre de la casilla que se crea
                cas.transform.SetParent(this.transform);
                float tamano = 1.0f / 8f; //Tener en cuenta que son distancias relativas al padre. 1 es del tamaño del padre

                cas.transform.localScale = new Vector3(tamano, 1f, tamano);
                //Los cubos quedan en el medio del tablero central. Hay que trasladarlos, restar mitad y mitad 1/2, y quedaria centrar los subcubos. 1 / 8
                cas.transform.localPosition = new Vector3(y * tamano - 1f / 2f + (1f / 8f / 2f), 1f, x * tamano - 1f / 2f + (1f / 8f / 2f));

                //Rellenar la informacion del objeto Casilla con sus cordenadas
                cas.GetComponent<Casilla>().setx(x);
                cas.GetComponent<Casilla>().sety(y);

                //ALmacenar su color
                if (y % 2 == 0) {
                    if (x % 2 == 0) {
                        cas.GetComponent<Casilla>().colorOriginal = new Color(1, 1, 1);
                        cas.GetComponent<Casilla>().colorSelecion = new Color(1, 1, 1);
                    } else {
                        cas.GetComponent<Casilla>().colorOriginal = new Color(0, 0, 0);
                        cas.GetComponent<Casilla>().colorSelecion = new Color(0, 0, 0);
                    }
                } else {
                    if (x % 2 == 0) {
                        cas.GetComponent<Casilla>().colorOriginal = new Color(0, 0, 0);
                        cas.GetComponent<Casilla>().colorSelecion = new Color(0, 0, 0);

                    } else {
                        cas.GetComponent<Casilla>().colorOriginal = new Color(1, 1, 1);
                        cas.GetComponent<Casilla>().colorSelecion = new Color(1, 1, 1);
                    }
                }

                //Almacenar las casillas del tablero con cada casilla
                casillas[x, y] = cas.GetComponent<Casilla>();
            }
        }
    }

    public void pintarCasillasOriginales() {
        for (int y = 0; y < 8; y++) {
            for (int x = 0; x < 8; x++) {
                casillas[x, y].GetComponent<Renderer>().material.color = casillas[x, y].GetComponent<Casilla>().colorOriginal;
            }
        }
    }

    public void ponerColorCasillasOriginales() {
        for (int y = 0; y < 8; y++) {
            for (int x = 0; x < 8; x++) {
                casillas[x, y].GetComponent<Casilla>().colorSelecion = casillas[x, y].GetComponent<Casilla>().colorOriginal;
            }
        }
    }

    public void nulearColorCasillasActuales() {
        for (int y = 0; y < 8; y++) {
            for (int x = 0; x < 8; x++) {
                casillas[x, y].GetComponent<Casilla>().colorSelecion = new Color(-1, -1, -1);
            }
        }
    }

    public void pintarPosiblesMovimientos(bool[,] movs) {
        for (int y = 0; y < 8; y++) {
            for (int x = 0; x < 8; x++) {
                if (movs[x, y]) {
                    casillas[x, y].GetComponent<Casilla>().colorSelecion = new Color(1, 1, 0);
                }
            }
        }
    }

    public void pintarCasillas() {
        for (int y = 0; y < 8; y++) {
            for (int x = 0; x < 8; x++) {
                if (casillas[x, y].GetComponent<Casilla>().colorSelecion != new Color(-1, -1, -1)) {
                    casillas[x, y].GetComponent<Renderer>().material.color = casillas[x, y].GetComponent<Casilla>().colorSelecion;
                } else {
                    casillas[x, y].GetComponent<Renderer>().material.color = casillas[x, y].GetComponent<Casilla>().colorOriginal;
                }
            }
        }
    }

    /*Crea las piezas acorde con el tamaño del Tablero. Es responsivo*/
    private void crearPiezasBlancas() {
        //Obtengo la escala del tablero y se la aplico a las piezas
        Vector3 tamano = new Vector3(this.transform.lossyScale.x / tamanoPiezaAncho, this.transform.lossyScale.x / tamanoPiezaAlto, this.transform.lossyScale.x / tamanoPiezaAncho);

        GameObject go = Instantiate(prefabPeonBlanco); moverPieza(go.GetComponent<Pieza>(), 0, 6);
        go.transform.localScale = tamano; piezasBlancas.AddLast(go.GetComponent<Pieza>());
        casillas[0, 6].GetComponent<Casilla>().setPieza(go.GetComponent<Pieza>());

        GameObject go2 = Instantiate(prefabPeonBlanco); moverPieza(go2.GetComponent<Pieza>(), 1, 6);
        go2.transform.localScale = tamano; piezasBlancas.AddLast(go2.GetComponent<Pieza>());
        casillas[1, 6].GetComponent<Casilla>().setPieza(go2.GetComponent<Pieza>());

        GameObject go3 = Instantiate(prefabPeonBlanco); moverPieza(go3.GetComponent<Pieza>(), 2, 6);
        go3.transform.localScale = tamano; piezasBlancas.AddLast(go3.GetComponent<Pieza>());
        casillas[2, 6].GetComponent<Casilla>().setPieza(go3.GetComponent<Pieza>());

        GameObject go4 = Instantiate(prefabPeonBlanco); moverPieza(go4.GetComponent<Pieza>(), 3, 6);
        go4.transform.localScale = tamano; piezasBlancas.AddLast(go4.GetComponent<Pieza>());
        casillas[3, 6].GetComponent<Casilla>().setPieza(go4.GetComponent<Pieza>());

        GameObject go5 = Instantiate(prefabPeonBlanco); moverPieza(go5.GetComponent<Pieza>(), 4, 6);
        go5.transform.localScale = tamano; piezasBlancas.AddLast(go5.GetComponent<Pieza>());
        casillas[4, 6].GetComponent<Casilla>().setPieza(go5.GetComponent<Pieza>());

        GameObject go6 = Instantiate(prefabPeonBlanco); moverPieza(go6.GetComponent<Pieza>(), 5, 6);
        go6.transform.localScale = tamano; piezasBlancas.AddLast(go6.GetComponent<Pieza>());
        casillas[5, 6].GetComponent<Casilla>().setPieza(go6.GetComponent<Pieza>());

        GameObject go7 = Instantiate(prefabPeonBlanco); moverPieza(go7.GetComponent<Pieza>(), 6, 6);
        go7.transform.localScale = tamano; piezasBlancas.AddLast(go7.GetComponent<Pieza>());
        casillas[6, 6].GetComponent<Casilla>().setPieza(go7.GetComponent<Pieza>());

        GameObject go8 = Instantiate(prefabPeonBlanco); moverPieza(go8.GetComponent<Pieza>(), 7, 6);
        go8.transform.localScale = tamano; piezasBlancas.AddLast(go8.GetComponent<Pieza>());
        casillas[7, 6].GetComponent<Casilla>().setPieza(go8.GetComponent<Pieza>());

        //-------

        GameObject go9 = Instantiate(prefabTorreBlanco); moverPieza(go9.GetComponent<Pieza>(), 0, 7);
        go9.transform.localScale = tamano; piezasBlancas.AddLast(go9.GetComponent<Pieza>());
        casillas[0, 7].GetComponent<Casilla>().setPieza(go9.GetComponent<Pieza>());

        GameObject go10 = Instantiate(prefabCaballoBlanco); moverPieza(go10.GetComponent<Pieza>(), 1, 7);
        go10.transform.localScale = tamano; piezasBlancas.AddLast(go10.GetComponent<Pieza>());
        casillas[1, 7].GetComponent<Casilla>().setPieza(go10.GetComponent<Pieza>());

        GameObject go11 = Instantiate(prefabAlfilBlanco); moverPieza(go11.GetComponent<Pieza>(), 2, 7);
        go11.transform.localScale = tamano; piezasBlancas.AddLast(go11.GetComponent<Pieza>());
        casillas[2, 7].GetComponent<Casilla>().setPieza(go11.GetComponent<Pieza>());

        GameObject go12 = Instantiate(prefabReyBlanco); moverPieza(go12.GetComponent<Pieza>(), 3, 7);
        go12.transform.localScale = tamano; piezasBlancas.AddLast(go12.GetComponent<Pieza>());
        casillas[3, 7].GetComponent<Casilla>().setPieza(go12.GetComponent<Pieza>());

        GameObject go13 = Instantiate(prefabReinaBlanco); moverPieza(go13.GetComponent<Pieza>(), 4, 7);
        go13.transform.localScale = tamano; piezasBlancas.AddLast(go13.GetComponent<Pieza>());
        casillas[4, 7].GetComponent<Casilla>().setPieza(go13.GetComponent<Pieza>());

        GameObject go14 = Instantiate(prefabAlfilBlanco); moverPieza(go14.GetComponent<Pieza>(), 5, 7);
        go14.transform.localScale = tamano; piezasBlancas.AddLast(go14.GetComponent<Pieza>());
        casillas[5, 7].GetComponent<Casilla>().setPieza(go14.GetComponent<Pieza>());

        GameObject go15 = Instantiate(prefabCaballoBlanco); moverPieza(go15.GetComponent<Pieza>(), 6, 7);
        go15.transform.localScale = tamano; piezasBlancas.AddLast(go15.GetComponent<Pieza>());
        casillas[6, 7].GetComponent<Casilla>().setPieza(go15.GetComponent<Pieza>());

        GameObject go16 = Instantiate(prefabTorreBlanco); moverPieza(go16.GetComponent<Pieza>(), 7, 7);
        go16.transform.localScale = tamano; piezasBlancas.AddLast(go16.GetComponent<Pieza>());
        casillas[7, 7].GetComponent<Casilla>().setPieza(go16.GetComponent<Pieza>());

    }

    /*Crea las piezas acorde con el tamaño del Tablero. Es responsivo*/
    private void crearPiezasNegras() {
        //Obtengo la escala del tablero y se la aplico a las piezas
        Vector3 tamano = new Vector3(this.transform.lossyScale.x / tamanoPiezaAncho, this.transform.lossyScale.x / tamanoPiezaAlto, this.transform.lossyScale.x / tamanoPiezaAncho);

        GameObject go = Instantiate(prefabPeonNegro);
        moverPieza(go.GetComponent<Pieza>(), 0, 1);
        go.transform.localScale = tamano;
        piezasNegras.AddLast(go.GetComponent<Pieza>());
        casillas[0, 1].GetComponent<Casilla>().setPieza(go.GetComponent<Pieza>());

        GameObject go2 = Instantiate(prefabPeonNegro);
        moverPieza(go2.GetComponent<Pieza>(), 1, 1);
        go2.transform.localScale = tamano;
        piezasNegras.AddLast(go2.GetComponent<Pieza>());
        casillas[1, 1].GetComponent<Casilla>().setPieza(go2.GetComponent<Pieza>());

        GameObject go3 = Instantiate(prefabPeonNegro);
        moverPieza(go3.GetComponent<Pieza>(), 2, 1);
        go3.transform.localScale = tamano;
        piezasNegras.AddLast(go3.GetComponent<Pieza>());
        casillas[2, 1].GetComponent<Casilla>().setPieza(go3.GetComponent<Pieza>());

        GameObject go4 = Instantiate(prefabPeonNegro);
        moverPieza(go4.GetComponent<Pieza>(), 3, 1);
        go4.transform.localScale = tamano;
        piezasNegras.AddLast(go4.GetComponent<Pieza>());
        casillas[3, 1].GetComponent<Casilla>().setPieza(go4.GetComponent<Pieza>());

        GameObject go5 = Instantiate(prefabPeonNegro);
        moverPieza(go5.GetComponent<Pieza>(), 4, 1);
        go5.transform.localScale = tamano;
        piezasNegras.AddLast(go5.GetComponent<Pieza>());
        casillas[4, 1].GetComponent<Casilla>().setPieza(go5.GetComponent<Pieza>());

        GameObject go6 = Instantiate(prefabPeonNegro);
        moverPieza(go6.GetComponent<Pieza>(), 5, 1);
        go6.transform.localScale = tamano;
        piezasNegras.AddLast(go6.GetComponent<Pieza>());
        casillas[5, 1].GetComponent<Casilla>().setPieza(go6.GetComponent<Pieza>());

        GameObject go7 = Instantiate(prefabPeonNegro);
        moverPieza(go7.GetComponent<Pieza>(), 6, 1);
        go7.transform.localScale = tamano;
        piezasNegras.AddLast(go7.GetComponent<Pieza>());
        casillas[6, 1].GetComponent<Casilla>().setPieza(go7.GetComponent<Pieza>());

        GameObject go8 = Instantiate(prefabPeonNegro);
        moverPieza(go8.GetComponent<Pieza>(), 7, 1);
        go8.transform.localScale = tamano;
        piezasNegras.AddLast(go8.GetComponent<Pieza>());
        casillas[7, 1].GetComponent<Casilla>().setPieza(go8.GetComponent<Pieza>());

        //--------

        GameObject go9 = Instantiate(prefabTorreNegro);
        moverPieza(go9.GetComponent<Pieza>(), 0, 0);
        go9.transform.localScale = tamano;
        piezasNegras.AddLast(go9.GetComponent<Pieza>());
        casillas[0, 0].GetComponent<Casilla>().setPieza(go9.GetComponent<Pieza>());

        GameObject go10 = Instantiate(prefabCaballoNegro);
        moverPieza(go10.GetComponent<Pieza>(), 1, 0);
        go10.transform.localScale = tamano;
        piezasNegras.AddLast(go10.GetComponent<Pieza>());
        casillas[1, 0].GetComponent<Casilla>().setPieza(go10.GetComponent<Pieza>());

        GameObject go11 = Instantiate(prefabAlfilNegro);
        moverPieza(go11.GetComponent<Pieza>(), 2, 0);
        go11.transform.localScale = tamano;
        piezasNegras.AddLast(go11.GetComponent<Pieza>());
        casillas[2, 0].GetComponent<Casilla>().setPieza(go11.GetComponent<Pieza>());

        GameObject go12 = Instantiate(prefabReyNegro);
        moverPieza(go12.GetComponent<Pieza>(), 4, 0);
        go12.transform.localScale = tamano;
        piezasNegras.AddLast(go12.GetComponent<Pieza>());
        casillas[4, 0].GetComponent<Casilla>().setPieza(go12.GetComponent<Pieza>());

        GameObject go13 = Instantiate(prefabReinaNegro);
        moverPieza(go13.GetComponent<Pieza>(), 3, 0);
        go13.transform.localScale = tamano;
        piezasNegras.AddLast(go13.GetComponent<Pieza>());
        casillas[3, 0].GetComponent<Casilla>().setPieza(go13.GetComponent<Pieza>());

        GameObject go14 = Instantiate(prefabAlfilNegro);
        moverPieza(go14.GetComponent<Pieza>(), 5, 0);
        go14.transform.localScale = tamano;
        piezasNegras.AddLast(go14.GetComponent<Pieza>());
        casillas[5, 0].GetComponent<Casilla>().setPieza(go14.GetComponent<Pieza>());

        GameObject go15 = Instantiate(prefabCaballoNegro);
        moverPieza(go15.GetComponent<Pieza>(), 6, 0);
        go15.transform.localScale = tamano;
        piezasNegras.AddLast(go15.GetComponent<Pieza>());
        casillas[6, 0].GetComponent<Casilla>().setPieza(go15.GetComponent<Pieza>());

        GameObject go16 = Instantiate(prefabTorreNegro);
        moverPieza(go16.GetComponent<Pieza>(), 7, 0);
        go16.transform.localScale = tamano; piezasNegras.AddLast(go16.GetComponent<Pieza>());
        casillas[7, 0].GetComponent<Casilla>().setPieza(go16.GetComponent<Pieza>());
    }

    //GameObject[] clonPiezas = piezasBlancas.ToArray();
    //int indice = 0;
    //for(int y = 6; y < 8; y++) {
    //    for(int x = 0; x < 8; x++) {

    //        clonPiezas[indice].GetComponent<Pieza>().setx(x);
    //        clonPiezas[indice].GetComponent<Pieza>().sety(y);
    //        indice++;
    //    }
    //}

    public bool getTurnoBlanco() {
        return this.turnoBlanco;
    }

    /** Coloca la pieza pasada en su posicion en el tablero
     * Tratar como si el tablero fuese en 2D
     */
    public void moverPieza(Pieza pieza, int posX, int posY) {
        //Obtener la altura del tablero
        float altura = casillas[posX, posY].transform.position.y + 0.5f;

        //Colocar la pieza visualmente
        pieza.transform.position = new Vector3(casillas[posX, posY].transform.position.x,
            altura,
            casillas[posX, posY].transform.position.z);

        //Colocar pieza en clase Casilla
        casillas[posX, posY].GetComponent<Casilla>().setPieza(pieza);

        //Quitar pieza de la casilla anterior
        int posxAnterior = pieza.posx;
        int posyAnterior = pieza.posy;
        casillas[posxAnterior, posyAnterior].setPieza(null);

        //Actualizar la posicion xy de la pieza
        pieza.GetComponent<Pieza>().setx(posX);
        pieza.GetComponent<Pieza>().sety(posY);
    }

    public void comerPiezaNegra(int posX, int posY) {
        Casilla c = casillas[posX, posY];
        Pieza p = c.getPieza();
        Debug.Log(piezasNegras.Remove(p));
        Debug.Log(piezasNegrasEliminadas.AddLast(p));
        //p.transform.position = new Vector3(0, 0, 0);
        p.gameObject.SetActive(false);
        c.setPieza(null);
    }
}
