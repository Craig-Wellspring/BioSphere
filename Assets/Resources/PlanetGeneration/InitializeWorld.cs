using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeWorld : MonoBehaviour
{
    public GameObject planetPrefab;

    void Start()
    {
        GameObject newWorld = Instantiate(planetPrefab);
        newWorld.name = planetPrefab.name;
        newWorld.GetComponent<Planet>().GeneratePlanet();
    }
}
