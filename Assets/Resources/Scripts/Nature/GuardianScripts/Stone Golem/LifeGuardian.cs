using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class LifeGuardian : ObjectSpawner
{
    [Space(10)]
    public GameObject seedToPlant;
    public float maxEnergyPlanted = 500f;
    public float minimumGlobalEnergy;
    [Range(1, 100)]
    public int plantingArea = 2;
    [Range(1, 100)]
    public int roamingArea = 30;
    public int pathingSpread = 5000;

    [Space(10)]
    [SerializeField] bool manualPlantSeed = false;

    Animator guardianBrain;
    AIPath pathing;
    private bool playMode = true;
    private void OnApplicationQuit()
    {
        playMode = false;
    }

    private void OnEnable()
    {
        PlayerSoul.Cam.lifeGuardian = transform.root.GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>();
    }

    private void OnDisable()
    {
        if (playMode)
        {
            PlayerSoul.Cam.lifeGuardian = null;
        }
    }

    private void Start()
    {
        guardianBrain = GetComponent<Animator>();
        pathing = transform.root.GetComponent<AIPathAlignedToSurface>();

        //Spawn new Guardian if destroyed
        GetComponent<OnDestroyEvent>().BeingDestroyed += Servius.Server.GetComponent<GlobalLifeSource>().SpawnMeteor;
    }

    private void Update()
    {
        guardianBrain.SetFloat("DestinationDistance", pathing.remainingDistance);
    }


    //Plant Seedgrass
    public void PlantSeed()
    {
        float _energyToPlant = (Servius.Server.GetComponent<GlobalLifeSource>().lifeEnergyPool > maxEnergyPlanted + minimumGlobalEnergy) ? maxEnergyPlanted : Servius.Server.GetComponent<GlobalLifeSource>().lifeEnergyPool - minimumGlobalEnergy;
        if (_energyToPlant > 0)
        {
            SpawnObject(seedToPlant, plantingArea, true, null, _energyToPlant, null);
            Servius.Server.GetComponent<GlobalLifeSource>().lifeEnergyPool -= _energyToPlant;
        }
    }

    //Manually plant a seed
    private void OnValidate()
    {
        if (manualPlantSeed)
        {
            PlantSeed();
            manualPlantSeed = false;
        }
    }
}
