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
    

    //// Return Energy to Global Energy Reserve when Destroyed \\\\
    private void Start()
    {
        GetComponent<OnDestroyEvent>().BeingDestroyed += ReturnEnergyToReserve;
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
    public event Action EnergySpent;
    public event Action EnergyGained;

    public void SpendEnergy(float _amount)
    {
        energyReserve -= _amount;

        EnergySpent?.Invoke();
    }
    public void GainEnergy(float _amount)
    {
        energyReserve += _amount;

        EnergyGained?.Invoke();
    }
}
