using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class LifeGuardian : ObjectSpawner
{
    [Space(10)]
    public GameObject seedToPlant;
    public float maxEnergyPlanted = 500f;
    [Range(1, 100)]
    public int plantingArea = 2;
    [Range(1, 100)]
    public int roamingArea = 30;
    public int pathingSpread = 5000;

    Animator guardianBrain;
    AIPath pathing;
    GlobalLifeSource lifeSource;

    bool playMode = true;
    void OnApplicationQuit()
    {
        playMode = false;
    }
    void OnDisable()
    {
        if (playMode)
            PlayerSoul.Cam.lifeGuardian = null;
    }
    void OnEnable()
    {
        PlayerSoul.Cam.lifeGuardian = transform.root.GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>();
    }


    void Start()
    {
        guardianBrain = GetComponent<Animator>();
        pathing = transform.root.GetComponent<AIPathAlignedToSurface>();
        lifeSource = Servius.Server.GetComponent<GlobalLifeSource>();

        //Spawn new Guardian if destroyed
        GetComponent<OnDestroyEvent>().BeingDestroyed += lifeSource.SpawnMeteor;
    }

    void Update()
    {
        guardianBrain.SetFloat("DestinationDistance", pathing.remainingDistance);
    }


    //Plant Seedgrass
    public void PlantSeedFromSource()
    {
        float _energyToPlant = (lifeSource.lifeEnergyPool > maxEnergyPlanted + lifeSource.minimumEnergyReserve) ? maxEnergyPlanted : lifeSource.lifeEnergyPool - lifeSource.minimumEnergyReserve;
        if (_energyToPlant > 0)
        {
            SpawnObject(seedToPlant, null, _energyToPlant, 0, null, true, plantingArea);
            lifeSource.lifeEnergyPool -= _energyToPlant;
        }
    }
}
