using UnityEngine;

[RequireComponent(typeof(CreatureData), typeof(EnergyData))]
public class Evolution : MonoBehaviour
{
    #region Settings
    [Header("Settings")]
    [Tooltip("Energy Stored is considered In Surplus if beyond this Threshold")]
    public float evolutionCost;
    [SerializeField] float hungerIncreasePerEvo = 0.1f;


    [Header("Debug"), SerializeField]
    bool logEvolution = false;
    #endregion

    #region Private Variables
    //Cache
    EnergyData eData;
    CreatureData cData;

    //Events
    public event System.Action EvolutionBeginning;
    public event System.Action EvolutionFinishing;

    #endregion

    void Start()
    {
        eData = GetComponent<EnergyData>();
        cData = GetComponent<CreatureData>();
    }


    public void Evolve()
    {
        // Trigger beginning events 
        EvolutionBeginning?.Invoke();
        // - AI decides which stat to increase

        // Level up
        cData.IncreaseLevel();

        // Increase chosen stat
        cData.IncreaseStat(cData.targetCreatureStat);

        /* // Increase food required to stay fed
        GetComponent<Metabolism>().hungerGainedPerTick += hungerIncreasePerEvo;*/

        //Trigger ending events
        EvolutionFinishing?.Invoke();


        if (logEvolution)
            Debug.Log(transform.root.name + " evolved to level " + cData.currentLevel + " and increased its " + cData.targetCreatureStat + " to " + cData.CurrentTargetStat().baseValue);
    }
}
