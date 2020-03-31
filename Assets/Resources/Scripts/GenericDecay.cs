using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericDecay : MonoBehaviour
{
    public float decayTime;

    private void OnEnable()
    {
        Expire();
    }

    private void Expire()
    {
        Destroy(this.gameObject, decayTime);
    }
}
