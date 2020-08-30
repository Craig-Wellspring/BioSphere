using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : AdvancedMonoBehaviour
{
    //// Spawn Object \\\\
    public GameObject SpawnObject(GameObject _objectToSpawn, float _spawnAreaSize, bool _randomYRotation, Transform _parent)
    {
        GameObject newObject = (GameObject)Instantiate(_objectToSpawn, FindGroundPos(_spawnAreaSize), GravityUpRotation(), _parent);
        newObject.name = _objectToSpawn.name;

        //ResetTransform(transform, true);

        //Random Rotation
        if (_randomYRotation)
            newObject.transform.RotateAround(newObject.transform.position, newObject.transform.up, Random.Range(1f, 360f));

        return newObject;
    }

    //// Spawn Object and Add Energy \\\\
    public GameObject SpawnObject(GameObject _objectToSpawn, float _spawnAreaSize, bool _randomYRotation, Transform _parent, float _imbuedEnergy, EnergyData _sourceEData)
    {
        GameObject newObject = (GameObject)Instantiate(_objectToSpawn, FindGroundPos(_spawnAreaSize), GravityUpRotation(), _parent);
        newObject.name = _objectToSpawn.name;

        //ResetTransform(transform, true);

        //Random Rotation
        if (_randomYRotation)
            newObject.transform.RotateAround(newObject.transform.position, newObject.transform.up, Random.Range(1f, 360f));

        //Imbue energy
        EnergyData newObjectEData = newObject.GetComponentInChildren<EnergyData>();
        if (_imbuedEnergy > newObjectEData.nutritionalValue)
            newObjectEData.energyReserve = _imbuedEnergy - newObjectEData.nutritionalValue;
        else
            newObjectEData.nutritionalValue = _imbuedEnergy;

        _sourceEData?.SpendEnergy(_imbuedEnergy);

        return newObject;
    }



    //Get desired spawn position
    private Vector3 FindGroundPos(float _radius)
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
            //spawnCloud.transform.localPosition = Vector3.zero;
            //spawnCloud.transform.localScale = Vector3.one;
            //ResetTransform(transform, true);
            //StartCoroutine(ResetTransform());

            return pos;
        }
        else
            return PointOnTerrainUnderPosition(transform.position);
    }


    IEnumerator ResetTransform()
    {
        yield return 0;
        ResetTransform(transform, true);
    }



    //// Debug \\\\
    [Header("Debug")]
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
