using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureData : MonoBehaviour
{
    [Tooltip("Total Stored Energy Value")]
    public float energyUnits;


    public List<string> predatorList;
    public List<string> preyList;

    public List<string> lifetimeDiet;

}
