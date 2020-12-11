using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    // Variabili bool ausiliarie per il controllo dello stato
    // public bool isGameOver;
    public bool gamePaused;

    // Variabili utilizzare per il conteggio delle gemme e vita
    public int redGemCounter;
    public int blueGemCounter;
    public int healthCounter;
    private Text redGemText;
    private Text blueGemText;
    private Text healthCounterText;
    private Text powerUpsTextCanvas;

    public Color32 colorText;

    // Variabili oggetti
    private EnergyHealth energyHealthBar;
    public float stageTimer;
    public float energyHealth;
    private int redGemMax;
    private int blueGemMax;
    public AudioSource Water;

    // Variabili audio
    private AudioSource backgroundGameSound;
    private AudioSource timeOutSound;

    // Variabili canvas
    private Text timerTextCanvas;
    private GameObject canvasUI;

    // Variabili di supporto il posizione player
    private Vector3 respawnPosition;
    private Vector3 startPosition;

    // Variabili per i managers
    private MenuManager menuManager;

    public PlayerController thePlayer;
    public float fluidLevel;

    public bool timeOutPowerUps;
    public AudioSource lavaEffect;

    // Start is called before the first frame update
    void Awake () {
        initGame ();
        timeOutPowerUps = false;
    }

    void Start () {
        GameData.unlockLevel (SceneManager.GetActiveScene ().name);
    }

    // Update is called once per frame
    void Update () {
        // Aggiorno timer

        // Aggiorno barra energia
        setEnergyHealthBar (energyHealth);
        // Attiva suono TimeOut accompagnato da cambio colore timer
        triggerTimeOutSound ();
        checkPowerUpsCanvas ();

    }

    void FixedUpdate () {
        setTimer ();
    }

    private void initGame () {
        menuManager = FindObjectOfType<MenuManager> ();
        thePlayer = FindObjectOfType<PlayerController> ();
        backgroundGameSound = GameObject.Find ("Camera").transform.GetChild (0).GetComponent<AudioSource> ();
        timeOutSound = GameObject.Find ("Camera").transform.GetChild (1).GetComponent<AudioSource> ();
        canvasUI = GameObject.Find ("CanvasUI");
        timerTextCanvas = canvasUI.transform.GetChild (0).transform.GetChild (0).GetComponent<Text> ();
        powerUpsTextCanvas = canvasUI.transform.GetChild (0).transform.GetChild (1).GetComponent<Text> ();
        redGemText = canvasUI.transform.GetChild (1).transform.GetChild (0).GetComponent<Text> ();
        blueGemText = canvasUI.transform.GetChild (1).transform.GetChild (1).GetComponent<Text> ();
        healthCounterText = canvasUI.transform.GetChild (2).transform.GetChild (0).GetComponent<Text> ();
        energyHealthBar = canvasUI.transform.GetChild (2).transform.GetChild (1).GetComponent<EnergyHealth> ();
        switch (menuManager.getScene ()) {
            case "Level1":
                // Imposto valori di base per il livello
                setPlayer (3600 * 7, 10, 100, 25, 7);
                // Attivo musica di background
                backgroundGameSound.Play ();
                Water = GameObject.Find ("Water").GetComponent<AudioSource> ();
                fluidLevel = -1.8f;
                break;
            case "Level2":
                // Imposto valori di base per il livello
                setPlayer (3600 * 6, 10, 100, 19, 6);
                // Attivo musica di background
                backgroundGameSound.Play ();
                Water = GameObject.Find ("Water").GetComponent<AudioSource> ();
                fluidLevel = -1.2f;
                break;
            case "Level3":
                // Imposto valori di base per il livello
                setPlayer (3600 * 10, 10, 100, 7, 1);
                // Attivo musica di background
                backgroundGameSound.Play ();
                lavaEffect = GameObject.Find ("Camera").transform.GetChild (2).GetComponent<AudioSource> ();
                fluidLevel = 1f;
                break;
            case "BossLevel":
                // Imposto valori di base per il livello
                setPlayer (3600 * 6, 10, 100, 1, 1);
                // Attivo musica di background
                backgroundGameSound.Play ();
                lavaEffect = GameObject.Find ("Camera").transform.GetChild (2).GetComponent<AudioSource> ();
                fluidLevel = 4.28f;
                break;
            default:
                break;
        }
    }

    public Text getPowerUpsTextCanvas () {
        return powerUpsTextCanvas;
    }

    public AudioSource getBackgroundGameSound () {
        return backgroundGameSound;
    }

    public AudioSource getTimeOutSound () {
        return timeOutSound;
    }

    public int getRedGemMax () {
        return redGemMax;
    }
    // addGemRed: Incrementa numero gemma rossa e aggiorna il contatore nella canvas
    public void addGemRed () {
        redGemCounter = 1 + redGemCounter;
        redGemText.text = redGemCounter.ToString () + " / " + redGemMax.ToString ();
    }

    // addGemBlue: Incrementa numero gemma blu e aggiorna il contatore nella canvas
    public void addGemBlue () {
        blueGemCounter = 1 + blueGemCounter;
        blueGemText.text = blueGemCounter.ToString () + " / " + blueGemMax.ToString ();
    }

    // editHealtNumber: Aggiorna il numero delle vite
    public void editHealtNumber (int healthToEdit) {
        this.healthCounter = this.healthCounter + healthToEdit;
        if (healthCounter <= 0) {
            healthCounter = 0;
        }
        healthCounterText.text = healthCounter.ToString ();
    }

    // setTimer: Gestore del timer
    void setTimer () {
        if (!gamePaused) {

            stageTimer--;
            if (stageTimer > -1) {
                TimeSpan ts = TimeSpan.FromSeconds (stageTimer);
                timerTextCanvas.text = ts.ToString ();
            }
        }
    }

    // setEnergyHealthBar: 
    public void setEnergyHealthBar (float healthEdit) {
        energyHealthBar.editEnergyHealthImage (healthEdit);
    }

    // editEnergyHealth: 
    public void editEnergyHealth (float healthEdit) {
        energyHealth = energyHealth + healthEdit;
        if (energyHealth <= 0f) {
            energyHealth = 100f;
            editHealtNumber (-1);
            respawPlayer ();
        }
    }

    // respawPlayer: Se chiamata riporta il personaggio al punto di respawn
    public void respawPlayer () {
        thePlayer.transform.position = respawnPosition;
    }

    // triggerDead: Se attiva fa morire il player
    public void triggerDead () {
        editEnergyHealth (-100f);
    }

    // triggerTimeOutSound: Se il tempo e' pari a 10 secondi, attiva l'effetto audio. Quando il tempo e' inferiore ai 10 secondi
    // Inizia ad alternare il colore rosso e bianco 
    public void triggerTimeOutSound () {
        if (stageTimer == 605f) {
            timeOutSound.Play ();
        }
        if (stageTimer < 605f) {
            triggerCanvasText (timerTextCanvas, new Color32 (252, 92, 101, 255), new Color32 (255, 255, 255, 255));
        }
    }

    // checkPowerUpsCanvas: Controlla se qualche power ups e' attivo. Se lo e' allora alterna tra il colore dell'ultimo power up
    // attivo e la trasparenza
    private void checkPowerUpsCanvas () {
        if ((thePlayer.getInvincibilityBool () == true || thePlayer.getdoubleJumpBool () == true || thePlayer.getMoveFastBool () == true) && (timeOutPowerUps))
            triggerCanvasText (powerUpsTextCanvas, colorText, new Color32 (0, 0, 0, 0));
    }

    //triggerCanvasText: Rende visibile e invisibile la scritta ogni mezzo secondo
    public void triggerCanvasText (Text canvas, Color32 color1, Color32 color2) {
        if (stageTimer % 60 > 0 && stageTimer % 60 > 31) {
            Color color = color1; // Converto colore rgba standard nel formato unity
            canvas.color = color;
        } else {
            Color color = color2;
            canvas.color = color;
        }
    }
    // setPlayer: 
    public void setPlayer (float stageTimer, int health, float energyHealth, int blueGemMax, int redGemMax) {
        this.stageTimer = stageTimer;
        this.blueGemMax = blueGemMax;
        this.redGemMax = redGemMax;
        blueGemText.text = blueGemCounter.ToString () + " / " + blueGemMax.ToString ();
        redGemText.text = redGemCounter.ToString () + " / " + redGemMax.ToString ();
        timerTextCanvas.text = TimeSpan.FromSeconds (stageTimer).ToString ();
        editHealtNumber (health);
        this.energyHealth = energyHealth;
        startPosition = thePlayer.transform.position;
        respawnPosition = startPosition;
    }

    // setSpawn: 
    public void setSpawn (Vector3 respawnPosition) {
        this.respawnPosition = respawnPosition;
    }

    // resetSpawn: 
    public void resetSpawn () {
        this.respawnPosition = startPosition;
    }

    // FUNZIONI SUPPORTO PER DISABILITARE CURSORE   
    // setActiveCursor: Attiva il cursore
    public void setActiveCursor () {
        Cursor.visible = true;
    }

    // setDisableCursor: Disattiva il cursore
    public void setDisableCursor () {
        Cursor.visible = false;
    }
}