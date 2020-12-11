using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class healtBarScript : MonoBehaviour
{
    // Start is called before the first frame update

    Image healthBar;
    float maxHealth = 100f;
    public float health;


    void Start()
    {
        healthBar = GetComponent<Image>();
        health = 100f;
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.fillAmount = health / maxHealth;
    }

    public void editHealth(float healthEdit)
    {
        health = healthEdit + health;
        if (health < 0) health = 0;
        if (health > 100f) health = 100f;
    }
}
