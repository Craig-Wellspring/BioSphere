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
    EnergyData corpseEData;
    EnergyData selfEData;
    Slider healthBar;
    #endregion



    void Start()
    {
        cData = GetComponent<CreatureData>();
        corpseEData = cData.corpse.GetComponent<EnergyData>();
        selfEData = GetComponent<EnergyData>();

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

    public void TakeDamage(int _amount)
    {
        if (!dead)
        {
            DamageTaken?.Invoke();

            if (!healthBar.gameObject.activeSelf)
                healthBar.gameObject.SetActive(true);

            healthBar.value -= _amount;    
            currentHealth -= _amount;


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
        foreach (Collider _body in cData.bodyColliders)
            _body.enabled = false;
        cData.corpse.SetActive(true);

        //Transfer Energy to Corpse
        corpseEData.nutritionalValue += selfEData.energyReserve;
        selfEData.energyReserve = 0;
    }
}
