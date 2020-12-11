using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class switchButton : MonoBehaviour
{

    private bool activeButton;
    

    // Start is called before the first frame update
    void Start()
    {;
        activeButton = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool getActiveButton()
    {
        return activeButton;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!activeButton)
        {
            gameObject.transform.Rotate(0, 0, 80);
            activeButton = true;
            FindObjectOfType<audioManager>().audiosource.PlayOneShot(FindObjectOfType<audioManager>().audioSwitch);
        }
            
    }
}
