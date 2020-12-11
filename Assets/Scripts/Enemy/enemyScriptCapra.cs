using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class enemyScriptCapra : MonoBehaviour
{
    public GameObject target;
    public float distanza;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        this.gameObject.GetComponent<NavMeshAgent>().destination = target.transform.position;
        distanza = this.gameObject.GetComponent<NavMeshAgent>().remainingDistance;
    }
}
