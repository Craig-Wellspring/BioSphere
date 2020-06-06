using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatchCreature : MonoBehaviour
{
    public GameObject creatureToHatch;
    public bool canHatch = false;


    public void Hatch()
    {
        CrackShell();
        SpawnCreature(creatureToHatch);
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

    private void SpawnCreature(GameObject newCreature)
    {
        GameObject tmp = Instantiate(newCreature, transform.position, newCreature.transform.rotation);
        tmp.name = newCreature.name;
    }
}
