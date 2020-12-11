using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class endLevel : MonoBehaviour
{
    public GameObject cameraBoss, camera;
    private GameObject[] switches;
    private switchButton button;
    public bool endLevelActive, youWin;
    private GameObject boss;
    public GameObject[] platforms;

    
    // Start is called before the first frame update
    void Start()
    {
        youWin = false;
        cameraBoss = GameObject.Find("CameraBoss");
        camera = GameObject.Find("Camera");

        platforms = new GameObject[18];
        
        for (int i = 0; i < 17; i++)
        {
            platforms[i] = GameObject.Find("bossPlate"+ ((i+1).ToString()));
            platforms[i].SetActive(false);
        }


        switches = new GameObject[5];
        switches[0] = GameObject.Find("switch0");
        switches[1] = GameObject.Find("switch1");
        switches[2] = GameObject.Find("switch2");
        switches[3] = GameObject.Find("switch3");
        switches[4] = GameObject.Find("switch4");
        boss = GameObject.Find("boss");
        boss.GetComponent<Rigidbody>().useGravity = false;

        endLevelActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        checkEndLevel();
        
    }
   
    public void checkEndLevel()
    {
        if (!endLevelActive)
        {
            bool flag = false;

            for (int i = 0; i < switches.Length; i++)
            {

                if ((switches[i].GetComponent<switchButton>().getActiveButton()) == false)
                {
                    flag = true;
                    break;
                }
            }

            if (flag == false)
            {
                endLevelActive = true;
                endLevelAction();
            }
        }
        

    }

    public void endLevelAction()
    {
        
        setActiveCameraBoss();
        boss.GetComponent<Rigidbody>().useGravity = true;
        for (int i = 0; i < 17; i++)
        {
            platforms[i].SetActive(true);
        }

    }

    public void setActiveCameraBoss()
    {
        cameraBoss.GetComponent<Camera>().enabled = (camera.GetComponent<Camera>().enabled);
        camera.GetComponent<Camera>().enabled = !(camera.GetComponent<Camera>().enabled);
        
    }

    public void setActiveCamera()
    {
        camera.GetComponent<Camera>().enabled = !(camera.GetComponent<Camera>().enabled);
        //cameraBoss.GetComponent<Camera>().enabled = !(camera.GetComponent<Camera>().enabled);
    }
    
}
