using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    #region Singleton
    public static CloudSpawner Spawner { get; private set; }
    void Awake()
    {
        if (Spawner == null)
            Spawner = this;
        else
            Destroy(gameObject);
    }
    #endregion


    // Settings
    public int numberOfClouds = 100;

    [Header("Settings")]
    [SerializeField] Vector2 spawnRadiusMinxXMaxY;
    [SerializeField] Vector2 cloudSizeMinXMaxY;
    [SerializeField] List<GameObject> cloudTypes;




    // Spawn initial clouds
    void Start()
    {
        for(int i = 0; i < numberOfClouds; i++)
            SpawnCloud(false);   
    }


    public void SpawnCloud(bool _increaseCount)
    {
        Vector3 spawnPos = Random.onUnitSphere * Random.Range(spawnRadiusMinxXMaxY.x, spawnRadiusMinxXMaxY.y);

        GameObject newCloud = Instantiate(cloudTypes[Random.Range(0,cloudTypes.Count)], spawnPos, Quaternion.identity, transform);
        newCloud.name = "Cloud";
        newCloud.transform.rotation = Quaternion.FromToRotation(newCloud.transform.up, -UtilityFunctions.GravityVector(newCloud.transform.position)) * newCloud.transform.rotation;
        newCloud.transform.localScale = new Vector3(Random.Range(cloudSizeMinXMaxY.x, cloudSizeMinXMaxY.y), Random.Range(cloudSizeMinXMaxY.x, cloudSizeMinXMaxY.y), Random.Range(cloudSizeMinXMaxY.x, cloudSizeMinXMaxY.y));

        if (_increaseCount)
            numberOfClouds += 1;
    }
}
