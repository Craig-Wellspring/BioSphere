using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : AdvancedMonoBehaviour
{
    //// Spawn Object \\\\
    public GameObject SpawnObject(GameObject _objectToSpawn, float _spawnAreaSize, bool _randomYRotation, Transform _parent)
    {
        GameObject newObject = (GameObject)Instantiate(_objectToSpawn, GetRandomLocation(_spawnAreaSize), GravityUp(), _parent);
        newObject.name = _objectToSpawn.name;

        //Random Rotation
        if (_randomYRotation)
            newObject.transform.RotateAround(newObject.transform.position, newObject.transform.up, Random.Range(1f, 360f));

        return newObject;
    }

    //// Spawn Object and Add Energy \\\\
    public GameObject SpawnObject(GameObject _objectToSpawn, float _spawnAreaSize, bool _randomYRotation, Transform _parent, float _imbuedEnergy, EnergyData _sourceEData)
    {
        GameObject newObject = (GameObject)Instantiate(_objectToSpawn, GetRandomLocation(_spawnAreaSize), GravityUp(), _parent);
        newObject.name = _objectToSpawn.name;

        //Random Rotation
        if (_randomYRotation)
            newObject.transform.RotateAround(newObject.transform.position, newObject.transform.up, Random.Range(1f, 360f));

        //Imbue energy
        EnergyData newObjectEData = newObject.GetComponentInChildren<EnergyData>();
        if (_imbuedEnergy > newObjectEData.nutritionalValue)
            newObjectEData.energyReserve = _imbuedEnergy - newObjectEData.nutritionalValue;
        else
            newObjectEData.nutritionalValue = _imbuedEnergy;

        if (_sourceEData != null)
            _sourceEData.SpendEnergy(_imbuedEnergy);

        return newObject;
    }



    //Get desired spawn position
    private Vector3 GetRandomLocation(float _radius)
    {
        if (_radius > 0)
        {
            BoxCollider spawnCloud = gameObject.AddComponent<BoxCollider>() as BoxCollider;
            spawnCloud.isTrigger = true;
            spawnCloud.transform.localPosition = new Vector3(0f, 50f, 0f);
            spawnCloud.transform.localScale = new Vector3(_radius, 0.001f, _radius);

            Vector3 seedCloudPos = GetRandomPointOnCol(spawnCloud);
            Vector3 pos = PointOnTerrainUnderPosition(seedCloudPos);

            while (pos == Vector3.zero || pos == null)
            {
                seedCloudPos = GetRandomPointOnCol(spawnCloud);
                pos = PointOnTerrainUnderPosition(seedCloudPos);
            }

            Destroy(spawnCloud);
            //ResetTransform(transform, true);

            return pos;
        }
        else
            return PointOnTerrainUnderPosition(transform.position);
    }




    //// Debug \\\\
    [SerializeField] private bool drawDebugSphere = false;

    private void OnValidate()
    {
        //Debug trigger
        if (drawDebugSphere)
        {
            DrawDebugSphere();
            drawDebugSphere = false;
        }
    }
    private void DrawDebugSphere()
    {
        //Find potential spawning location
        Vector3 drawSpherePoint = PointOnTerrainUnderPosition(transform.position);

        Debug.Log("DrawSphere FromPos: " + transform.position + ", ToPos: " + drawSpherePoint);

        //Draw Sphere at potential spawning location
        Debug.DrawLine(transform.position, drawSpherePoint, Color.green, 10);
        GameObject debugSphere = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere), drawSpherePoint, transform.rotation);
        debugSphere.transform.localScale *= 0.2f;
        Destroy(debugSphere, 10);
    }
}
