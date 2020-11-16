using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAttack : Attack
{
    void OnTriggerEnter(Collider _collider)
    {
        if (isActive && _collider.tag != "Ground")
        {
            DamageVitality(_collider.transform.root);
        }
    }
}
