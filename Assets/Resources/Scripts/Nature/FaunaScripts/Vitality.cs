using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(CreatureData))]
public class Vitality : MonoBehaviour
{
    [Header("Debug")]
    public bool dead = false;

    [Header("State")]
    public float currentHealth;
    [Header("Settings")]
    public float maxHealth;

    #region Private Variables
    //Events
    public event System.Action DamageTaken;
    public event System.Action DeathOccurs;

    //Cache
    CreatureData cData;
    EnergyData corpseEData;
    EnergyData selfEData;
    GameObject healthBar;
    #endregion



    void Start()
    {
        cData = GetComponent<CreatureData>();
        corpseEData = cData.corpse.GetComponent<EnergyData>();
        selfEData = GetComponent<EnergyData>();

        healthBar = transform.root.Find("Canvas").Find("Health Bar").gameObject;
        healthBar.SetActive(false);
    }

    public void TakeDamage(int amount)
    {
        if (!dead)
        {
            DamageTaken?.Invoke();

            if (!healthBar.activeSelf)
                healthBar.SetActive(true);
            currentHealth -= amount;


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

        //Update Animator
        if (healthBar.activeSelf)
            healthBar.SetActive(false);


        //Deactivate Body and Activate Corpse
        foreach (Collider _body in cData.bodyColliders)
            _body.enabled = false;
        cData.corpse.SetActive(true);

        //Transfer Energy to Corpse
        corpseEData.nutritionalValue += selfEData.energyReserve;
        selfEData.energyReserve = 0;
    }
}
