using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evolution : MonoBehaviour
{
    #region Settings
    [Tooltip("Energy Stored is considered In Surplus if beyond this Threshold")]
    public float evolutionCost;
    [Space(10)]
    public GameObject castoffSeed;
    
    public enum StatToEvolve { MaxHealth, MetabolismSpeed, PerceptionRadius };
    public StatToEvolve statToEvolve;
    #endregion

    #region Private Variables
    //Cache
    SeedSpawner seedSpawner;
    Vitality vitality;
    Metabolism metabolism;
    VisualPerception vPerception;
    Morphology morphology;

    //Events
    public event System.Action EnergyAboveSurplus;
    public event System.Action EnergyBelowSurplus;
    public event System.Action EvolutionBeginning;
    public event System.Action EvolutionFinishing;
    #endregion

    private void Start()
    {
        seedSpawner = GetComponent<SeedSpawner>();
        vitality = GetComponent<Vitality>();
        metabolism = GetComponent<Metabolism>();
        vPerception = GetComponent<VisualPerception>();
        morphology = GetComponent<Morphology>();

        metabolism.EnergyGained += SurplusCheck;
        metabolism.EnergySpent += SurplusCheck;
    }

    private void OnDisable()
    {
        metabolism.EnergyGained -= SurplusCheck;
        metabolism.EnergySpent -= SurplusCheck;
    }
    
    void SurplusCheck()
    {
        if (metabolism.storedEnergy >= evolutionCost)
        {
            //Tell UI and AI there is an energy surplus
            EnergyAboveSurplus();
        }
        else EnergyBelowSurplus();
    }


    public void Evolve()
    {
        //Trigger beginning events
        EvolutionBeginning();

        //Increase chosen stat
        IncreaseStat();


        //Check if eligible for new morph form
        morphology.CalculateMorphology();


        //Trigger ending events
        EvolutionFinishing?.Invoke();

        //Choose Cast-off Seed
        //Expend Energy and plant Seed with the Energy spent to Evolve
        seedSpawner.PlantSeed(evolutionCost, castoffSeed);
        metabolism.SpendEnergy(evolutionCost);

    }



    private void IncreaseStat()
    {
        //Max Health
        if (statToEvolve == StatToEvolve.MaxHealth)
        {
            vitality.maxHealth += 1;
            vitality.currentHealth += 1;
        }

        //Metabolism Rate
        else if (statToEvolve == StatToEvolve.MetabolismSpeed)
        {
            metabolism.metabolismRate += 0.5f;
        }


        //Perception Radius
        else if (statToEvolve == StatToEvolve.PerceptionRadius)
        {
            vPerception.perceptionRadius += 0.5f;
        }
    }
}
