using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(OnDestroyEvent))]
[RequireComponent(typeof(EnergyData))]
public class Metabolism : MonoBehaviour
{
    [Header("Current Hunger")]
    [Tooltip("How Hungry the Creature currently feels")]
    public float hungerUnits = 0f;
    public float hungerPercentage = 0f;
    [Space(10)]
    public List<DietData> dietHistory;

    #region Settings
    [Header("Hunger Index Settings")]
    [Tooltip("Hunger Percentage to become Hungry"), SerializeField]
    [Range(0, 100)] float hungryAtPercent = 20f;
    [Tooltip("Hunger Percentage to begin Wasting"), SerializeField]
    [Range(0, 100)] float wastingAtPercent = 80f;
    [Tooltip("Die when hunger level maximum reached"), SerializeField]
    float maximumHungerUnits = 100f;
    [Tooltip("Start eating meat when very hungry"), SerializeField]
    bool hungryMeatEater = true;


    [Header("Creature Diet")]
    public List<string> dietList;
    public List<string> preyList;


    [Header("Metabolism Settings")]
    [Tooltip("Time in Seconds it takes to gain one unit of Hunger")]
    public float metabolismRate = 3f;
    [Tooltip("Units of Hunger gained per tick")]
    public float hungerGainedPerTick = 3f;
    [Tooltip("Increase units of Hunger gained per tick every level up")]
    [SerializeField] float hungerPerTickPerLevel = 0.1f;
    public Transform mouth;
    [Range(0, 5)] public float biteSize = 0.4f;
    [Tooltip("How quickly this creature consumes food. Higher is faster. 1 is default.")]
    [Range(0, 10)] public float chewSpeed = 1f;


    [Header("Debug")]
    [SerializeField] bool logEating = false;
    [SerializeField] bool drawBiteSphere = false;
    [Space(10)]
    public bool isEating;
    public bool isHungry;
    public bool isWasting;
    public FoodData targetFData;
    public GameObject currentTargetFood = null;

    #endregion


    #region Internal Variables
    //Cache
    DietData morselIngested = new DietData(null, 0);
    float hungerTimer = 0f;
    Slider hungerBar;
    Image hungerFill;
    SingleGradient colorPicker;
    #endregion

    private void Start()
    {
        // Initialize UI
        hungerBar = transform.root.Find("Canvas").Find("Hunger Bar").GetComponent<Slider>();
        hungerFill = hungerBar.transform.Find("Hunger Fill").GetComponent<Image>();
        colorPicker = hungerFill.GetComponent<SingleGradient>();
        hungerBar.gameObject.SetActive(false);


        //GetComponent<CreatureData>()?.LevelUpBeginning += LevelingUp;
    }


    //// Become more hungry over time \\\\
    void Update()
    {
        hungerPercentage = (hungerUnits / maximumHungerUnits) * 100;

        if (!isEating)
            Metabolise();

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
        isEating = true;
        EatingBegins?.Invoke();

        currentTargetFood = _targetFood;
        targetFData = currentTargetFood.GetComponent<FoodData>();
        morselIngested.foodTag = _targetFood.tag;

        if (!targetFData)
            StopEating();


        //Update UI
        if (!hungerBar.gameObject.activeSelf)
        {
            hungerBar.gameObject.SetActive(true);
            //hungerFill.color = colorPicker.gradient.Sample(1);
        }

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

        // Gain energy from food and update diet history
        if (morselIngested.foodTag != null)
        {
            UpdateDietHistory(morselIngested.foodTag, morselIngested.energyUnits);

            GetComponent<EnergyData>().AddEnergy(morselIngested.energyUnits);
            morselIngested.energyUnits = 0;
            morselIngested.foodTag = null;
        }

        // Clear target data
        if (currentTargetFood != null)
        {
            currentTargetFood = null;
        }
        targetFData = null;


        //Update UI
        if (hungerBar)
        {
            //hungerFill.color = colorPicker.gradient.Sample(0);
            hungerBar.gameObject.SetActive(false);
        }
    }





    //// Consume food object one bite per frame \\\\
    public void Bite(FoodData _targetFData, float _biteSize)
    {
        // Adjust bite size
        if (_targetFData.nutritionalValue < _biteSize)
            _biteSize = _targetFData.nutritionalValue;

        // Transfer energy from food to mouth cache
        _targetFData.RemoveNV(_biteSize);
        morselIngested.energyUnits += _biteSize;


        // Become less hungry
        if (hungerUnits > 0)
            hungerUnits -= _biteSize;


        // Destroy Food if no energy remains
        if (_targetFData.nutritionalValue == 0)
            Devour(_targetFData);

        CheckHungerLevels();

        // Stop eating if completely full
        if (hungerPercentage <= 1)
            StopEating();
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



    //// Draw Debug BiteSphere \\\\
    private void OnDrawGizmosSelected()
    {
        if (drawBiteSphere && mouth != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(mouth.position + (transform.forward * biteSize / 2), biteSize);
        }
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
