using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDestroyEvent : MonoBehaviour
{
    private bool isPlaying = true;

    public event Action BeingDestroyed;
    

    private void OnApplicationQuit()
    {
        isPlaying = false;
    }

    private void OnDisable()
    {
        if (isPlaying)
        {
            BeingDestroyed?.Invoke();
        }
    }
}
