using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Casilla : MonoBehaviour {
    public int cordenadaX;
    public int cordenadaY;
    public Color colorOriginal;
    public Color colorSelecion;
    public bool casillaSelecionada;
    public Pieza pieza;

    public int getx() {
        return cordenadaX;
    }

    public int gety() {
        return cordenadaY;
    }

    public Pieza getPieza() {
        return this.pieza;
    }

    public void setx(int x) {
        cordenadaX = x;
    }

    public void sety(int y) {
        cordenadaY = y;
    }

    public void setPieza(Pieza pieza) {
        this.pieza = pieza;
    }


    /// <summary>
    /// Devuelve true si en esta casilla hay alguna pieza
    /// </summary>
    /// <returns></returns>
    public bool existePieza() {
        if (this.pieza != null) {
            return true;
        } else {
            return false;
        }
    }
}
