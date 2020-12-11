using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyHealth : MonoBehaviour
{

    Image energyHealthImage;
    float maxHealth = 100f;
    public float health;


    void Start()
    {
        energyHealthImage = GetComponent<Image>();
        health = 100f;
    }

    // Update is called once per frame
    void Update()
    {
        energyHealthImage.fillAmount = health / maxHealth;
    }

    public void editEnergyHealthImage(float healthEdit)
    {
        health = healthEdit;
        if (health < 0) health = 0;
        if (health > 100f) health = 100f;
    }


}