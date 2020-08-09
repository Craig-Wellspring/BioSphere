using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Servius : MonoBehaviour
{
    public static Servius Server { get; private set; }


    private void Awake()
    {
        if (Server == null)
        {
            Server = this;
        }
        else
        {
            Destroy(gameObject); //should never happen
        }
    }
}
