using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider)
    {
        Destroy(collider.transform.root.gameObject);
    }
}
