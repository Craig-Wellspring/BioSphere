using UnityEngine;
using System.Collections;

public class FoodData : MonoBehaviour
{
    [Header("State")]
    [Tooltip("Current / Default")]
    public Vector2 nutritionalValue;

    [Header("Settings")]
    [Tooltip("Higher is faster. 1 is default.")]
    public float chewRateModifier = 1f;
    public enum ConsumptionType { DestroyObject, DestroyRoot, DisableObject, DisableRoot }
    public ConsumptionType consumptionType;

    // On Spawn transfer energy from eData to NV
    void OnEnable()
    {
        if (transform.root.TryGetComponent<EnergyData>(out EnergyData eData))
        {
            float availableEnergy = Mathf.Min(nutritionalValue.y, eData.energyReserve);

            if (eData.RemoveEnergy(availableEnergy))
                AddNV(availableEnergy);
        }
    }


    // Return Energy to Global Energy Reserve when Destroyed
    void OnDisable()
    {
        if (Servius.Server != null)
            ReturnEnergyToReserve(nutritionalValue.x);
    }

    public void ReturnEnergyToReserve(float _amount)
    {
        if (RemoveNV(_amount))
            Servius.Server.GetComponent<GlobalLifeSource>().energyReserve += _amount;
        else Debug.LogError("Returned more nutritional value than available.", this);
    }


    // Add and Remove nutritional value
    public void AddNV(float _amount)
    {
        nutritionalValue.x += _amount;
    }

    public bool RemoveNV(float _amount)
    {
        if (nutritionalValue.x >= _amount)
        {
            nutritionalValue.x -= _amount;
            return true;
        }
        else return false;
    }
}
