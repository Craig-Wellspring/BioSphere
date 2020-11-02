using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CensusMaster : MonoBehaviour
{
    #region Singleton
    public static CensusMaster Census { get; private set; }
    private void Awake()
    {
        if (Census == null)
            Census = this;
        else
            Destroy(gameObject); //should never happen
    }
    #endregion


    [Header("Overpopulation Control")]
    [Tooltip("Emergency stop for global overpopulation"), SerializeField]
    int globalOvergrowth = 5000;
    [Tooltip("Emergency stop for overpopulation of each species"), SerializeField]
    int speciesOvergrowth = 3000;


    [Header("Event Log")]
    public bool logPopulationIncrease = false;
    public bool logPopulationDecrease = false;
    public bool logEmergences = false;
    public bool logExtinctions = false;


    [Header("Current Populations")]
    [SerializeField]
    int totalPopulation = 0;
    [Space(10)]
    public List<SpeciesRecord> listOfSpecies;



    public void PopulationIncrease(string _newMember)
    {
        bool exists = false;
        //Search list of species
        foreach (SpeciesRecord _member in listOfSpecies)
        {
            //If already on the list, add to population
            if (_member.speciesName.Equals(_newMember))
            {
                _member.current += 1;
                _member.allTime += 1;
                totalPopulation += 1;

                //Log
                if (logPopulationIncrease)
                    Debug.Log(_newMember + " population increased to " + _member.current);

                //Crash if population is too high
                if (_member.current >= speciesOvergrowth)
                {
                    Debug.LogError("Warning: " + _newMember + " overgrowth");
                    //UnityEditor.EditorApplication.isPlaying = false;
                }
                if (totalPopulation >= globalOvergrowth)
                {
                    Debug.LogError("Warning: Global population overgrowth");
                    //UnityEditor.EditorApplication.isPlaying = false;
                }
                exists = true;
                break;
            }
        }
        if (!exists)
        {
            //If not found on the list, add to list
            listOfSpecies.Add(new SpeciesRecord(_newMember, 1, 1));
            totalPopulation += 1;

            //Log
            if (logEmergences)
                Debug.Log(_newMember + " has emerged in the world.");
        }
    }


    public void PopulationDecrease(string _lostMember)
    {
        foreach (SpeciesRecord _member in listOfSpecies)
        {
            if (_member.speciesName.Equals(_lostMember))
            {
                _member.current -= 1;
                totalPopulation -= 1;

                //Log
                if (logPopulationDecrease)
                    Debug.Log(_lostMember + " population decreased to " + _member.current);

                if (_member.current < 1)
                {
                    //listOfSpecies.Remove(_member);

                    //Log
                    if (logExtinctions)
                        Debug.Log(_lostMember + " has gone extinct.");
                }
                break;
            }
        }
    }


    public int CurrentPopulation(string _nameOfSpecies)
    {
        foreach (SpeciesRecord _member in listOfSpecies)
            if (_member.speciesName.Equals(_nameOfSpecies))
                return _member.current;
        
        // else
        return 0;
    }
}


[System.Serializable]
public class SpeciesRecord
{
    [HideInInspector] public string speciesName;
    public int current;
    public int allTime;

    public SpeciesRecord(string _name, int _pop, int _allTimePop)
    {
        this.speciesName = _name;
        this.current = _pop;
        this.allTime = _allTimePop;
    }
}