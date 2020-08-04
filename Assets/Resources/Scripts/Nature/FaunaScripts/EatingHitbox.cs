using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatingHitbox : MonoBehaviour
{
    public GameObject hitFood;

    GameObject targetFood;

    void Start()
    {
        targetFood = GetComponentInParent<Metabolism>().currentTargetFood;
    }
    

    private void OnTriggerEnter(Collider _col)
    {
        if (_col.gameObject == targetFood)
        {
            hitFood = _col.gameObject;
            
            transform.parent.GetComponentInChildren<Animator>()?.SetBool("ProxyFood", true);
        }
    }

    private void OnTriggerExit(Collider _col)
    {
        transform.parent.GetComponentInChildren<Animator>()?.SetBool("ProxyFood", false);
    }
}
