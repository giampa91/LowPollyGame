using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage : MonoBehaviour {
    Transform playerTransform;
    public float range = 20f;
    public GameObject projectilePrefab;
    float time = 0;
    private AudioSource soundEffect;

    // Start is called before the first frame update
    void Start () {
        GameManager gameManager = FindObjectOfType<GameManager> ();
        playerTransform = gameManager.thePlayer.transform;
        time = Time.time;
        soundEffect = this.gameObject.GetComponent<AudioSource> ();
    }

    // Update is called once per frame
    void FixedUpdate () {
        if (Time.time - time > 2 && (playerTransform.position - transform.position).magnitude <= range) {
            Vector3 spawnPosition = this.transform.position;
            spawnPosition.y += transform.lossyScale.y;
            Spell spell = Spell.shoot (projectilePrefab, spawnPosition, 4);
            spell.EffectDurationSeconds = 1f;
            time = Time.time;
            soundEffect.Play ();
        }
        Vector3 playerDirection = playerTransform.position;
        playerDirection.y = 0;
        transform.LookAt (playerDirection);
    }

}