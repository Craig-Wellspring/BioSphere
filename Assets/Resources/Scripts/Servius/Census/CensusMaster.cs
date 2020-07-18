using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CensusMaster : MonoBehaviour
{
    public static CensusMaster Census { get; private set; }

    [Header("Overpopulation Control")]
    [Tooltip("Emergency stop for global overpopulation"), SerializeField]
    private int globalOvergrowth = 30000;
    [Tooltip("Emergency stop for overpopulation of each species"), SerializeField]
    private int speciesOvergrowth = 10000;

    [Header("Event Log")]
    public bool logPopulationIncrease = false;
    public bool logPopulationDecrease = false;
    public bool logExtinctions = false;
    public bool logEmergences = false;

    [Header("Current Populations")]
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

    public void PopulationIncrease(string _newMember)
    {
        bool found = false;
        //Search list of species
        foreach (CensusData member in listOfSpecies)
        {
            //If already on the list, add to population
            if (member.speciesName.Equals(_newMember))
            {

                member.populationSize += 1;
                totalPopulation += 1;

                //Log
                if (logPopulationIncrease)
                    Debug.Log(_newMember + " population increased to " + member.populationSize);

                //Crash if population is too high
                if (member.populationSize >= speciesOvergrowth)
                {
                    Debug.Log("Crash due to " + _newMember + " overgrowth");
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
            listOfSpecies.Add(new CensusData(_newMember, 1));
            totalPopulation += 1;

            //Log
            if (logEmergences)
                Debug.Log(_newMember + " has emerged in the world.");
        }

    }

    
    public void PopulationDecrease(string _lostMember)
    {
        foreach(CensusData member in listOfSpecies)
        {
            if (member.speciesName.Contains(_lostMember))
            {
                member.populationSize -= 1;
                totalPopulation -= 1;

                //Log
                if (logPopulationDecrease)
                    Debug.Log(_lostMember + " population decreased to " + member.populationSize);

                if (member.populationSize < 1)
                {
                    listOfSpecies.Remove(member);

                    //Log
                    if (logExtinctions)
                        Debug.Log(_lostMember + " has gone extinct.");
                }
                break;
            }
        }
    }


    int CurrentPopulation(string nameOfSpecies)
    {
        int currentPopulation = 0;
        foreach (CensusData speciesType in listOfSpecies)
        {
            if (speciesType.speciesName.Contains(nameOfSpecies))
            {
                currentPopulation = speciesType.populationSize;
                break;
            }
        }
        return currentPopulation;
    }
}
