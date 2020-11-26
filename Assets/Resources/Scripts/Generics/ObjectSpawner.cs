using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Spawn Collision")]
    [Tooltip("Find a new spawn position if point collides with objects in these layers."), SerializeField]
    LayerMask bumpLayers;
    [Tooltip("Area to check for other objects."), SerializeField]
    float bumpRadius = 2;
    [SerializeField]
    bool drawBumpSphere = false;

    public GameObject SpawnObject(GameObject _objectToSpawn, EnergyData _subtractFromEData = null, float _energyEndowed = 0, Transform _parent = null, bool _randomYRotation = false, float _randomSpawnArea = 0, bool _aboveSeaLevel = false)
    {
        // Consume energy from spawner
        if (_energyEndowed > 0 && _subtractFromEData)
            if (!_subtractFromEData.RemoveEnergy(_energyEndowed))
                Debug.LogWarning("Not enough energy to remove from source eData", this);

        // Spawn Object
        GameObject newObject = (GameObject)Instantiate(_objectToSpawn, FindSpawnPos(_randomSpawnArea, _aboveSeaLevel), Quaternion.identity, _parent);

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

    protected Vector3 FindSpawnPos(float _spawnRadius = 0, bool _aboveSeaLevel = true, int _maxTries = 200)
    {
        int tries = 0;
        float newRadius = _spawnRadius;

        // Get a new point
        Vector3 spawnPos = transform.root.position;
        if (_spawnRadius > 0)
            spawnPos = _aboveSeaLevel ? UtilityFunctions.FindDryLand(transform.root, _spawnRadius) : UtilityFunctions.GroundBelowPosition(UtilityFunctions.PositionAbove(transform.root, _spawnRadius)).position;

        // Get a list of other objects in the area
        List<Collider> objectsInArea = Physics.OverlapSphere(spawnPos, bumpRadius, bumpLayers).ToList();

        while (objectsInArea.Count > 0)
        {
            // Find a new point and check for other objects
            spawnPos = _aboveSeaLevel ? UtilityFunctions.FindDryLand(transform.root, newRadius) : UtilityFunctions.GroundBelowPosition(UtilityFunctions.PositionAbove(transform.root, newRadius)).position;

            objectsInArea = Physics.OverlapSphere(spawnPos, bumpRadius, bumpLayers).ToList();

            // Increase search radius
            newRadius += 0.5f;
            tries++;
            if (tries > _maxTries)
            {
                Debug.LogWarning("Spawn area crowded.", this.transform);
                return Vector3.zero;
            }
        }

        return spawnPos;
    }


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
        GameObject originSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        originSphere.GetComponent<MeshRenderer>().material.color = Color.red;
        originSphere.transform.position = originSpherePoint;
        originSphere.transform.localScale *= 0.2f;
        Destroy(originSphere, 10);

        // Draw sphere at ground position
        Vector3 destinationSpherePoint = UtilityFunctions.GroundBelowPosition(UtilityFunctions.PositionAbove(transform)).position;
        GameObject destinationSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        destinationSphere.GetComponent<MeshRenderer>().material.color = Color.green;
        destinationSphere.transform.position = destinationSpherePoint;
        destinationSphere.transform.localScale *= 0.2f;
        Destroy(destinationSphere, 10);

        // Draw line from origin to destination
        Debug.DrawLine(originSpherePoint, destinationSpherePoint, Color.yellow, 10);

        // Log positions
        Debug.Log("DrawSphere Origin: " + originSpherePoint + ", Destination: " + destinationSpherePoint + ", Distance: " + (destinationSpherePoint - originSpherePoint).magnitude);
    }
}
