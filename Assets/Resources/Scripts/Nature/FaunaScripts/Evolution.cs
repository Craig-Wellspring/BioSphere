using UnityEngine;

public class Evolution : ObjectSpawner
{
    #region Settings
    public bool logEvolution = false;

    public enum StatToEvolve { MaxHealth, MetabolismSpeed, PerceptionRadius };
    [Space(15)]
    public StatToEvolve statToEvolve;

    [Header("Settings")]
    public GameObject castoffSeed;
    [Tooltip("Energy Stored is considered In Surplus if beyond this Threshold")]
    public float evolutionCost;
    [SerializeField] float hungerIncreasePerEvo = 0.3f;
    #endregion

    #region Private Variables
    //Cache
    EnergyData eData;

    //Events
    public event System.Action EvolutionBeginning;
    public event System.Action EvolutionFinishing;
    #endregion

    void Start()
    {
        eData = GetComponent<EnergyData>();
    }


    public void Evolve()
    {
        //Trigger beginning events
        EvolutionBeginning?.Invoke();

        //Increase chosen stat
        IncreaseStat();
        GetComponent<Metabolism>().hungerGainedPerTick += hungerIncreasePerEvo;

        //Trigger ending events
        EvolutionFinishing?.Invoke();

        if (logEvolution)
            Debug.Log(transform.root.name + " evolved and increased its " + statToEvolve);
    }

    public void CastOffSeed()
    {
        //Choose Cast-off Seed
        //Expend Energy and plant Seed with the Energy spent to Evolve
        SpawnObject(castoffSeed, 2, false, null, evolutionCost, eData);
    }



    private void IncreaseStat()
    {
        switch (statToEvolve)
        {

            //Max Health
            case StatToEvolve.MaxHealth:
                GetComponent<Vitality>().IncreaseMaxHealth(1);
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
