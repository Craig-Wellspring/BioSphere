using UnityEngine;

[RequireComponent(typeof(OnDestroyEvent))]
public class NutritionalValue : MonoBehaviour
{
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
    private void ReturnEnergyToReserve()
    {
        if (nutritionalValue > 0)
        {
            Servius.Server.GetComponent<GlobalLifeSource>().lifeEnergyPool += nutritionalValue;
            nutritionalValue = 0;
        }
    }


    public void AddValue(float _amount)
    {
        nutritionalValue += _amount;
    }

    public bool RemoveValue(float _amount)
    {
        if (nutritionalValue >= _amount)
        {
            nutritionalValue -= _amount;
            return true;
        }
        else return false;
    }
}
