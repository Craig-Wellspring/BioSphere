using System;
using System.Collections;
using UnityEngine;


public class FoodData : MonoBehaviour
{
    [Header("Values")]
    [Tooltip("On Destroy, entity will spawn Seedgrass with remaining Energy Stored")]
    public float energyStored;
    public float nutritionalValue = 1f;

    [Header("Settings")]
    public float chewRateModifier = 1f;
    [Tooltip("Destroy the root parent entity when eaten")]
    public bool destroyRoot = true;
//ea
}
