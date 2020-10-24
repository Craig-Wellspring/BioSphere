using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class Metabolism : MonoBehaviour
{
    [Header("State")]
    [Tooltip("How Hungry the Creature currently feels")]
    public float hungerUnits = 0f;
    public float hungerPercentage = 0f;
    [Space(10)]
    public List<DietData> dietHistory;

    #region Settings
    [Header("Bite Settings")]
    public Transform mouth;
    [Range(0, 5)] public float biteSize = 0.4f;
    [SerializeField] bool drawBiteSphere = false;
    [Tooltip("How quickly this creature consumes food. Higher is faster. 1 is default.")]
    [Range(0, 10)] public float chewSpeed = 1f;
    [SerializeField] LayerMask biteLayers;


    [Header("Metabolism Settings")]
    [Tooltip("Time in Seconds it takes to gain one unit of Hunger")]
    public float metabolismRate = 3f;
    [SerializeField] float metaRateIncrement = 0.5f;
    [Tooltip("Units of Hunger gained per tick")]
    [SerializeField] float hungerGainedPerTick = 3f;
    [Tooltip("Increase units of Hunger gained per tick every level up")]
    [SerializeField] float hungerPerTickPerLevel = 0.1f;


    [Header("Hunger Index Settings")]
    [Tooltip("Hunger Percentage to become Hungry"), SerializeField]
    [Range(0, 100)] float hungryAtPercent = 20f;
    [Tooltip("Hunger Percentage to begin Wasting"), SerializeField]
    [Range(0, 100)] float wastingAtPercent = 80f;
    [Tooltip("Start eating meat when very hungry"), SerializeField]
    bool hungryMeatEater = true;
    [Tooltip("Die when hunger level maximum reached"), SerializeField]
    float maximumHungerUnits = 100f;


    [Header("Diet Settings")]
    public List<string> dietList;
    public List<string> preyList;


    [Header("Debug")]
    [SerializeField] bool logEating = false;
    [Space(10)]
    public bool isEating;
    public bool isHungry;
    public bool isWasting;
    [Space(10)]
    public FoodData targetFData;

    #endregion


    #region Internal Variables
    //Cache
    DietData morselIngested = new DietData(null, 0);
    float hungerTimer = 0f;
    [HideInInspector] public Slider hungerBar;
    Image hungerFill;
    SingleGradient colorPicker;
    #endregion

    void Start()
    {
        // Initialize UI
        hungerBar = transform.root.Find("Canvas").Find("Hunger Bar").GetComponent<Slider>();
        hungerFill = hungerBar.transform.Find("Hunger Fill").GetComponent<Image>();
        colorPicker = hungerFill.GetComponent<SingleGradient>();
        hungerBar.gameObject.SetActive(false);

        


        CreatureStats cStats = GetComponent<CreatureStats>();
        if (cStats)
        {
            // Register Metabolism Rate in StatBlock
            cStats.AddNewStat("Metabolism", metabolismRate, metaRateIncrement);

            // Increase hunger per level
            cStats.LevelUpBeginning += LevelingUp;
        }
    }


    //// Become more hungry over time \\\\
    void Update()
    {
        if (!isEating)
            Metabolise();

        hungerPercentage = (hungerUnits / maximumHungerUnits) * 100;

        //Update UI
        hungerBar.value = 100 - hungerPercentage;
    }

    void Metabolise()
    {
        hungerTimer += Time.deltaTime;
        if (hungerTimer >= metabolismRate)
        {
            hungerTimer = 0f;

            //Get more hungry and update Hunger status
            if (hungerPercentage < 100)
                hungerUnits += hungerGainedPerTick;

            CheckHungerLevels();
        }
    }


    //// Start Eating \\\\
    public event Action EatingBegins;
    public void StartEating(GameObject _targetFood)
    {
        targetFData = _targetFood.GetComponent<FoodData>();

        isEating = true;
        EatingBegins?.Invoke();
        // - Tell Animator and AI

        if (!targetFData)
            StopEating();

        morselIngested.foodTag = _targetFood.tag;

        //Update UI
        if (!hungerBar.gameObject.activeSelf)
            hungerBar.gameObject.SetActive(true);

        hungerFill.color = colorPicker.gradient.Evaluate(1);

        //Disable egg hatching
        if (_targetFood.tag == "Egg")
            _targetFood.GetComponentInParent<Animator>().SetBool("CanHatch", false);
    }


    //// Stop Eating \\\\
    public event Action EatingEnds;
    public void StopEating()
    {
        isEating = false;
        EatingEnds?.Invoke();
        // - Tell Animator and AI

        // Gain energy from food and update diet history
        if (morselIngested.foodTag != null)
        {
            UpdateDietHistory(morselIngested.foodTag, morselIngested.energyUnits);

            EnergyData eData = GetComponent<EnergyData>();
            if (eData != null)
                eData.AddEnergy(morselIngested.energyUnits);
            else
                Servius.Server.GetComponent<GlobalLifeSource>().energyReserve += morselIngested.energyUnits;

            morselIngested.energyUnits = 0;
            morselIngested.foodTag = null;
        }

        // Clear target data
        targetFData = null;


        //Update UI
        if (hungerBar)
        {
            hungerFill.color = colorPicker.gradient.Evaluate(0);
            //hungerBar.gameObject.SetActive(false);
        }
    }


    Vector3 BiteCenter(float _biteSize)
    {
        return mouth.position + (transform.forward * _biteSize / 2);
    }

    public void Bite()
    {
        Collider[] hitFoodList = Physics.OverlapSphere(BiteCenter(biteSize), biteSize, biteLayers);

        foreach (Collider _hitFood in hitFoodList)
        {
            if (dietList.Contains(_hitFood.tag))
            {
                StartEating(_hitFood.gameObject);
                break;
            }
        }
    }

    //// Draw Debug BiteSphere \\\\
    private void OnDrawGizmosSelected()
    {
        if (drawBiteSphere && mouth)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(BiteCenter(biteSize), biteSize);
        }
    }


    //// Consume food object one bite per frame \\\\
    public void Chew(FoodData _targetFData, float _biteSize)
    {
        // Adjust bite size
        if (_targetFData.nutritionalValue < _biteSize)
            _biteSize = _targetFData.nutritionalValue;

        // Transfer energy from food to mouth cache
        if (_targetFData.RemoveNV(_biteSize))
            morselIngested.energyUnits += _biteSize;

        // Become less hungry
        if (hungerUnits > 0)
            hungerUnits -= _biteSize;

        CheckHungerLevels();

        // Stop eating if completely full
        if (hungerPercentage <= 1)
            StopEating();

        // Destroy Food if no energy remains
        if (_targetFData.nutritionalValue == 0)
            Devour(_targetFData);
    }


    // Update diet history   
    void UpdateDietHistory(string _foodTag, float _energyUnits)
    {
        bool newFood = true;
        foreach (DietData _foodType in dietHistory)
        {
            if (_foodType.foodTag.Equals(_foodTag))
            {
                newFood = false;
                _foodType.energyUnits += _energyUnits;
                break;
            }
        }
        if (newFood)
            dietHistory.Add(new DietData(_foodTag, _energyUnits));
    }

    //// Destroy food item and stop eating \\\\
    void Devour(FoodData _targetFData)
    {
        StopEating();

        if (logEating)
            Debug.Log(this.transform.root.name + " ate " + _targetFData.name + " from " + _targetFData.transform.root.name);

        // Destroy food
        if (_targetFData.destroyRoot)
            Destroy(_targetFData.transform.root.gameObject);
        else
            Destroy(_targetFData.gameObject);
    }

    //// Check if Hungry or Wasting \\\\
    void CheckHungerLevels()
    {
        if (HungerCheck())
            if (WastingCheck())
                WasteAway();
    }

    public event Action HungerChange;
    bool HungerCheck()
    {
        bool _isHungry = hungerPercentage >= hungryAtPercent ? true : false;
        if (isHungry != _isHungry)
        {
            isHungry = _isHungry;
            HungerChange?.Invoke();
        }
        return _isHungry;
    }
    public event Action WastingChange;
    bool WastingCheck()
    {
        bool _isWasting = hungerPercentage >= wastingAtPercent ? true : false;
        if (isWasting != _isWasting)
        {
            isWasting = _isWasting;
            WastingChange?.Invoke();
        }
        return _isWasting;
    }

    // Very Hungry, starving
    void WasteAway()
    {
        //Eat meat if desperate for food
        if (!dietList.Contains("Meat") && hungryMeatEater)
            dietList.Add("Meat");

        Vitality vitality = GetComponent<Vitality>();
        if (!vitality.dead && hungerPercentage >= 100)
            vitality.Die();
    }

    // Increase food required to stay fed
    void LevelingUp()
    {
        hungerGainedPerTick += hungerPerTickPerLevel;
    }
}


[Serializable]
public class DietData
{
    [HideInInspector] public string foodTag;
    public float energyUnits;

    public DietData(string _tag, float _energy)
    {
        this.foodTag = _tag;
        this.energyUnits = _energy;
    }
}
