using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : AdvancedMonoBehaviour
{
    protected float spawnRadius = 3f;

    //// Spawn Object \\\\
    public GameObject SpawnObject(GameObject _objectToSpawn, float _spawnAreaSize, bool _randomYRotation, Transform _parent)
    {
        GameObject newObject = (GameObject)Instantiate(_objectToSpawn, RandomGroundPos(transform.root, _spawnAreaSize), GravityOrientedRotation(), _parent);
        newObject.name = _objectToSpawn.name;

        // Random Rotation
        if (_randomYRotation)
            newObject.transform.RotateAround(newObject.transform.position, newObject.transform.up, Random.Range(1f, 360f));

        return newObject;
    }

    //// Spawn Object and Add Energy \\\\
    public GameObject SpawnObject(GameObject _objectToSpawn, float _spawnAreaSize, bool _randomYRotation, Transform _parent, float _imbuedEnergy, EnergyData _sourceEData)
    {
        GameObject newObject = (GameObject)Instantiate(_objectToSpawn, RandomGroundPos(transform.root, _spawnAreaSize), GravityOrientedRotation(), _parent);
        newObject.name = _objectToSpawn.name;

        // Random Rotation
        if (_randomYRotation)
            newObject.transform.RotateAround(newObject.transform.position, newObject.transform.up, Random.Range(1f, 360f));

        // Imbue energy
        EnergyData newObjectEData = newObject.GetComponentInChildren<EnergyData>(true);
        if (_imbuedEnergy > newObjectEData.nutritionalValue)
            newObjectEData.energyReserve = _imbuedEnergy - newObjectEData.nutritionalValue;
        else
            newObjectEData.nutritionalValue = _imbuedEnergy;

        _sourceEData?.SpendEnergy(_imbuedEnergy);

        return newObject;
    }



    public void DrawDebug()
    {
        // Draw sphere at origin location
        Vector3 originSpherePoint = RandomLocalPos(transform, 0);
        GameObject originSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        originSphere.GetComponent<MeshRenderer>().material.color = Color.red;
        originSphere.transform.position = originSpherePoint;
        originSphere.transform.localScale *= 0.2f;
        Destroy(originSphere, 10);

        // Draw sphere at ground position
        Vector3 destinationSpherePoint = PointOnTerrainUnderPosition(RandomLocalPos(transform, 0));
        GameObject destinationSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        destinationSphere.GetComponent<MeshRenderer>().material.color = Color.green;
        destinationSphere.transform.position = destinationSpherePoint;
        destinationSphere.transform.localScale *= 0.2f;
        Destroy(destinationSphere, 10);

        // Draw line from origin to destination
        Debug.DrawLine(originSpherePoint, destinationSpherePoint, Color.yellow, 10);
        
        // Log positions
        Debug.Log("DrawSphere Origin: " + originSpherePoint + ", Destination: " + destinationSpherePoint);
    }
}
