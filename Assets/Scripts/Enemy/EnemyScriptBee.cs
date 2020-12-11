using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScriptBee : MonoBehaviour
{
    public float spostamento;
    public float distance, timeLeft, damage, startingTimeLeft;
    GameManager gameManager;
    public Animator anim;
    public bool isAttacking, chooseTarget, targetInPlatform;
    GameObject enemy;
    private AudioSource soundEffect;
    

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        chooseTarget = false;
        damage = 25f;
        startingTimeLeft = 1f;
        timeLeft = startingTimeLeft;
        soundEffect =  this.gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        anim = gameObject.GetComponentInChildren<Animator>();
        basicAnimMovement();

        setTarget();

        spostamento = this.GetComponent<NavMeshAgent>().velocity.x;
        distance = this.gameObject.GetComponent<NavMeshAgent>().remainingDistance;
        attack();
        
    }

    public void attack()
    {
        
        if (distance <= 2f && chooseTarget && targetInPlatform)
        {

            if (timeLeft < 0)
            {
                
                if ((!gameManager.thePlayer.getInvincibilityBool()))
                {
                    isAttacking = true;
                    gameManager.editEnergyHealth(-damage);
                    soundEffect.Play();
                }
                else
                {
                    isAttacking = false;
                }


                timeLeft = startingTimeLeft;

            }
            else
            {
                timeLeft = timeLeft - Time.deltaTime;
            }

        }

    }

    public void basicAnimMovement()
    {

        anim.SetBool("isAttacking", isAttacking);
        anim.SetFloat("speed", Mathf.Abs(spostamento));

    }

    public void setTarget()
    {
        if (chooseTarget)
        {
            this.gameObject.GetComponent<NavMeshAgent>().destination = gameManager.thePlayer.transform.position;
        }
        else
        {
            this.gameObject.GetComponent<NavMeshAgent>().destination = gameObject.transform.position;
        }
    }


}
