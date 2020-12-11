using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour {
    // Canvas Dialogo
    private GameObject canvasDialog;
    private CanvasGroup canvasGroupDialog;
    private AudioSource continueSound;
    private AudioSource warningSound;
    private Text dialogText;

    //Variabili di stato per il dialogo

    //public bool dialogArrayActive;
    // Array utilizzato per il caricamento dei messaggi sulla canvas di dialogo
    public String[] messages;
    // Contatore per la posizione del messaggio all'interno dell'array
    public int messageCounter;
    private GameManager gameManager;
    private MenuManager menuManager;

    public int arraySize;
    // Array dove salvo i messaggi dei tutorial + indexTips e' l'indice che utilizzo per non far ripetere lo stesso messaggio
    public bool[] tutorialTips;
    public bool endGame;

    private void Awake () {
        menuManager = FindObjectOfType<MenuManager> ();
        
    }

    // Start is called before the first frame update
    private void Start () {
        // La certezza di trovare tutti e 3 i manager  e' data dal fatto che il DialogManager è utilizzato in presenza degli altri due manager
        // Quindi non eseguo controlli di sicurezza
        gameManager = FindObjectOfType<GameManager> ();
        canvasDialog = GameObject.Find ("CanvasDialog");
        endGame = false;
        // Ottengo i vari componenti / figli attaccati alla canvasDialog
        // Componenti essenziali per l'interazione con il dialog
        canvasGroupDialog = canvasDialog.GetComponent<CanvasGroup> ();
        continueSound = canvasDialog.transform.GetChild (0).transform.GetChild (1).GetComponent<AudioSource> ();
        dialogText = canvasDialog.transform.GetChild (0).transform.GetChild (0).transform.GetChild (0).GetComponent<Text> ();

        // Inizializzo tutorial se sono nel primo livello
        initTutorial ();
        messageCounter = 0;

        initDialog (); // Ogni singolo modulo carica le sue componenti base 
        dialogPopupArray ();
    }

    // Update is called once per frame
    private void Update () {
        // Attiva il menu di dialogo in posizioni predefinite   
        toggleDialogMenu ();
        checkEndGame ();
    }

    private void checkEndGame () {
        switch(menuManager.getScene()) {
            case "Level1":
             if ((gameManager.getRedGemMax () == gameManager.redGemCounter) && !endGame) {
            endGame = true;
            dialogPopup ("Ora che hai preso tutte le gemme rosse, vai al porto per completare il livello");
        }
        break;
            case "Level2":
             if ((gameManager.getRedGemMax () == gameManager.redGemCounter) && !endGame) {
            endGame = true;
            dialogPopup ("Ora che hai preso tutte le gemme rosse, prendi il percorso alternativo a inizio livello e vai alla montagna per completare il livello");
        }
        break;
        default: break;
        }
       
    }

    public float getDialogActive () {
        return canvasGroupDialog.alpha;
    }

    private void initTutorial () {
        tutorialTips = new bool[10];
    }
    // toggleDialogMenu: Consente di interagire con il DialogMenu attraverso tastiera
    public void toggleDialogMenu () {
        if (Input.GetKeyDown (KeyCode.E) && canvasGroupDialog.alpha == 1f) {
            continueDialogButton ();
        }
    }

    // initDialog: Inizializza i messaggi dialog in base al livello caricato
    public void initDialog () {
        switch (menuManager.getScene ()) {
            case "Level1":
                messages = new String[5] {
                    "Consiglio: Puoi interagire andando avanti nei menu di dialogo schiacciando il tasto E",
                    "Per troppo tempo il villaggio di Gallinton ha dovuto sottostare alla brutale tirannia delle fiamme rosse, guidate dal temibile generale Josif Flamin, il quale ha sottratto al povero Low Polly i suoi Low pulcini.",
                    "E' ora di prendere parte alla ribellione e recuperare i propri Low pulcini. Raggiungi la citta' di Moscor e sconfiggi Flamin prima che le fiamme rosse e i loro seguaci soggioghino l’intero continente; ma ricorda: non si entra con facilita' a Moscor.",
                    "Prendi tutte le gemme rosse per poter prendere la barca al molo e raggiungere Flamin",
                    "Comandi:\n\t* Avanti (Tasto W o Freccia Su')\n\t* Sinistra (Tasto A o Freccia Sinistra)\n\t* Destra (Tasto D o Freccia Destra)\n\t* Indietro (Tasto S o Freccia Giu')\n\t* Salta (Barra spaziatrice)"
                };
                arraySize = 5;
                canvasDialog.SetActive (true);
                canvasGroupDialog.alpha = 1f;
                break;
            case "Level2":
                messages = new String[1] {
                    "Ora che sei arrivato al molo, fai attenzione a quale strada prenderai perche potresti perdere tempo prezioso!"
                };
                arraySize = 1;
                canvasDialog.SetActive (true);
                canvasGroupDialog.alpha = 1f;
                break;
            case "Level3":
                messages = new String[1] {
                    "Un ultimo sforzo! "
                };
                arraySize = 1;
                canvasDialog.SetActive (true);
                canvasGroupDialog.alpha = 1f;
                break;
             case "BossLevel":
                messages = new String[2] {
                    "Stai attento alle palle di fuoco che lancia Sauron!",
                     "Passa sopra le leve posizionate sopra le piattaforme per ammazzare il boss"
                };
                arraySize = 2;
                canvasDialog.SetActive (true);
                canvasGroupDialog.alpha = 1f;
                break;
            default:
                break;
        }
    }

    // disableDialog: Chiude l'instanza di dialogo
    public void disableDialog () {
        // Imposto fattore di trasparenza a 0. Rendendo invisibile la finestra di dialogo.
        canvasGroupDialog.alpha = 0f;
        gameManager.gamePaused = false;
        dialogText.text = null;
        gameManager.setDisableCursor ();
        menuManager.getCanvasGroupPauseMenu ().blocksRaycasts = false;
        menuManager.getCanvasGroupPauseMenu ().interactable = false;
    }

    // OnTriggerEnter: Controlla quale collider ha invocato OnTriggerEnter e solleva un messaggio di dialogo
    private void OnTriggerEnter (Collider other) {
        if (other.tag == "Player") {
            if (gameObject.tag != null) {
                switch (menuManager.getScene ()) {
                    case "Level1":
                        switch (gameObject.tag) {
                            case "Finish":
                                if (gameManager.redGemCounter == gameManager.getRedGemMax ()) {
                                    GameData.save(menuManager.getScene(), gameManager.blueGemCounter);
                                    gameManager.setActiveCursor();
                                    menuManager.getCanvasEndGameMenu ().SetActive (true);
                                    gameManager.gamePaused = true;
                                    gameManager.getBackgroundGameSound ().Stop ();
                                } else {
                                    if (gameManager.redGemCounter != gameManager.getRedGemMax ()) {
                                        warningSound = GetComponent<AudioSource> ();
                                        warningSound.Play ();
                                        dialogPopup ("Non hai trovato ancora tutte le gemme rosse!");
                                    }
                                }
                                break;
                            case "blueGemTutorial":
                                if (tutorialTips[0] == false) {
                                    tutorialTips[0] = true;
                                    dialogPopup ("Le gemme verde acqua sono opzionali ma utili per incremantare i tuoi records personali");
                                }
                                break;
                            case "redGemTutorial":
                                if (tutorialTips[1] == false) {
                                    tutorialTips[1] = true;
                                    dialogPopup ("Le gemme rosse sono importanti perche senza di esse non potrai concludere il livello!");
                                }
                                break;
                            case "redPotionTutorial":
                                if (tutorialTips[2] == false) {
                                    tutorialTips[2] = true;
                                    dialogPopup ("La pozione rossa ti consente di effettuare due salti ravvicinati");
                                }
                                break;
                            case "greenPotionTutorial":
                                if (tutorialTips[3] == false) {
                                    tutorialTips[3] = true;
                                    dialogPopup ("Con la pozione verde puoi correre piu' velocemente per pochi secondi");
                                }
                                break;
                            case "bluePotionTutorial":
                                if (tutorialTips[6] == false) {
                                    tutorialTips[6] = true;
                                    dialogPopup ("La pozione blu ti consente di diventare invincibile per pochi secondi");
                                }
                                break;
                            case "heartTutorial":
                                if (tutorialTips[6] == false) {
                                    tutorialTips[6] = true;
                                    dialogPopup ("Se non vuoi perdere, prendi tutti i cuori che trovi in giro!");
                                }
                                break;
                            case "medikitTutorial":
                                if (tutorialTips[6] == false) {
                                    tutorialTips[6] = true;
                                    dialogPopup ("Se non hai un cuore completo, con il medikit puoi acquisire il 100% della vita");
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    case "Level2":
                        switch (gameObject.tag) {
                            case "Finish":
                                if (gameManager.redGemCounter == gameManager.getRedGemMax ()) {
                                    GameData.save(menuManager.getScene(), gameManager.blueGemCounter);
                                    gameManager.setActiveCursor();
                                    menuManager.getCanvasEndGameMenu ().SetActive (true);
                                    gameManager.gamePaused = true;
                                    gameManager.getBackgroundGameSound ().Stop ();
                                } else {
                                    if (gameManager.redGemCounter != gameManager.getRedGemMax ()) {
                                        warningSound = GetComponent<AudioSource> ();
                                        warningSound.Play ();
                                        dialogPopup ("Non hai trovato ancora tutte le gemme rosse!");
                                    }
                                }
                                break;
                                default: break;
                        }               
                        break;
                     case "Level3":
                        switch (gameObject.tag) {
                            case "Finish":
                                GameData.save(menuManager.getScene(), gameManager.blueGemCounter);
                                gameManager.setActiveCursor();
                                menuManager.canvasBossMenu.SetActive (true);
                                gameManager.gamePaused = true;
                                gameManager.getBackgroundGameSound ().Stop ();
                                break;
                                default: break;
                               }
                        break;
                    case "BossLevel":
                        switch (gameObject.tag) {
                            case "Finish":
                                GameData.save(menuManager.getScene(), gameManager.blueGemCounter);
                                gameManager.setActiveCursor();
                                menuManager.getCanvasEndGameMenu ().SetActive (true);          
                                gameManager.getBackgroundGameSound ().Stop ();
                                gameManager.gamePaused = true;
                                break;
                                default:
                                    break;
                        }
                        break;
                    default:
                        break;
                }
            }

        }
    }


    public void continueDialogButton () {
        gameManager.setDisableCursor();
        if (canvasGroupDialog.alpha == 1f) { 
            // Poiche' l'unico array e' quello di inizio livello, non ha senso continuare a incrementare il contatore. Se ho trovato tutte le gemme rosse non incremento.
             if(gameManager.getRedGemMax() != gameManager.redGemCounter){ 
                 messageCounter++;
             } 
            continueSound.Play ();            
            dialogPopupArray ();
        }
    }

    // dialogPopupArray: Chiude l'instanza di dialogo
    public void dialogPopupArray () {
        gameManager.setDisableCursor();
        if (gameManager.getRedGemMax() == gameManager.redGemCounter){
            disableDialog();
        }else {
        if (messageCounter < arraySize) {
            dialogPopup (((String) messages[messageCounter]));
        } else {
            disableDialog ();
        }
        }
    }

    // dialogPopup: Se chiamata mostra inizializza l'array dei messaggi alla canvas Dialog e mostra a video i messaggi
    public void dialogPopup (String message) {
        gameManager.setDisableCursor (); // Disattivo il cursore per rendere possibile l'interazione con il dialogo solo con i tasti
        dialogText.text = message;
        canvasGroupDialog.alpha = 1f;
        gameManager.gamePaused = true;
    }
}