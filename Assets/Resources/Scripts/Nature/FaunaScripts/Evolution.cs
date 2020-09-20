using UnityEngine;

public class Evolution : MonoBehaviour
{
    #region Stats
    [Header("Current")]
    public int currentLevel = 1;
    public enum StatToEvolve { MaxHealth, MetabolismSpeed, PerceptionRadius };
    public StatToEvolve statToEvolve;
    #endregion

    #region Settings
    [Header("Settings")]
    [Tooltip("Energy Stored is considered In Surplus if beyond this Threshold")]
    public float evolutionCost;
    [SerializeField] float hungerIncreasePerEvo = 0.3f;

    
    [Header("Debug"), SerializeField]
    bool logEvolution = false;
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
        // Trigger beginning events
        EvolutionBeginning?.Invoke();

        // Increase chosen stat
        IncreaseStat();

        // Level up
        IncreaseLevel();
    
        GetComponent<Metabolism>().hungerGainedPerTick += hungerIncreasePerEvo;

        //Trigger ending events
        EvolutionFinishing?.Invoke();

        if (logEvolution)
            Debug.Log(transform.root.name + " evolved to level " + currentLevel + " and increased its " + statToEvolve);
    }



    void IncreaseStat()
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

    void IncreaseLevel()
    {
        currentLevel += 1;
    }
}
