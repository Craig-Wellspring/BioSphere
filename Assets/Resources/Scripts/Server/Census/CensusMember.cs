using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CensusMember : MonoBehaviour
{
    private bool playMode = true;
    private void OnApplicationQuit()
    {
        playMode = false;
    }

    void Start()
    {
        string name = gameObject.name.Replace("(Clone)", "");
        CensusMaster.Census.PopulationIncrease(name);
    }

    void OnDisable()
    {
        if (playMode)
            CensusMaster.Census.PopulationDecrease(name);
    }
}
