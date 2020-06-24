using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatingHitbox : MonoBehaviour
{
    public bool hitEdibleFood = false;
    public GameObject targetFood;

    //Collider hitbox;
    List<string> dietList;

    Animator AIMachine;

    void Start()
    {
        //hitbox = GetComponent<SphereCollider>();
        dietList = GetComponentInParent<Metabolism>().dietList;
        AIMachine = GetComponentInParent<Animator>();
    }
    

    private void OnTriggerEnter(Collider _col)
    {
        if (dietList.Contains(_col.gameObject.tag))
        {
            Debug.Log(transform.root.gameObject.name + " hit " + _col.gameObject.name + " with EatingHitbox");
            hitEdibleFood = true;

            targetFood = _col.gameObject;

            if (AIMachine != null)
                AIMachine.SetTrigger("HitFood");
        }
    }


    private void OnTriggerExit(Collider _col)
    {
        hitEdibleFood = false;
    }

    private void OnDisable()
    {
        hitEdibleFood = false;
    }
}
