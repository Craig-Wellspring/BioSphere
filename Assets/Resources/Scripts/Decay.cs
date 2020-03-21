using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decay : MonoBehaviour
{
    public float decayTime;

    private float lifeTimer = 0;
    
    void Update()
    {
        lifeTimer += Time.deltaTime;

        if (lifeTimer > decayTime)
            Expire();
    }

    private void Expire()
    {
        Destroy(this.gameObject);
    }
}
