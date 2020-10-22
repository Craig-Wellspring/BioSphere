using UnityEngine;

public class ObjectSpawner : AdvancedMonoBehaviour
{
    public GameObject SpawnObject(GameObject _objectToSpawn, EnergyData _subtractFromEData = null, float _energyEndowed = 0/*, float _percentReturnToSource = 0f*/, Transform _parent = null, bool _randomYRotation = false, float _randomSpawnArea = 0)
    {
        // Calculate energy
        if (_energyEndowed > 0)
        {
            // Consume energy from selected spawner
            if (_subtractFromEData)
                if (!_subtractFromEData.RemoveEnergy(_energyEndowed))
                    Debug.LogWarning("Not enough energy to remove from source", this);


            // Compensate for nutritional value
            FoodData[] newFData = _objectToSpawn.GetComponentsInChildren<FoodData>(true);
            foreach (FoodData fData in newFData)
                _energyEndowed -= fData.nutritionalValue;
        }

        // Spawn Object
        

        GameObject newObject = (GameObject)Instantiate(_objectToSpawn, TerrainUnderPosition(PositionAbove(transform.root, _randomSpawnArea)).position, GravityOrientedRotation(), _parent);
        newObject.name = _objectToSpawn.name;

        // Random Rotation
        if (_randomYRotation)
            newObject.transform.RotateAround(newObject.transform.position, newObject.transform.up, Random.Range(1f, 360f));

        // Transfer energy to new object
        if (_energyEndowed > 0)
            newObject.GetComponentInChildren<EnergyData>(true)?.AddEnergy(_energyEndowed);
        else if (_energyEndowed < 0)
            Debug.LogWarning("Tried to endow negative energy to object", this);


        return newObject;
    }



    public void DrawDebug()
    {
        // Draw sphere at origin location
        Vector3 originSpherePoint = PositionAbove(transform);
        GameObject originSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        originSphere.GetComponent<MeshRenderer>().material.color = Color.red;
        originSphere.transform.position = originSpherePoint;
        originSphere.transform.localScale *= 0.2f;
        Destroy(originSphere, 10);

        // Draw sphere at ground position
        Vector3 destinationSpherePoint = TerrainUnderPosition(PositionAbove(transform)).position;
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
