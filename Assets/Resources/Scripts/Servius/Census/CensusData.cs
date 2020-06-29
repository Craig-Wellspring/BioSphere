using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CensusData
{
    [HideInInspector] public string speciesName;
    public int populationSize;

    public CensusData(string name, int pop)
    {
        this.speciesName = name;
        this.populationSize = pop;
    }
}
