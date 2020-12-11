using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScriptMushroom : MonoBehaviour
{

    //Animator animator;

    public const string ATTACK = "Attack";
    
    public const string RUN = "Run";
    
    public const string IDLE = "Idle";
    

    public Animation anim;
    public float distance, timeLeft, damage;
    GameManager gameManager;

    public float spostamento, startingTimeLeft;
    public bool isAttacking, chooseTarget, targetInPlatform;
    private AudioSource soundEffect;

    GameObject enemy;

    // Start is called before the first frame update
    void Start()
    {

        //animator = this.GetComponentInChildren<Animator>();

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        chooseTarget = false;
        damage = 25f;
        startingTimeLeft = 0.5f;
        timeLeft = startingTimeLeft;

        anim = this.GetComponentInChildren<Animation>();

        movementAnim();
        soundEffect =  this.gameObject.GetComponent<AudioSource>();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        setTarget();

        spostamento = this.GetComponent<NavMeshAgent>().velocity.x;
        distance = this.gameObject.GetComponent<NavMeshAgent>().remainingDistance;
        attack();
        movementAnim();
        
    }

    public void attack()
    {
        if (distance <= 1f && chooseTarget && targetInPlatform)
        {
            if (timeLeft < 0)
            {
                
                if (!gameManager.thePlayer.getInvincibilityBool())
                {
                    Debug.Log("attacco fungo si");
                    gameManager.editEnergyHealth(-damage);
                    isAttacking = true;
                    soundEffect.Play();                    
                    Vector3 knockbackDir = gameManager.thePlayer.transform.position-this.transform.position;
                    knockbackDir.y = 0;
                    gameManager.thePlayer.knockback(knockbackDir.normalized);
                }
                else
                {
                    Debug.Log("attacco fungo no");
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

    public void IdleAni()
    {
        
        anim.Play(IDLE);
    }

    public void RunAni()
    {
        anim.Play(RUN);
    }

    public void AttackAni()
    {
        anim.Play(ATTACK);
    }

    public void movementAnim()
    {
        spostamento = this.GetComponent<NavMeshAgent>().velocity.x;

        if ((Mathf.Abs(spostamento)) > 0)
        {
            RunAni();
            
        }
        else if (isAttacking)
        {
            AttackAni();
        }
        else
        {
            IdleAni();
        }
        
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