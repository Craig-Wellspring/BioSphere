using UnityEngine;

[RequireComponent(typeof(OnDestroyEvent))]
public class FoodData : MonoBehaviour
{
    [Header("Current")]
    public float nutritionalValue;

    [Header("Settings")]
    [Tooltip("Higher is faster. 1 is default.")]
    public float chewRateModifier = 1f;
    [Tooltip("Destroy the root parent entity when eaten")]
    public bool destroyRoot = true;


    // Return Energy to Global Energy Reserve when Destroyed
    private void Start()
    {
        GetComponent<OnDestroyEvent>().BeingDestroyed += ReturnEnergyToReserve;
    }
    void ReturnEnergyToReserve()
    {
        if (nutritionalValue > 0)
        {
            Servius.Server.GetComponent<GlobalLifeSource>().energyReserve += nutritionalValue;
            nutritionalValue = 0;
        }
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
