using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Juego : MonoBehaviour {

    #region Configuracion general del juego
    public int LIMITE_DERECHO = 10;
    public float LIMITE_SUPERIOR = 4.40f;
    public int CANTIDAD_MAXIMA_GLOBOS = 2;
    public int CANTIDAD_MAXIMA_VIDAS = 2;
    #endregion

    #region Definicion jugador
    public GameObject jugador;

    #region posibles imagenes
    public Sprite imagenJugador1Globo;
    public Sprite imagenJugador2Globos;
    public Sprite imagenJugadorMuerto;
    #endregion posibles imagenes

    #region estado
    public int velocidadJugador;
    public float fuerzaMovimientoVerticalJugador;
    public int cantidadGlobosJugador;
    public List<GameObject> vidasJugador = new List<GameObject>();
    #endregion

    #region configuracion
    public Vector2 posicionInicialJugador = new Vector2(-10f,-3.2f);
    public float FUERZA_INICIAL_JUGADOR = 4f;
    public int VELOCIDAD_INICIAL_JUGADOR = 2;
    #endregion

    #endregion

    #region Configuracion de los enemigos
    public GameObject Enemigo;
    public List<Sprite> spriteEnemigos;
    
    #endregion

    public GameObject moldeVida;
    public GameObject moldeGameOver;



    // Use this for initialization
    void Start () {
        jugador = GameObject.Find("jugador-2globos");

        RespawnearJugador();
        CrearVidas();
    }

    void Update()
    {
        CheckearJugadorAhogado();
        MovimientoDelJugador(jugador, velocidadJugador, fuerzaMovimientoVerticalJugador);
        MoverEnemigo();
    }

    #region manejo de vidas y globos del jugador

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

    private void CheckearJugadorAhogado()
    {
        if (jugador.transform.position.y < -LIMITE_SUPERIOR)
        {
            PerderVida();
        }
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
            Avanzar(jugador, jugador.transform.right * - 1, velocidad);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            Avanzar(jugador, jugador.transform.right, velocidad);
        }
    }

    protected void PerderGlobo()
    {
        ExplotarGlobo();
        if (cantidadGlobosJugador <= 0) {
            PerderVida();
        }
    }

    private void ExplotarGlobo()
    {
        cantidadGlobosJugador -= 1;
        CambiarImagen(jugador, imagenJugador1Globo);
        //como tenemos menos globos nos movemos mas lento
        fuerzaMovimientoVerticalJugador -= 2f;
        velocidadJugador += 2;
    }
    
    private void PerderVida()
    {
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
        CambiarImagen(jugador, imagenJugador2Globos);
        cantidadGlobosJugador = CANTIDAD_MAXIMA_GLOBOS;
        jugador.transform.position =posicionInicialJugador;
        fuerzaMovimientoVerticalJugador = FUERZA_INICIAL_JUGADOR;
        velocidadJugador = VELOCIDAD_INICIAL_JUGADOR;
    }

    private void PerderJuego()
    {
        print("You loose... shame on you");
        jugador.transform.position = new Vector2(0, 0);
        CambiarImagen(jugador, imagenJugadorMuerto);
        fuerzaMovimientoVerticalJugador = 0f;
        velocidadJugador = 0;
        GameObject resultado = Instantiate(moldeGameOver);
        resultado.transform.position = new Vector2(0,0);
    }
    #endregion

    #region manejar el movimiento de los elementos en la escena

    private void ModificarAltura(GameObject objeto, int direccion, float fuerzaDeMovimiento)
    {
        Rigidbody2D rb = objeto.GetComponent<Rigidbody2D>();
        //esto es para que no se acumule la fuerza
        rb.velocity = Vector3.zero;
        rb.AddForce(Vector3.up * direccion* fuerzaDeMovimiento, ForceMode2D.Impulse);
    }

    private void Avanzar(GameObject objeto, Vector3 destino, float velocidad)
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
            objeto.transform.position += destino * Time.deltaTime * velocidad ;
        }
    }

    private void MoverObjetoEnX(GameObject objeto, float nuevoX)
    {
        objeto.transform.position = new Vector2(nuevoX, objeto.transform.position.y);
    }
    #endregion


    void MoverEnemigo()
    {   
        float x = 1f;
        float sentidoY = 1;
        float y = (Mathf.Sin(x * 0.5f) * LIMITE_SUPERIOR) * sentidoY;

        if (Enemigo.transform.position.y >= LIMITE_SUPERIOR)
        {
            sentidoY = -1;
        }

        print(y);
        print(Enemigo.transform.position);
        Avanzar(Enemigo, new Vector3(x, y), 0.5f);

        //if (Enemigo.transform.position.x > LIMITE_DERECHO)
        //{
        //    MoverObjetoEnX(Enemigo, -LIMITE_DERECHO);
        //}
        //else if (Enemigo.transform.position.x < -LIMITE_DERECHO)
        //{
        //    MoverObjetoEnX(Enemigo, LIMITE_DERECHO);
        //}
        //else
        //{
        //    Enemigo.transform.position += new Vector3(x, y);
        //}


        //print(Enemigo.transform.position);
    }

    private void CambiarImagen(GameObject target, Sprite nuevaImagen)
    {
        var spriteActual = target.GetComponent<SpriteRenderer>();
        spriteActual.sprite = nuevaImagen;
    }
}
