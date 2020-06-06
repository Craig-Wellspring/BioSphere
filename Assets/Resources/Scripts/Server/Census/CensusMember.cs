using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CensusMember : MonoBehaviour
{
    void Start()
    {
        string name = gameObject.name.Replace("(Clone)", "");
        CensusMaster.Census.PopulationIncrease(name);
    }

    void OnDisable()
    {
        if (Application.isPlaying)
            CensusMaster.Census.PopulationDecrease(name);
    }
}
