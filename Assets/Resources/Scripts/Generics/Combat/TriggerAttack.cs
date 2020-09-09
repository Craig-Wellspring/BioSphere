using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAttack : MonoBehaviour
{
    public bool isActive = true;

    [Header("Debug")]
    [SerializeField] private bool logAttacks = false;


    [Header("Settings")]
    [SerializeField] private int attackDamage = 1;

    
    void OnTriggerEnter(Collider collider)
    {
        if (isActive && collider.tag != "Ground")
        {
            Vitality targetVitality = collider.transform.root.GetComponentInChildren<Vitality>();
            if (targetVitality)
            {
                targetVitality.TakeDamage(attackDamage);

                if (logAttacks)
                    Debug.Log(transform.root.name + " struck " + collider.transform.root.name + " with an attack.");
            }
        }
    }
}
