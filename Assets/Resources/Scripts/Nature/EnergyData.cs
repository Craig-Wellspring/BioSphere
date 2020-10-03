using UnityEngine;

[RequireComponent(typeof(OnDestroyEvent))]
public class EnergyData : MonoBehaviour
{
    [Header("Current")]
    public float energyReserve = 0;
    [Space(10)]
    public bool energySurplus = false;


    CreatureData cData;


    void Start()
    {
        GetComponent<OnDestroyEvent>().BeingDestroyed += ReturnEnergyToReserve;

        cData = GetComponent<CreatureData>();

        if (cData)
            SurplusCheck();
    }


    // Return Energy to Global Energy Reserve when Destroyed
    void ReturnEnergyToReserve()
    {
        if (energyReserve > 0)
        {
            Servius.Server.GetComponent<GlobalLifeSource>().energyReserve += energyReserve;
            energyReserve = 0;
        }
    }



    //// Increase and Decrease Energy Reserve Pool \\\\
    public void AddEnergy(float _energyAdded)
    {
        energyReserve += _energyAdded;

        if (cData)
            SurplusCheck();
    }

    public bool RemoveEnergy(float _energyRemoved, bool _makeUpWithNV = false)
    {
        if (energyReserve >= _energyRemoved)
        {
            energyReserve -= _energyRemoved;

            if (cData)
                SurplusCheck();

            // Catch energy overuse
            if (energyReserve < 0)
                Debug.LogError("Spent more energy than available", this);

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
                    Debug.LogError("Spent more energy than available", this);

                return true;
            }
            else return false;
        }
        else return false;
    }


    // Check for energy surplus, set bool and trigger events
    public event System.Action EnergySurplusChange;
    public bool SurplusCheck()
    {
        bool _energySurplus = energyReserve >= cData.levelUpCost ? true : false;
        if (energySurplus != _energySurplus)
        {
            energySurplus = _energySurplus;
            EnergySurplusChange?.Invoke();
        }
        return _energySurplus;
    }
}
