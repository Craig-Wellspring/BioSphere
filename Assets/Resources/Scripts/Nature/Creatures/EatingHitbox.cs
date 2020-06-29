using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatingHitbox : MonoBehaviour
{
    public bool hitEdibleFood = false;
    public GameObject targetFood;

    //Collider hitbox;
    List<string> dietList;

    Animator AIBrain;

    void Start()
    {
        //hitbox = GetComponent<SphereCollider>();
        dietList = GetComponentInParent<Metabolism>().dietList;
        AIBrain = GetComponentInParent<Animator>();
    }
    

    private void OnTriggerEnter(Collider _col)
    {
        if (dietList.Contains(_col.gameObject.tag))
        {
            //Debug.Log(transform.root.gameObject.name + " hit " + _col.gameObject.name + " with EatingHitbox");
            hitEdibleFood = true;

            targetFood = _col.gameObject;

            if (AIBrain != null)
                AIBrain.SetTrigger("HitFood");
        }
    }


    private void OnTriggerExit(Collider _col)
    {
        hitEdibleFood = false;
        targetFood = null;
    }

    private void OnDisable()
    {
        hitEdibleFood = false;
        targetFood = null;
    }
}
