using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boss : MonoBehaviour
{

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        FindObjectOfType<endLevel>().setActiveCamera();
        FindObjectOfType<audioManager>().audiosource.PlayOneShot(FindObjectOfType<audioManager>().audioBoss);
        FindObjectOfType<DialogManager>().dialogPopup("salva i pulcini per completare il livello");
        Destroy(gameObject);
    }
}
