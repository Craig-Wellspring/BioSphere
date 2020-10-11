using UnityEngine;
using UnityEngine.UI;
using Pathfinding;

[RequireComponent(typeof(CreatureStats))]
public class Vitality : MonoBehaviour
{
    [Header("State")]
    public float currentHealth;


    [Header("Settings")]
    public float maxHealth;


    [Header("Debug")]
    public bool dead = false;


    //Cache
    Slider healthBar;


    void Start()
    {
        currentHealth = maxHealth;

        healthBar = transform.root.Find("Canvas").Find("Health Bar").GetComponent<Slider>();
        healthBar.maxValue = maxHealth;
        healthBar.value = healthBar.maxValue;
        healthBar.gameObject.SetActive(false);
    }

    public void IncreaseMaxHealth(int _amount)
    {
        maxHealth += _amount;
        healthBar.maxValue = maxHealth;
        IncreaseCurrentHealth(_amount);

        transform.root.localScale += new Vector3(_amount / 10, _amount / 10, _amount / 10);
    }
    public void DecreaseMaxHealth(int _amount)
    {
        maxHealth -= _amount;
        healthBar.maxValue = maxHealth;
        DecreaseCurrentHealth(_amount);

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


    public event System.Action DamageTaken;
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


    public event System.Action DeathOccurs;
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


        //Deactivate Body
        BodyReference body = transform.root.GetComponent<BodyReference>();
        body.mainBodyCollider.enabled = false;
        foreach (Collider _body in body.adlBodyColliders)
            _body.enabled = false;

        // Activate Corpse
        if (body.corpse != null)
        {
            body.corpse.SetActive(true);

            //Transfer Energy to Corpse
            EnergyData selfEData = GetComponent<EnergyData>();
            FoodData corpseFData = body.corpse.GetComponent<FoodData>();
            if (selfEData && corpseFData)
            {
                corpseFData.AddNV(selfEData.energyReserve);
                selfEData.RemoveEnergy(selfEData.energyReserve);
            }
        }
    }
}
