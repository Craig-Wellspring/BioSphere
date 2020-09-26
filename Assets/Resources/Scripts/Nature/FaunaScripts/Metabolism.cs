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
    float hungryAtPercent = 10f;
    [Tooltip("Hunger Percentage to begin Wasting"), SerializeField]
    float wastingAtPercent = 90f;
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
    public Transform mouth;
    public float biteSize = 0.4f;
    [Tooltip("How quickly this creature consumes food. 1 is default.")]
    public float chewSpeed = 1f;


    [Header("Debug")]
    [SerializeField] bool logEating = false;
    [SerializeField] bool drawBiteSphere = false;
    public EnergyData targetEData;
    public GameObject currentTargetFood = null;

    #endregion


    #region Internal Variables
    //Cache
    float hungerTimer = 0f;
    Slider hungerBar;
    Image hungerFill;
    float energyIngested;

    EnergyData selfEData;
    Vitality vitality;

    Color primaryColor;
    Color secondaryColor;


    //Events
    public event Action EatingBegins;
    public event Action EatingEnds;
    public event Action NowHungry;
    public event Action NowFull;
    public event Action WastingBegins;
    public event Action WastingEnds;

    #endregion

    private void Start()
    {
        selfEData = GetComponent<EnergyData>();
        vitality = GetComponent<Vitality>();

        // Initialize UI
        hungerBar = transform.root.Find("Canvas").Find("Hunger Bar").GetComponent<Slider>();
        hungerFill = hungerBar.transform.Find("Hunger Fill").GetComponent<Image>();
        primaryColor = hungerFill.GetComponent<ColorPicker>().primaryColor;
        secondaryColor = hungerFill.GetComponent<ColorPicker>().secondaryColor;
        hungerBar.gameObject.SetActive(false);
    }

    private void Update()
    {
        Metabolise();
    }

    //// Become more hungry over time \\\\
    private void Metabolise()
    {
        hungerTimer += Time.deltaTime;
        if (hungerTimer >= metabolismRate)
        {
            hungerTimer = 0f;
            //Get more hungry
            if (hungerPercentage < 100)
                hungerUnits += hungerGainedPerTick;
            hungerPercentage = (hungerUnits / maximumHungerUnits) * 100;

            //Update UI
            hungerBar.value = 100 - hungerPercentage;

            //Check if hungry
            if (hungerPercentage >= hungryAtPercent)
            {
                NowHungry?.Invoke();

                if (hungerPercentage >= wastingAtPercent)
                {
                    WastingBegins?.Invoke();
                    WasteAway();
                }
            }
        }
    }



    //// Start Eating \\\\
    public void StartEating(GameObject _targetFood)
    {
        EatingBegins();

        currentTargetFood = _targetFood;
        targetEData = currentTargetFood.GetComponent<EnergyData>();
        currentTargetFood.GetComponent<OnDestroyEvent>().BeingDestroyed += StopEating;

        if (!targetEData)
            StopEating();


        //Update UI
        if (!hungerBar.gameObject.activeSelf)
        {
            hungerBar.gameObject.SetActive(true);
            hungerFill.color = secondaryColor;
        }

        //Disable egg hatching
        if (_targetFood.tag == "Egg")
            _targetFood.GetComponentInParent<Animator>().SetBool("CanHatch", false);
    }




    //// Consume food object one bite per frame \\\\
    public void Bite(EnergyData _eData, float _biteSize)
    {
        // Adjust bite size
        if (_eData.nutritionalValue < _biteSize)
            _biteSize = _eData.nutritionalValue;

        // Remove energy from food
        _eData.nutritionalValue -= _biteSize;

        // Cache energy gained from bite
        energyIngested += _biteSize;

        // Become less hungry
        if (hungerUnits > 0)
            hungerUnits -= _biteSize;


        // Destroy Food if no energy remains
        if (_eData.nutritionalValue == 0)
        {
            Devour(_eData.gameObject);
            StopEating();
        }


        // Check Hunger levels and stop when completely full
        if (hungerPercentage <= hungryAtPercent)
        {
            NowFull?.Invoke();
            return;
        }
        if (hungerPercentage <= wastingAtPercent)
        {
            WastingEnds?.Invoke();
            return;
        }
        if (hungerPercentage <= 1)
        {
            StopEating();
        }
    }



    //// Stop Eating \\\\
    public void StopEating()
    {
        EatingEnds?.Invoke();


        // Update diet history   
        if (targetEData != null)
        {
            bool newFood = true;
            foreach (DietData _foodType in dietHistory)
            {
                if (_foodType.foodTag.Equals(targetEData.tag))
                {
                    newFood = false;
                    _foodType.energyUnits += energyIngested;
                    break;
                }
            }
            if (newFood)
                dietHistory.Add(new DietData(targetEData.tag, energyIngested));


            // Gain energy from food eaten
            selfEData.GainEnergy(energyIngested);
            energyIngested = 0;

            // Clear target data
            currentTargetFood.GetComponent<OnDestroyEvent>().BeingDestroyed -= StopEating;
            targetEData = null;
        }
        currentTargetFood = null;


        //Update UI
        if (hungerBar)
        {
            hungerFill.color = primaryColor;
            hungerBar.gameObject.SetActive(false);
        }
    }




    //// Destroy food item and stop eating \\\\
    private void Devour(GameObject _target)
    {
        if (logEating)
            Debug.Log(this.transform.root.name + " ate " + _target.name + " from " + _target.transform.root.name);

        // Destroy food
        if (targetEData.destroyRoot)
            Destroy(_target.transform.root.gameObject);
        else
            Destroy(_target);
    }

    //// Very Hungry, starving \\\\
    private void WasteAway()
    {
        //Eat meat if desperate for food
        if (!dietList.Contains("Meat") && hungryMeatEater)
            dietList.Add("Meat");

        if (!vitality.dead && hungerPercentage >= 100)
            vitality.Die();
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
