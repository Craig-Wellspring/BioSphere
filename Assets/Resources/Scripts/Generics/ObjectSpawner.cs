using UnityEngine;

public class ObjectSpawner : AdvancedMonoBehaviour
{
    public GameObject SpawnObject(GameObject _objectToSpawn, EnergyData _subtractFromEData = null, float _energyEndowed = 0, float _percentReturnToSource = 0f, Transform _parent = null, bool _randomYRotation = false, float _randomSpawnArea = 0)
    {
        // Spawn Object
        GameObject newObject = (GameObject)Instantiate(_objectToSpawn, RandomGroundPos(transform.root, _randomSpawnArea), GravityOrientedRotation(), _parent);
        newObject.name = _objectToSpawn.name;

        // Random Rotation
        if (_randomYRotation)
            newObject.transform.RotateAround(newObject.transform.position, newObject.transform.up, Random.Range(1f, 360f));

        // Allocate energy
        if (_energyEndowed > 0)
        {
            float energyEndowed = _energyEndowed;
            // Return energy to source if applicable
            if (_percentReturnToSource > 0)
            {
                float energyReturned = _energyEndowed * (_percentReturnToSource / 100);
                energyEndowed -= energyReturned;
                Servius.Server.GetComponent<GlobalLifeSource>().lifeEnergyPool += energyReturned;
            }

            // Tranfer energy to new object
            NutritionalValue newObjectNV = newObject.GetComponentInChildren<NutritionalValue>(true);
            if (energyEndowed > newObjectNV.nutritionalValue)
            {
                EnergyData newObjectEData = newObject.GetComponentInChildren<EnergyData>(true);
                newObjectEData.energyReserve = energyEndowed - newObjectNV.nutritionalValue;
                newObjectEData.SurplusCheck();
            }
            else
                newObjectNV.nutritionalValue = energyEndowed;


            // Consume energy from selected spawner
            if (_subtractFromEData)
                return _subtractFromEData.SpendEnergy(energyEndowed) ? newObject : null;
            else
                return newObject;
        }
        else
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
        Debug.Log("DrawSphere Origin: " + originSpherePoint + ", Destination: " + destinationSpherePoint + ", Distance: " + (destinationSpherePoint - originSpherePoint).magnitude);
    }
}
