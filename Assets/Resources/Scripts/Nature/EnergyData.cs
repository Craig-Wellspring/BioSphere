using System;
using UnityEngine;

[RequireComponent(typeof(OnDestroyEvent))]
public class EnergyData : MonoBehaviour
{
    #region Settings
    [Header("Energy Values")]
    [Tooltip("On Destroy, entity will return remaining Energy Reserve and Nutritional Value to the Global Energy Reserve")]
    public float energyReserve;
    public float nutritionalValue;

    [Header("Settings")]
    public float chewRateModifier = 1f;
    [Tooltip("Destroy the root parent entity when eaten")]
    public bool destroyRoot = true;
    #endregion

    // Cache
    Evolution evolution;
    Ovary ovary;
    

    //// Return Energy to Global Energy Reserve when Destroyed \\\\
    private void Start()
    {
        GetComponent<OnDestroyEvent>().BeingDestroyed += ReturnEnergyToReserve;

        evolution = GetComponent<Evolution>();
        ovary = GetComponent<Ovary>();
    }

    private void ReturnEnergyToReserve()
    {
        if (energyReserve > 0 || nutritionalValue > 0)
        {
            Servius.Server.GetComponent<GlobalLifeSource>().lifeEnergyPool += energyReserve + nutritionalValue;
            energyReserve = 0;
            nutritionalValue = 0;
        }
    }



    //// Increase and Decrease Energy Reserve Pool \\\\
    //public event Action EnergySpent;
    //public event Action EnergyGained;

    public void SpendEnergy(float _amount)
    {
        energyReserve -= _amount;

        SurplusCheck();
        //EnergySpent?.Invoke();
    }
    public void GainEnergy(float _amount)
    {
        energyReserve += _amount;

        SurplusCheck();
        //EnergyGained?.Invoke();
    }
    

    public event System.Action EnergyAboveSurplus;
    public event System.Action EnergyBelowSurplus;
    void SurplusCheck()
    {
        if (evolution?.evolutionCost <= energyReserve && ovary?.reproductionCost <= energyReserve)
        {
            //Tell UI and AI there is an energy surplus
            EnergyAboveSurplus?.Invoke();
        }
        else EnergyBelowSurplus?.Invoke();
    }
}
