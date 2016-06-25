using UnityEngine;
using System.Collections;
using System;

public class Juego : MonoBehaviour {

    GameObject jugador;
    int velocidadJugador;
    float fuerzaMovimientoVerticalJugador;

    int limiteDerecho = 14;



    // Use this for initialization
    void Start () {

        jugador = GameObject.Find("jugador-2globos");
        velocidadJugador = 3;
        fuerzaMovimientoVerticalJugador = 4f;



    }

    void Update()
    {
        MovimientoDelJugador(jugador, velocidadJugador, fuerzaMovimientoVerticalJugador);
    }

    private void MovimientoDelJugador(GameObject jugador, int velocidad, float fuerzaMovimiento)
    {

        if (Input.GetKey(KeyCode.UpArrow))
        {
            ModificarAltura(jugador, 1, fuerzaMovimiento);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            ModificarAltura(jugador, -1, fuerzaMovimiento);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Avanzar(jugador, -1, velocidad);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            Avanzar(jugador,1, velocidad);
        }
    }

    private void ModificarAltura(GameObject objeto, int direccion, float fuerzaDeMovimiento)
    {
        Rigidbody2D rb = objeto.GetComponent<Rigidbody2D>();
        //esto es para que no se acumule la fuerza
        rb.velocity = Vector3.zero;
        rb.AddForce(Vector3.up * direccion* fuerzaDeMovimiento, ForceMode2D.Impulse);
    }

    private void Avanzar(GameObject objeto, int direccion, int velocidad)
    {
        if(objeto.transform.position.x > limiteDerecho){
            MoverObjetoEnX(objeto, -limiteDerecho);
        }
        else if(objeto.transform.position.x < -limiteDerecho)
        {
            MoverObjetoEnX(objeto, limiteDerecho);
        }
        else
        {
            objeto.transform.position += objeto.transform.right * Time.deltaTime * velocidad * direccion;
        }
    }

    private void MoverObjetoEnX(GameObject objeto, int nuevoX)
    {
        objeto.transform.position = new Vector2(nuevoX, objeto.transform.position.y);
    }
}
