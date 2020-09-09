using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peon : Pieza {

    //Se va a devolver una matriz con los 64 puntos del tablero indicando
    //cuales son validos. Al declarar el array[,] este rellena los huecos con false, asique
    //calcular cuales serán las casillas con true.
    public override bool[,] posiblesMovimientos(Tablero tablero) {

        bool[,] movs = new bool[8, 8];
        Casilla[,] casillas = tablero.getCasillas();

        //--- Blanca --------------------------------------------------------------------
        if (this.esBlanca) {
            //Primer movimiento, doble avance, si no hay piezas delante
            if (this.posy == 6 && casillas[this.posx, this.posy - 1].existePieza() == false) {
                movs[this.posx, this.posy - 2] = true;
            }

            //Movimiento hacia delante, comprobar que no haya otras piezas
            if (casillas[this.posx, this.posy - 1].existePieza() == false) { //Si no hay piezas delante obtener movimiento valido
                movs[this.posx, this.posy - 1] = true;
            } else {
                movs[this.posx, this.posy - 1] = false; //invalidar por ej. el doble avance
            }

            //Movimiento diagonal derecha para comer negras
            if (this.posx < 7) {
                if (casillas[this.posx + 1, this.posy - 1].existePieza()) {
                    Pieza p = casillas[this.posx + 1, this.posy - 1].getPieza();
                    if (!p.esBlanca) {
                        movs[this.posx + 1, this.posy - 1] = true;
                    }
                }
            }

            //Movimiento diagonal izquierda para comer negras
            if (this.posx > 0) {
                if (casillas[this.posx - 1, this.posy - 1].existePieza()) {
                    Pieza p = casillas[this.posx - 1, this.posy - 1].getPieza();
                    if (!p.esBlanca) {
                        movs[this.posx - 1, this.posy - 1] = true;
                    }
                }
            }
        } else {
            //--- Negra ------------------------------------------------------------------------
            //Primer movimiento, doble avance, si no hay piezas delante
            if (this.posy == 1 && casillas[this.posx, this.posy + 1].existePieza() == false) {
                movs[this.posx, this.posy + 2] = true;
            }

            //Movimiento hacia delante, comprobar que no haya otras piezas
            if (casillas[this.posx, this.posy + 1].existePieza() == false) { //Si no hay piezas delante obtener movimiento valido
                movs[this.posx, this.posy + 1] = true;
            } else {
                movs[this.posx, this.posy + 1] = false; //invalidar por ej. el doble avance
            }

            //Movimiento diagonal derecha para comer Blancas
            if (this.posx < 7) {
                if (casillas[this.posx + 1, this.posy + 1].existePieza()) {
                    Pieza p = casillas[this.posx + 1, this.posy + 1].getPieza();
                    if (p.esBlanca) {
                        movs[this.posx + 1, this.posy + 1] = true;
                    }
                }
            }

            //Movimiento diagonal izquierda para comer Blancas
            if (this.posx > 0) {
                if (casillas[this.posx - 1, this.posy + 1].existePieza()) {
                    Pieza p = casillas[this.posx - 1, this.posy + 1].getPieza();
                    if (p.esBlanca) {
                        movs[this.posx - 1, this.posy + 1] = true;
                    }
                }
            }
        }
        return movs;
    }
}
