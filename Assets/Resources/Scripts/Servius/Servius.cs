using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Servius : MonoBehaviour
{
    #region Singleton
    public static Servius Server { get; private set; }
    private void Awake()
    {
        if (Server == null)
            Server = this;
        else
            Destroy(gameObject); //should never happen
    }
    #endregion
    
    [Range(0, 10)]
    public float timeScale = 1;
    
    void OnGUI()
    {
        if (Time.timeScale != timeScale)
            Time.timeScale = timeScale;
    }
}
