using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

    // Variabili canvas
    private GameObject canvasMainScene;
    private GameObject canvasLoadScene;
    private GameObject canvasPauseMenu;
    private GameObject canvasDeathMenu;
    private GameObject canvasUI;
    private GameObject canvasEndGameMenu;
    public GameObject canvasBossMenu;

    // Variabili manager
    private GameManager gameManager;
    private DialogManager dialogManager;

    // Variabili canvasGroup
    private CanvasGroup canvasGroupMainScene;
    private CanvasGroup canvasGroupLoadScene;
    private CanvasGroup canvasGroupPauseMenu;
    private CanvasGroup canvasGroupDeathMenu;
    private CanvasGroup canvasGroupUI;
    private CanvasGroup canvasGroupEndGameMenu;
    public CanvasGroup canvasGroupBossMenu;

    // Variabili audio canvas
    private AudioSource deathSound;

    private void Awake () { }

    void Start () {
        initMenu ();
    }

    private void Update () {
      if(getScene() != "MainScene") {   
          // Controllo se il giocatore preme il tasto ESC
        triggerEscKey ();
        // Se il giocatore muove, viene attivata il menu relativo al GameOver
        toggleDeathMenu ();
        // Controlla lo stato del flag togglePauseGame
        togglePauseGame ();
      }
        
    }

    // GETTERS
    // Non sono veri getters. Li utilizzo per evitare di rendere publiche le variabili su unity
    public GameObject getCanvasUI () {
        return canvasUI;
    }
    public GameObject getCanvasMainScene () {
        return canvasMainScene;
    }
    public GameObject getCanvasPauseMenu () {
        return canvasPauseMenu;
    }
    public GameObject getCanvasDeathMenu () {
        return canvasDeathMenu;
    }

    public GameObject getCanvasEndGameMenu() {
        return canvasEndGameMenu;
    }

       public CanvasGroup getCanvasGroupPauseMenu () {
        return canvasGroupPauseMenu;
    }


    // getScene: Metodo di supporto per ottenere la scena corrente. Utile per richiamarla all'interno di altri manager
    // Questa funzione potrebbe essere dentro il GameManager ma poiche il MenuManager e' l'unico componente che e' presente in tutte le scene
    // Per evitare ridondanza ho preferito posizionarla qua dentro
    public String getScene () {
        return (SceneManager.GetActiveScene ()).name;
    }


    // initMenu: Cerca i componenti nella scena e li setta. Per la canvasUI e canvasPauseMenu ho utilizzato una canvasGroup poiche' disabilitare e riabilitare un componente
    // Attraverso l'uso del SetActive puo' generare CPU OVERHEAD. Mentre per tutte le altre canvas ho utilizzato il SetActive poiche' non c'e' possibilita' di aprire e richiudere
    // Piu' volte in un intervallo ridotto il menu. EG: il menu di morte non puo' essere aperto con la stessa frequenza del menu di pausa.
    private void initMenu () {
        switch (getScene ()) {
            case "MainScene":
                canvasLoadScene = GameObject.Find ("CanvasLoadScene");
                canvasMainScene = GameObject.Find ("CanvasMainScene");
                canvasGroupLoadScene = canvasLoadScene.GetComponent<CanvasGroup> ();
                canvasGroupMainScene = canvasMainScene.GetComponent<CanvasGroup> ();
                canvasMainScene.SetActive (true);
                canvasLoadScene.SetActive (true);
                canvasGroupMainScene.interactable=true;
                canvasGroupMainScene.blocksRaycasts=true;
                canvasGroupLoadScene.interactable=false;
                canvasGroupLoadScene.blocksRaycasts=false;
                canvasGroupMainScene.alpha = 1f;
                canvasGroupLoadScene.alpha = 0f;
                break;
            case "Level1":
                gameManager = FindObjectOfType<GameManager> ();
                canvasPauseMenu = GameObject.Find ("CanvasPauseMenu");
                canvasDeathMenu = GameObject.Find ("CanvasDeathMenu");
                canvasUI = GameObject.Find ("CanvasUI");
                canvasEndGameMenu = GameObject.Find ("CanvasEndGameMenu");
                dialogManager = FindObjectOfType<DialogManager> ();
                canvasGroupPauseMenu = canvasPauseMenu.GetComponent<CanvasGroup> ();
                canvasGroupDeathMenu = canvasDeathMenu.GetComponent<CanvasGroup> ();
                canvasGroupUI = canvasUI.GetComponent<CanvasGroup> ();
                canvasGroupEndGameMenu = canvasEndGameMenu.GetComponent<CanvasGroup> ();
                canvasPauseMenu.SetActive (true);
                canvasDeathMenu.SetActive (false);
                canvasEndGameMenu.SetActive(false);
                canvasUI.SetActive (true);
                canvasGroupPauseMenu.alpha = 0f;
                canvasGroupPauseMenu.blocksRaycasts=false;
                canvasGroupPauseMenu.interactable=false;
                canvasGroupUI.alpha = 1f;
                break;
            case "Level2":
                 gameManager = FindObjectOfType<GameManager> ();
                canvasPauseMenu = GameObject.Find ("CanvasPauseMenu");
                canvasDeathMenu = GameObject.Find ("CanvasDeathMenu");
                canvasUI = GameObject.Find ("CanvasUI");
                canvasEndGameMenu = GameObject.Find ("CanvasEndGameMenu");
                dialogManager = FindObjectOfType<DialogManager> ();
                canvasGroupPauseMenu = canvasPauseMenu.GetComponent<CanvasGroup> ();
                canvasGroupDeathMenu = canvasDeathMenu.GetComponent<CanvasGroup> ();
                canvasGroupUI = canvasUI.GetComponent<CanvasGroup> ();
                canvasGroupEndGameMenu = canvasEndGameMenu.GetComponent<CanvasGroup> ();
                canvasPauseMenu.SetActive (true);
                canvasDeathMenu.SetActive (false);
                canvasEndGameMenu.SetActive(false);
                canvasUI.SetActive (true);
                canvasGroupPauseMenu.alpha = 0f;
                 canvasGroupPauseMenu.blocksRaycasts=false;
                canvasGroupPauseMenu.interactable=false;
                canvasGroupUI.alpha = 1f;
                break;
            case "Level3":
                gameManager = FindObjectOfType<GameManager> ();
                canvasPauseMenu = GameObject.Find ("CanvasPauseMenu");
                canvasDeathMenu = GameObject.Find ("CanvasDeathMenu");
                canvasUI = GameObject.Find ("CanvasUI");
                canvasBossMenu = GameObject.Find ("CanvasBossMenu");
                dialogManager = FindObjectOfType<DialogManager> ();
                canvasGroupPauseMenu = canvasPauseMenu.GetComponent<CanvasGroup> ();
                canvasGroupDeathMenu = canvasDeathMenu.GetComponent<CanvasGroup> ();
                canvasGroupUI = canvasUI.GetComponent<CanvasGroup> ();
                canvasGroupBossMenu = canvasBossMenu.GetComponent<CanvasGroup> ();
                canvasPauseMenu.SetActive (true);
                canvasDeathMenu.SetActive (false);
                canvasBossMenu.SetActive(false);                
                canvasUI.SetActive (true);
                canvasGroupPauseMenu.alpha = 0f;
                 canvasGroupPauseMenu.blocksRaycasts=false;
                canvasGroupPauseMenu.interactable=false;
                canvasGroupUI.alpha = 1f;
                break;
             case "BossLevel":
                 gameManager = FindObjectOfType<GameManager> ();
                canvasPauseMenu = GameObject.Find ("CanvasPauseMenu");
                canvasDeathMenu = GameObject.Find ("CanvasDeathMenu");
                canvasUI = GameObject.Find ("CanvasUI");
                canvasEndGameMenu = GameObject.Find ("CanvasEndGameMenu");
                dialogManager = FindObjectOfType<DialogManager> ();
                canvasGroupPauseMenu = canvasPauseMenu.GetComponent<CanvasGroup> ();
                canvasGroupDeathMenu = canvasDeathMenu.GetComponent<CanvasGroup> ();
                canvasGroupUI = canvasUI.GetComponent<CanvasGroup> ();
                canvasGroupEndGameMenu = canvasEndGameMenu.GetComponent<CanvasGroup> ();
                canvasPauseMenu.SetActive (true);
                canvasDeathMenu.SetActive (false);
                canvasEndGameMenu.SetActive(false);
                canvasUI.SetActive (true);
                canvasGroupPauseMenu.alpha = 0f;
                 canvasGroupPauseMenu.blocksRaycasts=false;
                canvasGroupPauseMenu.interactable=false;
                canvasGroupUI.alpha = 1f;
                break;
            default:
                break;
        }
    }

 
    // mainMenu: 
    public void mainMenu () {
        gameManager.setActiveCursor();
        SceneManager.LoadScene ("MainScene");  
    }

    // loadLevel1: 
    public void loadLevel1 () {
        SceneManager.LoadScene ("Level1");
    }

    // loadLevel2: 
    public void loadLevel2 () {
        SceneManager.LoadScene ("Level2");
    }

    // loadLevel3: 
    public void loadLevel3 () {
        SceneManager.LoadScene ("Level3");
    }

        // loadLevel3: 
    public void loadBossLevel () {
        SceneManager.LoadScene ("BossLevel");
    }

    // homeMenu: 
    public void homeMenu () {
         SceneManager.LoadScene ("MainScene");
        gameManager.setActiveCursor();
                gameManager.gamePaused = false;
    }

    // backHome: 
    public void backHome () {
         canvasGroupMainScene.alpha = 1f;
         canvasGroupLoadScene.alpha = 0f;
         canvasGroupMainScene.interactable=true;
         canvasGroupMainScene.blocksRaycasts=true;
         canvasGroupLoadScene.interactable=false;         
        canvasGroupLoadScene.blocksRaycasts=false;     
    }

    // loadGame: Abilita la schermata della scelta dei livelli e calcola l'ultimo livello sbloccato
    public void loadGame () {        
         canvasGroupMainScene.interactable=false;
         canvasGroupMainScene.blocksRaycasts=false;
         canvasGroupMainScene.alpha = 0f;
         canvasGroupLoadScene.alpha = 1f;
         canvasGroupLoadScene.interactable=true;
         canvasGroupLoadScene.blocksRaycasts=true;
        Color color = new Color32(255, 255, 255, 100);

        // Setto gli score 
        canvasLoadScene.transform.GetChild(1).transform.GetChild(5).GetChild(0).GetComponent<Text>().text = GameData.getLevelScore("Level1").ToString()+" / 25"; 
        canvasLoadScene.transform.GetChild(1).transform.GetChild(6).GetChild(0).GetComponent<Text>().text = GameData.getLevelScore("Level2").ToString()+" / 19"; 
        canvasLoadScene.transform.GetChild(1).transform.GetChild(7).GetChild(0).GetComponent<Text>().text = GameData.getLevelScore("Level3").ToString()+" / 7"; 
      
        // Sblocco o blocco i livelli in base allo stato di avanzamento del gioco
          if(!GameData.isLevelUnlocked("Level2")) {
              canvasLoadScene.transform.GetChild(1).transform.GetChild(1).GetComponent<Image>().color = color; // Background level1
               canvasLoadScene.transform.GetChild(1).transform.GetChild(1).GetComponent<Button>().interactable=false;
            }
          if(!GameData.isLevelUnlocked("Level3")) {          
              canvasLoadScene.transform.GetChild(1).transform.GetChild(2).GetComponent<Image>().color = color; // Background level1
              canvasLoadScene.transform.GetChild(1).transform.GetChild(2).GetComponent<Button>().interactable=false;
            }
            if(!GameData.isLevelUnlocked("BossLevel")) {          
              canvasLoadScene.transform.GetChild(1).transform.GetChild(3).GetComponent<Image>().color = color; // Background level1
              canvasLoadScene.transform.GetChild(1).transform.GetChild(3).GetComponent<Button>().interactable=false;
            }         
    }


    // backGame: 
    public void backGame () {
        gameManager.setDisableCursor();
        canvasGroupPauseMenu.alpha = 0f;
        canvasGroupUI.alpha = 1f;
        gameManager.gamePaused = false;
    }

    // restartGame: Ottiene il nome della scena attuale e la ricarica
    public void restartGame () {
        gameManager.setDisableCursor();
        SceneManager.LoadScene (getScene());
    }

    // respawPlayer: 
    public void respawPlayer () {
        gameManager.respawPlayer ();
    }

    // quitGame: 
    public void quitGame () {
        Application.Quit ();
    }

    // MENU TRIGGERS //

    // triggerEscKey: appena viene premuto il tasto ESC, avvia la canvas del menu di pausa
    public void triggerEscKey () {
        if (Input.GetKeyDown (KeyCode.Escape) && (dialogManager.getDialogActive () == 0f) && (!canvasDeathMenu.activeSelf)) //(!isGameOver) Se il tasto e' stato premuto e deathCanvas o dialogCanvas non sono attive, entro
        {
            if (canvasGroupUI.alpha == 1f) // Se e' attiva la UI quindi non esistono altri menu attivi
            {
                gameManager.setActiveCursor();
                canvasGroupUI.alpha = 0f;
                canvasGroupPauseMenu.alpha = 1f;
                canvasGroupPauseMenu.blocksRaycasts=true;
                canvasGroupPauseMenu.interactable=true;
                gameManager.gamePaused = true;
            } else {
                canvasGroupUI.alpha = 1f;
                 canvasGroupPauseMenu.alpha = 0f;
                 canvasGroupPauseMenu.blocksRaycasts=false;
                canvasGroupPauseMenu.interactable=false;
                gameManager.setDisableCursor();
                gameManager.gamePaused = false;
            }
        }
    }

    // toggleDeathMenu: controlla se la vita o il timer sono pari a 0. Nel caso lo siano
    // pausa il gioco per non far muovere il player, attivo la canvas del game over (deathCanvas)
    // disattiva backgroundSound, getTimeOutSound(), attivo deathSound 
    public void toggleDeathMenu () {
        
        if (gameManager.healthCounter == 0f || gameManager.stageTimer <= 0f) {
            gameManager.setActiveCursor();
            gameManager.gamePaused = true;
            canvasDeathMenu.SetActive(true);
            gameManager.getBackgroundGameSound().Stop ();
            gameManager.getTimeOutSound().Stop ();
        }
    }

    // togglePauseGame: Quando chiamata cambia lo stato pausa del gioco
    public void togglePauseGame () {
        
        if (gameManager.gamePaused == true) {
            gameManager.setActiveCursor();
            Time.timeScale = 0f;
        } else {
            Time.timeScale = 1f;
        }
    }

}