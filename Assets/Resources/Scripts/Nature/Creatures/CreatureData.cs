using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureData : MonoBehaviour
{
    [Tooltip("Total Stored Energy Value")]
    public float energyUnits;

    [Header("Lifetime Stats")]
    public List<string> lifetimeDiet;
    
    [Header("Relationships")]
    public List<string> predatorList;
    public List<string> preyList;


}
