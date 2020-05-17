using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Pathfinding;

public class Vitality : MonoBehaviour
{
    [Header("Body Parts")]
    [Tooltip("GameObject that represents the Creature's corpse")]
    public GameObject corpse;
    [Tooltip("GameObjects that contain the Creature's body colliders")]
    public List<Collider> bodyColliders;

    private CreatureAI creatureAI;
    private Animator animator;
    
    [Header("Health")]
    public float maxHealth;
    public float currentHealth;
    [Space(10)]
    public bool dead = false;


    void Start()
    {
        animator = GetComponent<Animator>();
        creatureAI = GetComponentInChildren<CreatureAI>();
    }

    public void TakeDamage(int amount)
    {
        if (!dead)
        {
            currentHealth -= amount;
            animator.SetTrigger("TakeDamage");

            if (creatureAI.eating)
                creatureAI.StopEating();

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
        animator.SetBool("Dead", true);
        dead = true;
        foreach (Collider body in bodyColliders)
            body.enabled = false;
        
        corpse.gameObject.SetActive(true);

        GetComponent<NavMeshAgent>().enabled = false;
        if (creatureAI != null)
            creatureAI.enabled = false;

        //Decay
        //this.gameObject.GetComponent<Decay>().enabled = true;
    }

    public void Revivify()
    {
        //Come back to life
        animator.SetBool("Dead", false);
        dead = false;
        foreach (Collider body in bodyColliders)
            body.enabled = true;

        corpse.gameObject.SetActive(false);

        GetComponent<NavMeshAgent>().enabled = true;
        if (creatureAI != null)
            creatureAI.enabled = true;

        //Stop Decaying
        //this.gameObject.GetComponent<Decay>().enabled = false;

    }
}
