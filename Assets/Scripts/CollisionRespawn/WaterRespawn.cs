using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterRespawn : MonoBehaviour
{

    private GameManager gameManager;
    private Vector3 playerPosition;
    private AudioSource splashEffectSound;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        splashEffectSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            splashEffectSound.Play();
            gameManager.triggerDead();
            gameManager.respawPlayer();
        }
    }

}
