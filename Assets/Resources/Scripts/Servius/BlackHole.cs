using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    [SerializeField] bool active;

    void OnTriggerEnter(Collider collider)
    {
        if (active)
        {
            Destroy(collider.transform.root.gameObject);
            Debug.Log(collider.transform.root.name + " was destroyed by a black hole.");
        }
    }
}
