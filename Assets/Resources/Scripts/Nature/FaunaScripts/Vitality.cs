using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.AI;
using System.Collections.Generic;
//using Pathfinding;

[RequireComponent(typeof(Metabolism))]
public class Vitality : MonoBehaviour
{
    [Header("Health")]
    public GameObject healthBar;
    public float maxHealth;
    public float currentHealth;
    [Space(10)]
    public bool dead = false;

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
    private AIBrainData AIData;
    private Animator AIBrain;
    private Metabolism metabolism;
    private Animator animator;
    private FoodData corpseFData;
    #endregion



    void Start()
    {
        AIData = GetComponent<AIBrainData>();
        AIBrain = GetComponent<Animator>();
        metabolism = GetComponent<Metabolism>();
        animator = transform.root.GetComponent<Animator>();
        corpseFData = corpse.GetComponent<FoodData>();
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
        corpseFData.nutritionalValue += metabolism.storedEnergy;
        metabolism.SpendEnergy(metabolism.storedEnergy);

        gameObject.SetActive(false);
    }
}
