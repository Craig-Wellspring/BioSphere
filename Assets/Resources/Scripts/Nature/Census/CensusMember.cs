using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CensusMember : MonoBehaviour
{
    void OnEnable()
    {
        string name = gameObject.name.Replace("(Clone)", "");
        CensusMaster.Census.PopulationIncrease(name);
    }

    void OnDisable()
    {
        CensusMaster.Census.PopulationDecrease(name);
    }
}
