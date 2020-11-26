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
            ReturnEnergyToSource(energyReserve);
    }

    public void TakeEnergyFromSource(float _amount, bool _surplusQuery = true)
    {
        GlobalLifeSource lifeSource = Servius.Server.GetComponent<GlobalLifeSource>();
        float energyTaken = lifeSource.EnergyAvailable(_amount);

        if (energyTaken > 0)
        {
            lifeSource.energyReserve -= energyTaken;
            AddEnergy(energyTaken);
            
            if (lifeSource.logEnergyTaken)
                Debug.Log(transform.root.name + " took " + _amount + " Energy from Source.", this);
        }
    }

    public void ReturnEnergyToSource(float _amount, bool _surplusQuery = true)
    {
        if (RemoveEnergy(_amount, _surplusQuery))
        {
            GlobalLifeSource lifeSource = Servius.Server.GetComponent<GlobalLifeSource>();
            lifeSource.energyReserve += _amount;

            if (lifeSource.logEnergyReturn)
                Debug.Log(transform.root.name + " returned " + _amount + " Energy to Source.", this);
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
    public bool RemoveEnergy(float _energyRemoved, bool _surplusQuery = true)
    {
        if (energyReserve >= _energyRemoved)
        {
            energyReserve -= _energyRemoved;

            if (_surplusQuery)
                EnergyRemoved?.Invoke();
            // - evolution.SurplusQuery

            // Catch energy overuse
            if (energyReserve < 0)
                Debug.LogError("Removed more energy than available", this);

            return true;
        }
        else return false;
    }
}
