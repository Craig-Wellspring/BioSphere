using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Collections.Generic;
using Pathfinding;

public class Vitality : MonoBehaviour
{
    [Header("Health")]
    public GameObject creatureCanvas;
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
    private CreatureAI creatureAI;
    private Metabolism metabolism;
    private Animator animator;
    private CreatureData cData;
    #endregion



    void Start()
    {
        creatureAI = GetComponent<CreatureAI>();
        metabolism = GetComponent<Metabolism>();
        cData = GetComponent<CreatureData>();
        animator = GetComponentInParent<Animator>();
    }

    public void TakeDamage(int amount)
    {
        if (!dead)
        {
            creatureCanvas.gameObject.SetActive(true);
            currentHealth -= amount;
            animator.SetTrigger("TakeDamage");

            if (metabolism.isEating)
                metabolism.StopEating();

            if (currentHealth <= 0)
                Die();
        }
    }

    /*public void OnValidate()
    {
        if (dead)
            Die();
        else
            Revivify();
    }*/

    public void Die()
    {
        //Die
        //animator.SetBool("Dead", true);
        dead = true;
        if (creatureCanvas.gameObject.activeSelf)
            creatureCanvas.gameObject.SetActive(false);
        foreach (Collider body in bodyColliders)
            body.enabled = false;
        
        corpse.gameObject.SetActive(true);

        corpse.GetComponent<FoodData>().nutritionalValue += cData.energyUnits;
        cData.energyUnits = 0;
        
        if (creatureAI != null)
            creatureAI.enabled = false;

        //Decay
        //this.gameObject.GetComponent<Decay>().enabled = true;
    }

    public void Revivify()
    {
        //Come back to life
        //animator.SetBool("Dead", false);
        dead = false;
        creatureCanvas.gameObject.SetActive(true);
        foreach (Collider body in bodyColliders)
            body.enabled = true;

        corpse.GetComponent<FoodData>().nutritionalValue = 0;
        corpse.gameObject.SetActive(false);

        //GetComponent<NavMeshAgent>().enabled = true;
        if (creatureAI != null)
            creatureAI.enabled = true;

        //Stop Decaying
        //this.gameObject.GetComponent<Decay>().enabled = false;

    }
}
