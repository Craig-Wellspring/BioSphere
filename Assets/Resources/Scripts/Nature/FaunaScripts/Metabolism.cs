using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(OnDestroyEvent))]
[RequireComponent(typeof(EnergyData))]
public class Metabolism : MonoBehaviour
{
    #region Settings

    [Header("Debug")]
    [SerializeField] private bool logEating = false;
    [SerializeField] private bool drawBiteSphere = false;
    public EnergyData targetEData;
    public GameObject currentTargetFood = null;


    [Header("Current Hunger")]
    [Tooltip("How Hungry the Creature currently feels")]
    public float hungerUnits = 0f;
    public float hungerPercentage = 0f;
    
    [Header("Hunger Index Settings")]
    [Tooltip("Hunger Percentage to become Hungry"), SerializeField]
    private float hungryAtPercent = 10f;
    [Tooltip("Hunger Percentage to begin Wasting"), SerializeField]
    private float wastingAtPercent = 90f;
    [Tooltip("Die when hunger level maximum reached"), SerializeField]
    private float maximumHungerUnits = 100f;
    [Tooltip("Start eating meat when very hungry"), SerializeField]
    private bool hungryMeatEater = true;

    [Header("Metabolism Settings")]
    [Tooltip("Time in Seconds it takes to gain one unit of Hunger")]
    public float metabolismRate = 3f;
    [Tooltip("Units of Hunger gained per tick"), SerializeField]
    public float hungerGainedPerTick = 3f;
    public Transform mouth;
    public float biteSize = 0.4f;
    [Tooltip("How quickly this creature consumes food. 1 is default.")]
    public float chewSpeed = 1f;

    [Header("Creature Diet")]
    public List<DietData> dietHistory;
    [Space(15)]
    public List<string> dietList;
    public List<string> preyList;


    #endregion


    #region Internal Variables
    //Cache
    float hungerTimer = 0f;
    Slider hungerBar;
    Image hungerFill;

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


    //// Stop Eating \\\\
    public void StopEating()
    {
        EatingEnds?.Invoke();

        if (targetEData != null)
        {
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



    //// Gain nutritional value from the food \\\\
    public void Ingest(EnergyData _eData, float _biteSize)
    {
        //Allocate Energy
        if (_eData.nutritionalValue < _biteSize)
            _biteSize = _eData.nutritionalValue;

        _eData.nutritionalValue -= _biteSize;
        selfEData.GainEnergy(_biteSize);

        if (_eData.nutritionalValue == 0)
        {
            Devour(_eData.gameObject);
            StopEating();
        }

        if (hungerUnits > 0)
            hungerUnits -= _biteSize;


        //Update diet history        
        bool newFood = true;
        foreach (DietData foodType in dietHistory)
        {
            if (foodType.foodTag.Equals(_eData.tag))
            {
                newFood = false;
                foodType.energyUnits += _biteSize;
                break;
            }
        }
        if (newFood)
        {
            dietHistory.Add(new DietData(_eData.tag, _biteSize));
        }

        //Check Hunger and Energy levels
        if (hungerPercentage <= hungryAtPercent)
        {
            StopEating();
            NowFull?.Invoke();
            return;
        }
        if (hungerPercentage <= wastingAtPercent)
        {
            WastingEnds?.Invoke();
        }
    }



    //// Destroy food item and stop eating \\\\
    private void Devour(GameObject _target)
    {
        if (logEating)
            Debug.Log(this.transform.root.name + " ate " + _target.name + " from " + _target.transform.root.name);

        //Destroy food
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
        if (drawBiteSphere)
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
