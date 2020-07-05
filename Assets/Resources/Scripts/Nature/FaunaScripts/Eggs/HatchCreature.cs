using UnityEngine;

public class HatchCreature : MonoBehaviour
{
    public GameObject creatureToHatch;

    
    private void SpawnCreature()
    {
        //Spawn baby creature
        Quaternion newRot = Quaternion.FromToRotation(transform.root.up, (transform.root.position - Vector3.zero).normalized) * transform.root.rotation;
        GameObject newBaby = Instantiate(creatureToHatch, PointOnTerrainUnderPosition(), newRot);
        newBaby.name = creatureToHatch.name;


        //Allocate Energy
        FoodData fData = GetComponentInChildren<FoodData>();
        newBaby.GetComponentInChildren<Metabolism>().storedEnergy = fData.nutritionalValue;
        fData.nutritionalValue = 0;
    }


    private void CrackShell()
    {
        Destroy(transform.root.gameObject);
    }


    private Vector3 PointOnTerrainUnderPosition()
    {
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(transform.position + ((transform.position - Vector3.zero).normalized), -(transform.position - Vector3.zero).normalized, out hit, 2000, LayerMask.GetMask("Terrain")))
        {
            if (hit.collider.CompareTag("Ground"))
                return hit.point;
            else
                return Vector3.zero;
        }
        else
            return Vector3.zero;
    }
}
