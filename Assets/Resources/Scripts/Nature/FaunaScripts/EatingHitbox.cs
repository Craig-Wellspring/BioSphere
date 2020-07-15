using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatingHitbox : MonoBehaviour
{
    public GameObject targetFood;
    
    List<string> dietList;

    void Start()
    {
        dietList = GetComponentInParent<Metabolism>().dietList;
    }
    

    private void OnTriggerEnter(Collider _col)
    {
        if (dietList.Contains(_col.gameObject.tag))
        {
            targetFood = _col.gameObject;

            if (GetComponentInParent<Animator>() != null)
                GetComponentInParent<Animator>().SetTrigger("HitFood");
        }
    }
}
