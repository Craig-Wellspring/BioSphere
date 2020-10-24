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
    [SerializeField] float healthIncrement = 2f;


    [Header("Corpse")]
    [Tooltip("GameObject that represents the Creature's corpse")]
    public GameObject corpse;


    [Header("Debug")]
    public bool dead = false;


    //Cache
    Slider healthBar;


    void Start()
    {
        currentHealth = maxHealth;

        healthBar = transform.root.Find("Canvas").Find("Health Bar").GetComponent<Slider>();
        UpdateHealthBar();

        // Register Max Health in StatBlock
        GetComponent<CreatureStats>()?.AddNewStat("Health", maxHealth, healthIncrement);
    }

    void UpdateHealthBar()
    {
        healthBar.value = currentHealth;

        // Show health bar if not at Max
        healthBar.gameObject.SetActive(!currentHealth.Equals(maxHealth));
    }

    public void ChangeMaxHealth(int _amount)
    {
        maxHealth += _amount;
        if (healthBar != null)
            healthBar.maxValue = maxHealth;

        ChangeCurrentHealth(_amount);

        transform.root.localScale += new Vector3(_amount / 10, _amount / 10, _amount / 10);
    }

    void ChangeCurrentHealth(int _amount = 0)
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

            ChangeCurrentHealth(-_amount);

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

        if (transform.root.TryGetComponent<AIPathAlignedToSurface>(out AIPathAlignedToSurface aiPath))
            aiPath.enabled = false;

        if (transform.root.TryGetComponent<AIDestinationSetter>(out AIDestinationSetter destinationSetter))
            destinationSetter.enabled = false;

        //Deactivate Body
        BodyReference body = transform.root.GetComponent<BodyReference>();
        body.mainBodyCollider.enabled = false;
        foreach (Collider _body in body.adlBodyColliders)
            _body.enabled = false;

        // Activate Corpse
        if (corpse != null)
        {
            corpse.SetActive(true);

            //Transfer Energy to Corpse
            if (TryGetComponent<EnergyData>(out EnergyData selfEData) && TryGetComponent<FoodData>(out FoodData corpseFData))
                if (selfEData.RemoveEnergy(selfEData.energyReserve))
                    corpseFData.AddNV(selfEData.energyReserve);
        }

        // Deactivate Canvas
        transform.root.Find("Canvas").gameObject.SetActive(false);

        // Deactivate Vitals
        this.gameObject.SetActive(false);
    }
}
