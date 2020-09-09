using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderAttack : MonoBehaviour
{
    public bool isActive = false;
    
    [Header("Debug")]
    [SerializeField] private bool logAttacks = false;


    [Header("Settings")]
    [SerializeField] private int attackDamage = 1;


    void OnCollisionEnter(Collision _collision)
    {
        if (isActive && _collision.collider.tag != "Ground")
        {
            Vitality targetVitality = _collision.collider.transform.root.GetComponentInChildren<Vitality>();
            if (targetVitality)
            {
                targetVitality.TakeDamage(attackDamage);

                if (logAttacks)
                    Debug.Log(transform.root.name + " struck " + _collision.transform.root.name + " with an attack.");
            }
        }
    }
}
