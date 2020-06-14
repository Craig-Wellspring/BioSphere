using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatchCreature : MonoBehaviour
{
    public GameObject creatureToHatch;
    public bool canHatch = false;


    public void Hatch()
    {
        SpawnCreature(creatureToHatch);
        CrackShell();
    }




    private void SpawnCreature(GameObject _newCreature)
    {
        GameObject creatureToSpawn = Instantiate(_newCreature, transform.position, transform.rotation);
        PlanetCore.Core.AlignWithGravity(creatureToSpawn.transform);
        creatureToSpawn.name = _newCreature.name;


        //Allocate Energy
        FoodData fData = GetComponent<FoodData>();
        creatureToSpawn.GetComponentInChildren<CreatureData>().energyUnits = fData.nutritionalValue;
        fData.nutritionalValue = 0;

    }


    private void CrackShell()
    {
        Vector3 currentScale = transform.GetChild(0).transform.localScale;
        GameObject topshell = transform.GetChild(1).gameObject;
        GameObject bottomshell = transform.GetChild(2).gameObject;

        topshell.SetActive(true);
        bottomshell.SetActive(true);
        topshell.transform.localScale = currentScale;
        bottomshell.transform.localScale = currentScale;
        topshell.transform.SetParent(null);
        bottomshell.transform.SetParent(null);
        
        Destroy(this.gameObject);
    }
}
