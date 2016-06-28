using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class Juego : MonoBehaviour {

    #region Configuracion general del juego
    public int LIMITE_DERECHO = 10;
    public float LIMITE_SUPERIOR;
    public int CANTIDAD_MAXIMA_GLOBOS = 2;
    public int CANTIDAD_MAXIMA_VIDAS = 2;
    public Text score;
    private int internalScore = 0;
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
    int CANTIDAD_MAXIMA_ENEMIGOS = 1;

    public List<Sprite> spriteEnemigos;
    public GameObject moldeEnemigo;
    protected List<GameObject> enemigos = new List<GameObject>();
    protected float timer;
        
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
        CrearEnemigos();
        foreach (var enemigo in enemigos)
        {
            CheckCollisions(jugador, enemigo);
            MoverEnemigo(enemigo);
        }
    }

    #region manejo de vidas y globos del jugador

    private void CrearVidas()
    {
        float posicionX = -15.5f;
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

    void CrearEnemigos()
    {
        timer += Time.deltaTime;
        //Cada 3s invalido el timer sin importar que pase.
        if (timer >= 3.0f)
        {
            timer = 0f;
            if(enemigos.Count < CANTIDAD_MAXIMA_ENEMIGOS)
            {
                var posicionInicial = new Vector2(7.76f, - 3.29f);
                enemigos.Add(CrearEnemigo(posicionInicial));
            }
        }
    }

    GameObject CrearEnemigo (Vector2 posicionInicial)
    {
        var Enemigo = Instantiate(moldeEnemigo);
        Enemigo.transform.position = posicionInicial;
        return Enemigo;
    }

    void MoverEnemigo(GameObject UnEnemigo)
    {
        float x = 1f;
        float y = Mathf.Sin(UnEnemigo.transform.position.x) * LIMITE_SUPERIOR;
        Avanzar(UnEnemigo, new Vector3(x, y), 0.7f);
    }

    private void CambiarImagen(GameObject target, Sprite nuevaImagen)
    {
        var spriteActual = target.GetComponent<SpriteRenderer>();
        spriteActual.sprite = nuevaImagen;
    }

    public void CheckCollisions(GameObject jugador, GameObject enemigo)
    {
        Collider2D[] collidersJugador = jugador.GetComponents<Collider2D>();
        Collider2D[] collidersEnemigo = enemigo.GetComponents<Collider2D>();

        if (collidersJugador[0].bounds.Intersects(collidersEnemigo[1].bounds))
        {
            PerderGlobo();
            jugador.transform.position = new Vector2(jugador.transform.position.x + 1.0f, jugador.transform.position.y + 1.0f);
        }

        if (collidersJugador[1].bounds.Intersects(collidersEnemigo[0].bounds))
        {
            print("Mataste enemigo");
            //TODO: implementar muerte del enemigo
            sumarPuntaje();
        }
    }

    public void sumarPuntaje()
    {
        internalScore += 10;
        score.text = internalScore + "";
    }
}
