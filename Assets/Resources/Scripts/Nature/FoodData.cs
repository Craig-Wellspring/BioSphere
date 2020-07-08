using System;
using System.Collections;
using UnityEngine;


public class FoodData : MonoBehaviour
{
    #region Settings
    [Header("Values")]
    [Tooltip("On Destroy, entity will spawn Seedgrass with remaining Energy Stored and Nutritional Value")]
    public float energyStored;
    public float nutritionalValue = 1f;

    [Header("Settings")]
    public float chewRateModifier = 1f;
    [Tooltip("Destroy the root parent entity when eaten")]
    public bool destroyRoot = true;
    #endregion

    #region Internal Variables
    bool isEnabled = true;

    //Events
    public event Action FoodDestroyed;
    #endregion



    private void OnApplicationQuit()
    {
        isEnabled = false;
    }

    private void OnDisable()
    {
        if (isEnabled && FoodDestroyed != null)
            FoodDestroyed();
    }
}
