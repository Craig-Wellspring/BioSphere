using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(EnergyData))]
public class TakeEnergyFromSource : MonoBehaviour
{
    [SerializeField] float maxEnergyTaken = 500;


    void Start()
    {
        GetComponent<EnergyData>().TakeEnergyFromSource(maxEnergyTaken);
    }
}
