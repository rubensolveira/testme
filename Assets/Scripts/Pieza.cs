using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pieza : MonoBehaviour {
    public int posx;
    public int posy;
    public bool esBlanca;

    public void setx(int x) {
        posx = x;
    }

    public void sety(int y) {
        posy = y;
    }

    public bool getEsBlanca() {
        return esBlanca;
    }

    //Este nombre de funcion tiene un override en cada tipo de pieza. asi que este se ignora
    public virtual bool[,] posiblesMovimientos(Tablero tablero) {
        return new bool[8, 8];
    }
}
