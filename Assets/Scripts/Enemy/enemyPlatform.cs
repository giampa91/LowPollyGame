using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyPlatform : MonoBehaviour
{
    public EnemyScriptBee bee;
    public EnemyScriptMushroom mushroom;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        
        if ((other.tag == "Player") && gameObject.tag == "beePlatform")
        {
            if(bee != null)
            {
                beeActivateFunctions(bee);
                bee.chooseTarget = true;
            }
            
        }
        else if ((other.tag == "Player") && gameObject.tag == "mushroomPlatform")
        {

            if (bee != mushroom)
            {
                mushroomActivateFunctions(mushroom);
                mushroom.chooseTarget = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {

        if ((other.tag == "Player") && gameObject.tag == "beePlatform")
        {
            if (bee != null)
            {
                beeDeactivateFunctions(bee);
                bee.chooseTarget = false;
            }
        }
        else if ((other.tag == "Player") && gameObject.tag == "mushroomPlatform")
        {
            if (mushroom != null)
            {
                mushroomDeactivateFunctions(mushroom);
                mushroom.chooseTarget = false;
            }
        }
    }

    public void mushroomActivateFunctions(EnemyScriptMushroom mushroom)
    {
        mushroom.chooseTarget = true;
        mushroom.targetInPlatform = true;
    }

    public void mushroomDeactivateFunctions(EnemyScriptMushroom mushroom)
    {
        mushroom.chooseTarget = false;
        mushroom.targetInPlatform = false;
        mushroom.isAttacking = false;
    }

    public void beeActivateFunctions(EnemyScriptBee bee)
    {
        bee.chooseTarget = true;
        bee.targetInPlatform = true;
    }

    public void beeDeactivateFunctions(EnemyScriptBee bee)
    {
        bee.chooseTarget = false;
        bee.targetInPlatform = false;
        bee.isAttacking = false;
    }
}

