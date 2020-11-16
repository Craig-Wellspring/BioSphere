using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderAttack : Attack
{
    void OnCollisionEnter(Collision _collision)
    {
        if (isActive && _collision.collider.tag != "Ground")
        {
            DamageVitality(_collision.collider.transform.root);
        }
    }
}
