using UnityEngine;

public class FoodData : MonoBehaviour
{
    [Header("State")]
    public float nutritionalValue;

    [Header("Settings")]
    [Tooltip("Higher is faster. 1 is default.")]
    public float chewRateModifier = 1f;
    [Tooltip("Destroy the root parent entity when eaten")]
    public bool destroyRoot = true;


    // Return Energy to Global Energy Reserve when Destroyed
    void OnDisable()
    {
        if (Servius.Server != null)
            ReturnEnergyToReserve(nutritionalValue);
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
        nutritionalValue += _amount;
    }

    public bool RemoveNV(float _amount)
    {
        if (nutritionalValue >= _amount)
        {
            nutritionalValue -= _amount;
            return true;
        }
        else return false;
    }
}
