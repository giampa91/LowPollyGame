using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickUp : MonoBehaviour {
    // Start is called before the first frame update
    public int value;
    public float timeLeft, startTimeLeft;
    public bool timeLeftBool;
    private GameManager gameManager;

    BoxCollider collider;
    MeshRenderer mesh;

    private AudioSource soundObject;

    void Start () {

        gameManager = FindObjectOfType<GameManager> ();

        // prendere i componenti
        collider = gameObject.GetComponent<BoxCollider> ();
        mesh = gameObject.transform.GetChild (0).GetComponent<MeshRenderer> ();

        value = 1;
        startTimeLeft = 10f;
        timeLeft = startTimeLeft;
        soundObject = GetComponent<AudioSource> ();

    }

    // Update is called once per frame
    void Update () {
        timeLeftEvent ();
    }

    private void OnTriggerEnter (Collider other) {

        if (other.tag == "Player") {
            if (gameObject.tag == "medikit" || gameObject.tag == "potionRed" || gameObject.tag == "potionBlu" || gameObject.tag == "potionGreen") {
                medikit ();
                powerUpsToEnable ();
                timeLeftBool = true;
                soundObject.Play ();
            } else {
                if (gameObject.tag == "blueGem") {
                    gameManager.addGemBlue ();
                    soundObject.Play ();
                }
                if (gameObject.tag == "redGem") {
                    gameManager.addGemRed ();
                    soundObject.Play ();
                } else if (gameObject.tag == "heart") {
                    gameManager.editHealtNumber (1);
                    soundObject.Play ();
                }
            }
            // Rendi invisibile oggetto. 
            // Utilizzare Destroy(gameObject) comporta non esecuzione dell'audio associato all'oggetto.
            collider.enabled = !collider.enabled;
            mesh.enabled = false;
        }
    }

    public void addGemRed () {
        gameManager.addGemRed ();
    }

    public void addGemBlue () {
        gameManager.addGemBlue ();
    }

    public void medikit () {
        if (gameObject.tag == "medikit") {
            // Calcolo quanta vita manca e l'aggiungo la differenza 
            float restoreHealth = 100f - gameManager.energyHealth;
            gameManager.editEnergyHealth (restoreHealth);
        }
    }

    public void powerUpsToEnable () {
        Color color;
        switch (gameObject.tag) {
            case "potionRed":
                color = new Color32 (235, 59, 90, 255);
                gameManager.colorText = color;
                gameManager.getPowerUpsTextCanvas ().color = color;
                gameManager.getPowerUpsTextCanvas ().text = "Doppio salto";
                gameManager.thePlayer.setDobleJumpBool (true);
                break;
            case "potionBlu":
                color = new Color32 (69, 170, 242, 255);
                gameManager.colorText = color;
                gameManager.getPowerUpsTextCanvas ().color = color;
                gameManager.getPowerUpsTextCanvas ().text = "Invicibilita'";
                gameManager.thePlayer.setInvincibilityBool (true);
                break;
            case "potionGreen":
                color = new Color32 (38, 222, 129, 255);
                gameManager.colorText = color;
                gameManager.getPowerUpsTextCanvas ().color = color;
                gameManager.getPowerUpsTextCanvas ().text = "Sprint";
                gameManager.thePlayer.setMoveFastBool (true);
                break;
            default:
                break;
        }
    }

    public void powerUpsToDisable () {
        gameManager.getPowerUpsTextCanvas ().color = new Color32 (0, 0, 0, 0); // Setto colore trasparente sulla canvas del power ups
        if (gameManager.thePlayer.getdoubleJumpBool () == true) {
            gameManager.thePlayer.setDobleJumpBool (false);
        } else if (gameManager.thePlayer.getInvincibilityBool () == true) {
            gameManager.thePlayer.setInvincibilityBool (false);
        } else if (gameManager.thePlayer.getMoveFastBool () == true) {
            gameManager.thePlayer.setMoveFastBool (false);
        }

    }

    public void timeLeftEvent () {
        if (timeLeftBool == true) {
            if (timeLeft > 0) {
                timeLeft = timeLeft - Time.deltaTime;
                if (timeLeft < 6f) {
                    gameManager.timeOutPowerUps = true;
                }
            } else {
                gameManager.timeOutPowerUps = false;
                timeLeftBool = false;
                timeLeft = startTimeLeft;
                powerUpsToDisable ();
                collider.enabled = !collider.enabled;
                mesh.enabled = true;
            }
        }
    }
}