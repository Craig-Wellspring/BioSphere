using UnityEngine;

public class Evolution : ObjectSpawner
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
    EnergyData eData;

    //Events
    public event System.Action EnergyAboveSurplus;
    public event System.Action EnergyBelowSurplus;
    public event System.Action EvolutionBeginning;
    public event System.Action EvolutionFinishing;
    #endregion

    private void Start()
    {
        eData = GetComponent<EnergyData>();

        eData.EnergyGained += SurplusCheck;
        eData.EnergySpent += SurplusCheck;
    }

    private void OnDisable()
    {
        eData.EnergyGained -= SurplusCheck;
        eData.EnergySpent -= SurplusCheck;
    }
    
    void SurplusCheck()
    {
        if (eData.energyReserve >= evolutionCost)
        {
            //Tell UI and AI there is an energy surplus
            EnergyAboveSurplus?.Invoke();
        }
        else EnergyBelowSurplus?.Invoke();
    }


    public void Evolve()
    {
        //Trigger beginning events
        EvolutionBeginning?.Invoke();

        //Increase chosen stat
        IncreaseStat();


        //Check if eligible for new morph form
        GetComponent<Morphology>()?.CalculateMorphology();


        //Trigger ending events
        EvolutionFinishing?.Invoke();

        //Choose Cast-off Seed
        //Expend Energy and plant Seed with the Energy spent to Evolve
        SpawnObject(castoffSeed, 0, false, null, evolutionCost, eData);
    }



    private void IncreaseStat()
    {
        switch(statToEvolve){

            //Max Health
            case StatToEvolve.MaxHealth:
            Vitality vitality = GetComponent<Vitality>();
            vitality.maxHealth += 1;
            vitality.currentHealth += 1;
            break;

            //Metabolism Rate
            case StatToEvolve.MetabolismSpeed:
            GetComponent<Metabolism>().metabolismRate += 0.5f;
            break;

            //Perception Radius
            case StatToEvolve.PerceptionRadius:
            GetComponent<VisualPerception>().perceptionRadius += 0.5f;
            break;
        }
    }
}
