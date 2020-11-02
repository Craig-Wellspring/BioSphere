using UnityEngine;
using System;

public class EnergyData : MonoBehaviour
{
    [Header("State")]
    public float energyReserve = 0;


    // Return Energy to Global Energy Reserve when Destroyed
    void OnDisable()
    {
        if (Servius.Server != null && energyReserve > 0)
            ReturnEnergyToReserve(energyReserve);
    }

    public void ReturnEnergyToReserve(float _amount)
    {
        if (RemoveEnergy(_amount))
        {
            GlobalLifeSource lifeSource = Servius.Server.GetComponent<GlobalLifeSource>();
            lifeSource.energyReserve += _amount;
            
            if (lifeSource.logEnergyReturn)
                Debug.Log("Returned " + _amount + " Energy to Source.", this);
        }
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
            if (energyReserve + fData.nutritionalValue.x >= _energyRemoved)
            {
                fData.nutritionalValue.x -= _energyRemoved - energyReserve;
                energyReserve = 0;

                // Catch energy overuse
                if (fData.nutritionalValue.x < 0)
                    Debug.LogError("Removed more energy than available", this);

                return true;
            }
            else return false;
        }
        else return false;
    }
}
