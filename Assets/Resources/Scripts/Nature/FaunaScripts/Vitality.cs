using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Pathfinding;

[RequireComponent(typeof(CreatureData))]
public class Vitality : MonoBehaviour
{
    [Header("State")]
    public float currentHealth;


    [Header("Settings")]
    public float maxHealth;


    [Header("Debug")]
    public bool dead = false;



    #region Private Variables
    //Events
    public event System.Action DamageTaken;
    public event System.Action DeathOccurs;

    //Cache
    CreatureData cData;
    NutritionalValue corpseNV;
    EnergyData selfEData;
    Slider healthBar;
    #endregion



    void Start()
    {
        cData = GetComponent<CreatureData>();
        if (cData.corpse == null)
            throw new System.Exception("No corpse found");
        corpseNV = cData.corpse.GetComponent<NutritionalValue>();
        selfEData = GetComponent<EnergyData>();

        currentHealth = cData.maxHealth.value;

        healthBar = transform.root.Find("Canvas").Find("Health Bar").GetComponent<Slider>();
        healthBar.maxValue = maxHealth;
        healthBar.value = healthBar.maxValue;
        healthBar.gameObject.SetActive(false);
    }

    public void IncreaseMaxHealth(int _amount)
    {
        currentHealth += _amount;
        maxHealth += _amount;
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;

        transform.root.localScale += new Vector3(_amount / 10, _amount / 10, _amount / 10);
    }
    public void DecreaseMaxHealth(int _amount)
    {
        currentHealth -= _amount;
        maxHealth -= _amount;
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;

        transform.root.localScale -= new Vector3(_amount / 10, _amount / 10, _amount / 10);
    }

    void IncreaseCurrentHealth(int _amount)
    {
            healthBar.value += _amount;    
            currentHealth += _amount;
    }
    void DecreaseCurrentHealth(int _amount)
    {
            healthBar.value -= _amount;    
            currentHealth -= _amount;

            // Show health bar if not already
            if (!healthBar.gameObject.activeSelf)
                healthBar.gameObject.SetActive(true);
    }

    public void TakeDamage(int _amount)
    {
        if (!dead)
        {
            DamageTaken?.Invoke();

            DecreaseCurrentHealth(_amount);

            if (currentHealth <= 0)
                Die();
        }
    }

    public void Die()
    {
        //Die
        dead = true;
        DeathOccurs?.Invoke();

        transform.root.gameObject.name += " (Dead)";

        if (GetComponent<VisualPerception>())
            GetComponent<VisualPerception>().enabled = false;

        if (transform.root.GetComponent<AIPathAlignedToSurface>())
            transform.root.GetComponent<AIPathAlignedToSurface>().enabled = false;

        //Update Animator
        if (healthBar.gameObject.activeSelf)
            healthBar.gameObject.SetActive(false);


        //Deactivate Body and Activate Corpse
        cData.mainBodyCollider.enabled = false;
        foreach (Collider _body in cData.adlBodyColliders)
            _body.enabled = false;

        cData.corpse.SetActive(true);

        //Transfer Energy to Corpse
        corpseNV.nutritionalValue += selfEData.energyReserve;
        selfEData.energyReserve = 0;
    }
}
