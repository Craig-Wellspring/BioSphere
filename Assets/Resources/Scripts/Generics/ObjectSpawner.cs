using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Spawn Collision")]
    [Tooltip("Find a new spawn position if point collides with objects in these layers."), SerializeField]
    LayerMask bumpLayers;
    [Tooltip("Area to check for other objects."), SerializeField]
    float bumpRadius = 2;
    [SerializeField]
    bool drawBumpSphere = false;
    [SerializeField] bool debug = false;

    public GameObject SpawnObject(GameObject _objectToSpawn, EnergyData _subtractFromEData = null, float _energyEndowed = 0, Transform _parent = null, bool _randomYRotation = false, float _randomSpawnArea = 0, bool _aboveSeaLevel = false)
    {
        // Consume energy from spawner
        if (_energyEndowed > 0 && _subtractFromEData)
            if (!_subtractFromEData.RemoveEnergy(_energyEndowed))
                Debug.LogWarning("Not enough energy to remove from source eData", this);

        // Find Spawning position
        Vector3 spawnPos = UtilityFunctions.FindNearbyPos(transform.root, _randomSpawnArea, _aboveSeaLevel, bumpRadius, bumpLayers);

        // Spawn Object
        GameObject newObject = (GameObject)Instantiate(_objectToSpawn, spawnPos, Quaternion.identity, _parent);

        // Orient to gravity
        newObject.transform.rotation = UtilityFunctions.GravityOrientedRotation(newObject.transform);

        // Rename
        newObject.name = _objectToSpawn.name;

        // Random Rotation
        if (_randomYRotation)
            newObject.transform.RotateAround(newObject.transform.position, newObject.transform.up, Random.Range(1f, 360f));

        // Transfer energy to new object
        EnergyData _newEData = newObject.GetComponentInChildren<EnergyData>(true);
        if (_energyEndowed > 0)
            _newEData?.AddEnergy(_energyEndowed);
        else if (_energyEndowed < 0)
            Debug.LogWarning("Tried to endow negative energy to object", this);

        // Allocate nutritional value
        foreach (FoodData fData in newObject.GetComponentsInChildren<FoodData>())
            fData.enabled = true;


        return newObject;
    }


    #region Debug
    //// Draw Debug BumpSphere \\\\
    void OnDrawGizmosSelected()
    {
        if (drawBumpSphere)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, bumpRadius);
        }
    }

    public void DrawDebug()
    {
        // Draw sphere at origin location
        Vector3 originSpherePoint = UtilityFunctions.PositionAbove(transform);

        DrawDebugSphere(originSpherePoint, Color.red);


        // Draw sphere at ground position
        Vector3 destinationSpherePoint = UtilityFunctions.GroundBelowPosition(UtilityFunctions.PositionAbove(transform)).position;

        DrawDebugSphere(destinationSpherePoint, Color.green);


        // Log positions
        Debug.Log("DrawSphere Origin: " + originSpherePoint + ", Destination: " + destinationSpherePoint + ", Distance: " + (destinationSpherePoint - originSpherePoint).magnitude);
    }

    public void DrawSpawnDebug(float _radius, bool _aboveSeaLevel)
    {
        // Find spawning location
        Vector3 sphereSpawnPoint = UtilityFunctions.FindNearbyPos(transform.root, _radius, _aboveSeaLevel, bumpRadius, bumpLayers);

        DrawDebugSphere(sphereSpawnPoint, Color.magenta);

        // Log position
        Debug.Log("Debug Spawn Position: " + sphereSpawnPoint);
    }


    GameObject DrawDebugSphere(Vector3 _spawnPos, Color _color)
    {
        // Create sphere
        GameObject debugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        debugSphere.GetComponent<MeshRenderer>().material.color = _color;
        debugSphere.transform.localScale *= 0.3f;

        // Position and orient sphere
        debugSphere.transform.position = _spawnPos;
        debugSphere.transform.rotation = UtilityFunctions.GravityOrientedRotation(debugSphere.transform);

        // Draw indicator line
        Debug.DrawLine(_spawnPos, UtilityFunctions.PositionAbove(debugSphere.transform, 0, 3), _color, 10);

        // Decay
        Destroy(debugSphere, 10);

        return debugSphere;
    }
    #endregion
}
