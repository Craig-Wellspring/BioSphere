using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CensusMaster : MonoBehaviour
{
    public static CensusMaster Census { get; private set; }

    [Tooltip("Emergency stop for global overpopulation"), SerializeField]
    private int globalOvergrowth = 30000;
    [Tooltip("Emergency stop for overpopulation of each species"), SerializeField]
    private int speciesOvergrowth = 10000;
    [SerializeField]
    private int totalPopulation = 0;
    public List<CensusData> listOfSpecies;


    private void Awake()
    {
        if (Census == null)
        {
            Census = this;
        }
        else
        {
            Destroy(gameObject); //should never happen
        }
    }

    public void PopulationIncrease(string newMember)
    {
        bool found = false;
        //Search list of species
        foreach (CensusData member in listOfSpecies)
        {
            //If already on the list, add to population
            if (member.speciesName.Equals(newMember))
            {
                //Debug.Log(newMember + " population increased to " + member.populationSize);
                member.populationSize += 1;
                totalPopulation += 1;

                //Crash if population is too high
                if (member.populationSize >= speciesOvergrowth)
                {
                    Debug.Log("Crash due to " + newMember + " overgrowth");
                    UnityEditor.EditorApplication.isPlaying = false;
                }
                if (totalPopulation >= globalOvergrowth)
                {
                    Debug.Log("Crash due to global population overgrowth");
                    UnityEditor.EditorApplication.isPlaying = false;
                }
                found = true;
                break;
            }
        }
        if (!found)
        {
            //If not found on the list, add to list
            listOfSpecies.Add(new CensusData(newMember, 1));
            totalPopulation += 1;
        }

    }

    
    public void PopulationDecrease(string lostMember)
    {
        foreach(CensusData member in listOfSpecies)
        {
            if (member.speciesName.Contains(lostMember))
            {
                //Debug.Log(lostMember + " population decreased to " + member.populationSize);
                member.populationSize -= 1;
                totalPopulation -= 1;

                if (member.populationSize < 1)
                {
                    Debug.Log(lostMember + " has gone extinct.");
                    listOfSpecies.Remove(member);
                }
                break;
            }
        }
    }
}
