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
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;

        // Show health bar if not at Max
        healthBar.gameObject.SetActive(!currentHealth.Equals(maxHealth));
    }

    public void UpdateMaxHealth(int _amount)
    {
        maxHealth += _amount;

        UpdateCurrentHealth(_amount);

        transform.root.localScale += new Vector3(_amount / 10, _amount / 10, _amount / 10);
    }

    void UpdateCurrentHealth(int _amount = 0)
    {
        currentHealth += _amount;

        if (healthBar != null)
            UpdateHealthBar();

        if (currentHealth <= 0)
            Die();
    }



    //public event System.Action DamageTaken;
    public void TakeDamage(int _amount)
    {
        if (!dead)
        {
            //DamageTaken?.Invoke();

            UpdateCurrentHealth(-_amount);

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
