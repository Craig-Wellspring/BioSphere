using UnityEngine;
using System.Collections.Generic;

public class Vitality : MonoBehaviour
{
    public bool dead = false;
    [Space(10)]
    [Header("Health")]
    public GameObject healthBar;
    public float maxHealth;
    public float currentHealth;

    [Header("Body Parts")]
    [Tooltip("GameObject that represents the Creature's corpse")]
    public GameObject corpse;
    [Tooltip("GameObjects that contain the Creature's body colliders")]
    public List<Collider> bodyColliders;

    #region Private Variables
    //Events
    public event System.Action DamageTaken;
    public event System.Action DeathOccurs;

    //Cache
    private EnergyData corpseEData;
    private EnergyData selfEData;
    #endregion



    void Start()
    {
        corpseEData = corpse.GetComponent<EnergyData>();
        selfEData = GetComponent<EnergyData>();
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

        //Update Animator
        if (healthBar.activeSelf)
            healthBar.SetActive(false);


        //Deactivate Body and Activate Corpse
        foreach (Collider body in bodyColliders)
            body.enabled = false;
        corpse.SetActive(true);

        //Transfer Energy to Corpse
        corpseEData.nutritionalValue += selfEData.energyReserve;
        selfEData.energyReserve = 0;

        gameObject.SetActive(false);
    }
}
