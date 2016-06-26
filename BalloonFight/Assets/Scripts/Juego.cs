using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Juego : MonoBehaviour {

    public Sprite imagenJugador1Globo;
    public Sprite imagenJugador2Globos;
    public GameObject jugador;
    public int velocidadJugador;
    public float fuerzaMovimientoVerticalJugador;
    public Vector2 posicionInicialJugador = new Vector2(-10f,-2.8f);
    public int cantidadGlobosJugador;
    public List<GameObject> vidasJugador = new List<GameObject>();
    public float FUERZA_INICIAL_JUGADOR = 4f;
    public int VELOCIDAD_INICIAL_JUGADOR = 2;


    public int LIMITE_DERECHO = 14;
    public int CANTIDAD_MAXIMA_GLOBOS = 2;
    public int CANTIDAD_MAXIMA_VIDAS = 2;

    public GameObject moldeVida;



    // Use this for initialization
    void Start () {
        jugador = GameObject.Find("jugador-2globos");

        RespawnearJugador();
        CrearVidas();
    }

    private void CrearVidas()
    {
        float posicionX = -12.5f;
        float posicionY = 4.5f;
        float separacionX = 0.5f;
        for (int i = 0; i < CANTIDAD_MAXIMA_VIDAS; i++)
        {
            GameObject vida = Instantiate(moldeVida);
            vida.transform.position = new Vector2(posicionX + separacionX * i, posicionY);
            vidasJugador.Add(vida);
        }
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


    protected void PerderGlobo()
    {
        print("Empiezo con Globos: " + cantidadGlobosJugador + " vidas: " + vidasJugador.Count);

        ExplotarGlobo();
        if (cantidadGlobosJugador <= 0) {
            PerderVida();
        }

        print("Termino con Globos: " + cantidadGlobosJugador + " vidas: " + vidasJugador) ;
    }

    private void ExplotarGlobo()
    {
        cantidadGlobosJugador -= 1;
        var spriteActual =  jugador.GetComponent<SpriteRenderer>();
        spriteActual.sprite = imagenJugador1Globo;
        //como tenemos menos globos nos movemos mas lento
        fuerzaMovimientoVerticalJugador -= 2f;
        velocidadJugador += 2;
    }
    

    private void PerderVida()
    {
        if (vidasJugador.Count == 0)
        {
            return;
        }

        EliminarIndicadorDeVida();
        if (vidasJugador.Count == 0)
        {
            PerderJuego();
        }
        else
        {
            RespawnearJugador();
        }
    }

    private void EliminarIndicadorDeVida()
    {
        int indiceVidaPerdida = vidasJugador.Count - 1;
        Destroy(vidasJugador[indiceVidaPerdida]);
        vidasJugador.RemoveAt(indiceVidaPerdida);
    }

    private void RespawnearJugador()
    {
        var spriteActual = jugador.GetComponent<SpriteRenderer>();
        spriteActual.sprite = imagenJugador2Globos;
        cantidadGlobosJugador = CANTIDAD_MAXIMA_GLOBOS;
        jugador.transform.position =posicionInicialJugador;
        fuerzaMovimientoVerticalJugador = FUERZA_INICIAL_JUGADOR;
        velocidadJugador = VELOCIDAD_INICIAL_JUGADOR;
    }

    private void PerderJuego()
    {
        print("You loose... shame on you");
        fuerzaMovimientoVerticalJugador = 0f;
        velocidadJugador = 0;
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
        if(objeto.transform.position.x > LIMITE_DERECHO){
            MoverObjetoEnX(objeto, -LIMITE_DERECHO);
        }
        else if(objeto.transform.position.x < -LIMITE_DERECHO)
        {
            MoverObjetoEnX(objeto, LIMITE_DERECHO);
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
