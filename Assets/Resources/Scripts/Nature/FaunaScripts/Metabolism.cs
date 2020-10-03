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
    [SerializeField] float hungerGainedPerLevel = 0.1f;
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
    float energyIngested;
    float hungerTimer = 0f;
    Slider hungerBar;
    Image hungerFill;
    ColorPicker colorPicker;
    #endregion

    private void Start()
    {
        // Initialize UI
        hungerBar = transform.root.Find("Canvas").Find("Hunger Bar").GetComponent<Slider>();
        hungerFill = hungerBar.transform.Find("Hunger Fill").GetComponent<Image>();
        colorPicker = hungerFill.GetComponent<ColorPicker>();
        hungerBar.gameObject.SetActive(false);

        CreatureData cData = GetComponent<CreatureData>();
        if (cData != null)
            cData.LevelUpBeginning += LevelingUp;
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
        currentTargetFood.GetComponent<OnDestroyEvent>().BeingDestroyed += StopEating;

        if (!targetFData)
            StopEating();


        //Update UI
        if (!hungerBar.gameObject.activeSelf)
        {
            hungerBar.gameObject.SetActive(true);
            hungerFill.color = colorPicker.secondaryColor;
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


        // Update diet history   
        if (targetFData != null)
        {
            bool newFood = true;
            foreach (DietData _foodType in dietHistory)
            {
                if (_foodType.foodTag.Equals(targetFData.tag))
                {
                    newFood = false;
                    _foodType.energyUnits += energyIngested;
                    break;
                }
            }
            if (newFood)
                dietHistory.Add(new DietData(targetFData.tag, energyIngested));


            // Gain energy from food eaten
            GetComponent<EnergyData>().AddEnergy(energyIngested);
            energyIngested = 0;

            // Clear target data
            currentTargetFood.GetComponent<OnDestroyEvent>().BeingDestroyed -= StopEating;
            targetFData = null;
        }
        currentTargetFood = null;


        //Update UI
        if (hungerBar)
        {
            hungerFill.color = colorPicker.primaryColor;
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
        energyIngested += _biteSize;

        // Become less hungry
        if (hungerUnits > 0)
            hungerUnits -= _biteSize;


        // Destroy Food if no energy remains
        if (_targetFData.nutritionalValue == 0)
        {
            Devour(_targetFData.gameObject);
            StopEating();
        }

        CheckHungerLevels();

        // Stop eating if completely full
        if (hungerPercentage <= 1)
            StopEating();
    }


    //// Destroy food item and stop eating \\\\
    void Devour(GameObject _target)
    {
        if (logEating)
            Debug.Log(this.transform.root.name + " ate " + _target.name + " from " + _target.transform.root.name);

        // Destroy food
        if (targetFData.destroyRoot)
            Destroy(_target.transform.root.gameObject);
        else
            Destroy(_target);
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
        hungerGainedPerTick += hungerGainedPerLevel;
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
