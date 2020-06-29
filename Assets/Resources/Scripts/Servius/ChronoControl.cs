using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChronoControl : MonoBehaviour
{
    [Range(0, 100)]
    public float timeScale = 1f;
    
    void OnGUI()
    {
        if (Time.timeScale != timeScale)
            Time.timeScale = timeScale;
    }
}
