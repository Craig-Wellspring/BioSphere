using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.root.GetComponentInChildren<SeedSpawner>() != null)
            collider.transform.root.GetComponentInChildren<SeedSpawner>().spawnSeed = false;
        Destroy(collider.transform.root.gameObject);
        Debug.Log(collider.transform.root.name + " was destroyed by a black hole.");
    }
}
