using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class Juego : MonoBehaviour {

    #region Configuracion general del juego
    public int LIMITE_DERECHO;
    public float TOP_LIMIT;
    public int CANTIDAD_MAXIMA_GLOBOS;
    public int CANTIDAD_MAXIMA_VIDAS;
    public Text score;
    private int internalScore = 0;
    #endregion

    #region Definicion character
    public GameObject character;

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
    public Vector2 posicionInicialJugador = new Vector2(-10f, -3.2f);
    public float FUERZA_INICIAL_JUGADOR = 4f;
    public int VELOCIDAD_INICIAL_JUGADOR = 2;
    #endregion

    #region Configuracion de los enemies
    public int MAX_ENEMY_QUANTITY_IN_SCENE;
    public int CANTIDAD_ENEMIGOS_EN_NIVEL;

    int enemyCreatedQuantity = 0;
    public List<Sprite> spriteEnemigos;
    public GameObject moldeEnemigo;
    protected List<GameObject> enemies = new List<GameObject>();
    protected float timer;
    #endregion

    #endregion
    public GameObject moldeVida;
    public GameObject moldeGameOver;
    public GameObject moldeGanaste; 

    private bool terminado = false;



    // Use this for initialization
    void Start () {
        character = GameObject.Find("character-2globos");
        RespawnearJugador();
        CrearVidas();
    }

    void Update()
    {
        if(terminado) { return; }
        CheckearJugadorAhogado();
        MovimientoDelJugador(character, velocidadJugador, fuerzaMovimientoVerticalJugador);
        CreateEnemies();
        MoveEnemiesAndCheckColisions();
        CheckearJugadorGana();
    }

    private void CheckearJugadorGana()
    {
        if(enemyCreatedQuantity == CANTIDAD_ENEMIGOS_EN_NIVEL && enemies.Count == 0)
        {
            print("Ganaste!!!!");
            terminado = true;
            Instantiate(moldeGanaste).transform.position = new Vector3(0,0,0);
            velocidadJugador = 0;
            fuerzaMovimientoVerticalJugador = 0; 
        }
    }


    #region manejo de vidas y globos del character

    private void CrearVidas()
    {
        float posicionX = -LIMITE_DERECHO + 0.5f;
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
        if (character.transform.position.y < -TOP_LIMIT)
        {
            PerderVida();
        }
    }

    private void MovimientoDelJugador(GameObject jugador, int velocidad, float fuerzaMovimiento)
    {
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            ModificarAltura(jugador, 1, fuerzaMovimiento);
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
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
        ChangeImage(character, imagenJugador1Globo);
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
        ChangeImage(character, imagenJugador2Globos);
        cantidadGlobosJugador = CANTIDAD_MAXIMA_GLOBOS;
        character.transform.position =posicionInicialJugador;
        fuerzaMovimientoVerticalJugador = FUERZA_INICIAL_JUGADOR;
        velocidadJugador = VELOCIDAD_INICIAL_JUGADOR;
    }

    private void PerderJuego()
    {
        print("You loose... shame on you");
        terminado = true;
        character.transform.position = new Vector2(0, 0);
        ChangeImage(character, imagenJugadorMuerto);
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

        if (objeto.transform.position.y >= TOP_LIMIT)
        {
            objeto.transform.position = new Vector2(objeto.transform.position.x, TOP_LIMIT);
        }

    }

    private void Rebotar(GameObject jugador, float fuerza)
    {
        Rigidbody2D rb = jugador.GetComponent<Rigidbody2D>();
        rb.velocity = Vector3.zero;
        rb.AddForce(new Vector3(0.5f, 0.5f, 0) * fuerza, ForceMode2D.Impulse);
    }

    private void Avanzar(GameObject objeto, Vector3 destino, float velocidad)
    {
        Vector3 posicionDestino = objeto.transform.position + destino * Time.deltaTime * velocidad;

        if (posicionDestino.x > LIMITE_DERECHO){
            MoveGameObjectInX(objeto, -LIMITE_DERECHO);
        }
        else if(posicionDestino.x < -LIMITE_DERECHO)
        {
            MoveGameObjectInX(objeto, LIMITE_DERECHO);
        }
        else
        {
            objeto.transform.position = posicionDestino;
        }
    }

    private void MoveGameObjectInX(GameObject anObject, float newX)
    {
        anObject.transform.position = new Vector2(newX, anObject.transform.position.y);
    }
    #endregion

    void CreateEnemies()
    {
        if (enemyCreatedQuantity >= CANTIDAD_ENEMIGOS_EN_NIVEL)
        { 
            //ya spawneamos a todos
            return;
        }

        timer += Time.deltaTime;
        //Cada 3s invalido el timer sin importar que pase.
        if (timer >= 3.0f)
        {
            timer = 0f;
            var initialPosition = new Vector2(7.76f, - 3.29f);
            if(enemies.Count < MAX_ENEMY_QUANTITY_IN_SCENE)
            {            
                enemies.Add(CreateEnemy(initialPosition));
            }
            else
            {
                EnableEnemies(initialPosition);
            }
        }
    }

    private void MoveEnemiesAndCheckColisions()
    {
        List<GameObject> deadEnemies = new List<GameObject>();
        foreach (var enemy in enemies)
        {
            MoveEnemy(enemy);
            CheckCollisions(character, enemy, deadEnemies);
        }

        foreach (var enemigo in deadEnemies)
        {
            enemies.Remove(enemigo);
            Destroy(enemigo);
        }
    }


    void EnableEnemies(Vector2 initialPosition)
    {
        foreach (var enemy in enemies)
        {
            if (!enemy.activeSelf)
            {
                enemy.transform.position = initialPosition;
                enemy.SetActive(true);
            }
        }
    }

    GameObject CreateEnemy (Vector2 initialPosition)
    {
        enemyCreatedQuantity ++;
        var enemy = Instantiate(moldeEnemigo);
        enemy.transform.position = initialPosition;
        return enemy;
    }

    private void MoveEnemy(GameObject enemy)
    {
        float x = 1f;
        float y = Mathf.Sin(enemy.transform.position.x) * TOP_LIMIT;
        Avanzar(enemy, new Vector3(x, y), 0.7f);
    }

    private void ChangeImage(GameObject target, Sprite newSprite)
    {
        var currentSrpite = target.GetComponent<SpriteRenderer>();
        currentSrpite.sprite = newSprite;
    }

    public void CheckCollisions(GameObject character, GameObject enemy, List<GameObject> deadEnemies)
    {
        Collider2D[] characterColliders = character.GetComponents<Collider2D>();
        Collider2D[] enemyColliders = enemy.GetComponents<Collider2D>();
        if (characterColliders[0].bounds.Intersects(enemyColliders[1].bounds))
        {
            PerderGlobo();
            Avanzar(character, character.transform.position += new Vector3(1, 1, 0),1);
            Rebotar(character, fuerzaMovimientoVerticalJugador);
        }
        if (characterColliders[1].bounds.Intersects(enemyColliders[0].bounds))
        {
            deadEnemies.Add(enemy);
            Rebotar(character,0.5f);
            IncreaseScore();
        }
    }

    public void IncreaseScore()
    {
        internalScore += 10;
        score.text = internalScore + "";
    }
}
