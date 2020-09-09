using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    void OnDisable()
    {
        CloudSpawner.Spawner.numberOfClouds -= 1;
    }
}
