using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChronoControl : MonoBehaviour
{
    [Range(0, 10)]
    public float timeScale = 1;
    
    void OnGUI()
    {
        if (Time.timeScale != timeScale)
            Time.timeScale = timeScale;
    }
}
