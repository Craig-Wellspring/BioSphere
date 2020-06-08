using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericDecay : MonoBehaviour
{
    public float decayTime;
    public bool destroyRoot = false;

    private void OnEnable()
    {
        Expire();
    }

    private void Expire()
    {
        if (destroyRoot)
            Destroy(transform.root.gameObject, decayTime);
        else
            Destroy(this.gameObject, decayTime);
    }
}
