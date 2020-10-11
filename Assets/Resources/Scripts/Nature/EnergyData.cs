using UnityEngine;
using System;

public class EnergyData : MonoBehaviour
{
    [Header("Current")]
    public float energyReserve = 0;


    // Return Energy to Global Energy Reserve when Destroyed
    void OnDisable()
    {
        if (Servius.Server != null)
            ReturnEnergyToReserve(energyReserve);
    }

    public void ReturnEnergyToReserve(float _amount)
    {
        if (RemoveEnergy(_amount))
            Servius.Server.GetComponent<GlobalLifeSource>().energyReserve += _amount;
        else Debug.LogError("Returned more energy than available.", this);
    }


    //// Increase and Decrease Energy Reserve Pool \\\\
    public Action EnergyAdded;
    public void AddEnergy(float _energyAdded)
    {
        energyReserve += _energyAdded;

        EnergyAdded?.Invoke();
        // - morphology.SurplusCheck
    }

    public Action EnergyRemoved;
    public bool RemoveEnergy(float _energyRemoved, bool _makeUpWithNV = false)
    {
        if (energyReserve >= _energyRemoved)
        {
            energyReserve -= _energyRemoved;

            EnergyRemoved?.Invoke();
            // - morphology.SurplusCheck

            // Catch energy overuse
            if (energyReserve < 0)
                Debug.LogError("Removed more energy than available", this);

            return true;
        }
        else if (_makeUpWithNV)
        {
            FoodData fData = GetComponent<FoodData>();
            if (energyReserve + fData.nutritionalValue >= _energyRemoved)
            {
                fData.nutritionalValue -= _energyRemoved - energyReserve;
                energyReserve = 0;

                // Catch energy overuse
                if (fData.nutritionalValue < 0)
                    Debug.LogError("Removed more energy than available", this);

                return true;
            }
            else return false;
        }
        else return false;
    }
}
