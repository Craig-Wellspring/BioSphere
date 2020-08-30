using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    void OnTriggerEnter(Collider collider)
    {
        Destroy(collider.transform.root.gameObject);
        Debug.Log(collider.transform.root.name + " was destroyed by a black hole.");
    }
}
