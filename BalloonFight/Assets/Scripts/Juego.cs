using UnityEngine;
using System.Collections;
using System;

public class Juego : MonoBehaviour {
    public GameObject jugador;
    public int velocidadJugador;
    public float fuerzaMovimientoVerticalJugador;
    public Vector2 posicionInicialJugador = new Vector2(-10f,-2.8f);
    public int cantidadGlobosJugador;
    public int cantidadVidasJugador;

    public int limiteDerecho = 14;
    public int CANTIDAD_MAXIMA_GLOBOS = 2;



    // Use this for initialization
    void Start () {
        cantidadVidasJugador = 2;
        cantidadGlobosJugador = CANTIDAD_MAXIMA_GLOBOS;

        jugador = GameObject.Find("jugador-2globos");
        velocidadJugador = 2;
        fuerzaMovimientoVerticalJugador = 4f;
        jugador.transform.position = posicionInicialJugador;
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


    private void PerderGlobo()
    {
        print("Empiezo con Globos: " + cantidadGlobosJugador + " vidas: " + cantidadVidasJugador);

        cantidadGlobosJugador -= 1;
        if (cantidadGlobosJugador <= 0) {
            cantidadVidasJugador -= 1;

            if (cantidadVidasJugador <= 0)
            {
                PerderJuego();
            }
            else
            {
                RespawnearJugador();
            }
        }

        print("Termino con Globos: " + cantidadGlobosJugador + " vidas: " + cantidadVidasJugador) ;
    }

    private void RespawnearJugador()
    {
        cantidadGlobosJugador = CANTIDAD_MAXIMA_GLOBOS;
        jugador.transform.position =posicionInicialJugador;
    }

    private void PerderJuego()
    {
        print("You loose... shame on you");
    }


    //Auxiliares para manejar el movimiento de los elementos en la escena
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




        PerderGlobo();
    }

    //----- FIN: Auxiliares para manejar el movimiento de los elementos en la escena -----
}
