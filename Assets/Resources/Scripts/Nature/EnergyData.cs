using System;
using UnityEngine;

[RequireComponent(typeof(OnDestroyEvent))]
public class EnergyData : MonoBehaviour
{
    public float energyReserve = 0;


    // Cache
    NutritionalValue nv;
    [HideInInspector] public bool energySurplus = false;
    [HideInInspector] public float surplusThreshold = 0;


    private void Start()
    {
        GetComponent<OnDestroyEvent>().BeingDestroyed += ReturnEnergyToReserve;

        // Initialize
        nv = GetComponent<NutritionalValue>();

        SurplusCheck();
    }


    // Return Energy to Global Energy Reserve when Destroyed
    private void ReturnEnergyToReserve()
    {
        if (energyReserve > 0)
        {
            Servius.Server.GetComponent<GlobalLifeSource>().lifeEnergyPool += energyReserve;
            energyReserve = 0;
        }
    }



    //// Increase and Decrease Energy Reserve Pool \\\\
    public void GainEnergy(float _energyGained)
    {
        energyReserve += _energyGained;

        SurplusCheck();
    }

    public bool SpendEnergy(float _energySpent, bool _makeUpWithNV = false)
    {
        if (energyReserve >= _energySpent)
        {
            energyReserve -= _energySpent;
            SurplusCheck();
            return true;
        }
        else if (_makeUpWithNV && (energyReserve + nv.nutritionalValue > _energySpent))
        {
            nv.nutritionalValue -= _energySpent - energyReserve;
            energyReserve = 0;

            // Catch energy overuse
            if (nv.nutritionalValue < 0 || nv.nutritionalValue < 0)
                Debug.LogError("Spent more energy than available", this);

            return true;
        }
        else return false;
    }


    // Check for energy surplus, set bool and trigger events
    public event System.Action EnergySurplusChange;
    public void SurplusCheck()
    {
        bool _energySurplus = energyReserve >= surplusThreshold ? true : false;
        if (energySurplus != _energySurplus)
        {
            energySurplus = _energySurplus;
            EnergySurplusChange?.Invoke();
        }
    }
}
