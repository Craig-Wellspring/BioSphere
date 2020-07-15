using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class Metabolism : MonoBehaviour
{
    #region Settings

    [SerializeField] private Slider hungerBar;
    [SerializeField] private Image hungerFill;

    [Header("Current Hunger")]
    [Tooltip("How Hungry the Creature currently feels")]
    public float hungerUnits = 0f;
    public float hungerPercentage = 0f;
    [Tooltip("Total Stored Energy Value")]
    public float storedEnergy;
    
    [Header("Metabolism Settings")]
    [Tooltip("Time in Seconds it takes to gain one unit of Hunger")]
    public float metabolismRate = 3f;
    [Tooltip("Units of Hunger gained per tick"), SerializeField]
    float hungerGainedPerTick = 3f;
    [Tooltip("How quickly this creature consumes food. 1 is default.")]
    public float chewSpeed = 1f;

    [Header("Creature Diet")]
    public List<DietData> dietHistory;
    [Space(15)]
    public List<string> dietList;
    public List<string> preyList;


    [Header("Hunger Index Settings")]
    [Tooltip("Hunger Percentage to become Hungry"), SerializeField]
    float hungryAtPercent = 10f;
    [Tooltip("Hunger Percentage to begin Wasting"), SerializeField]
    float wastingAtPercent = 90f;
    [Tooltip("Die when hunger level maximum reached"), SerializeField]
    float maximumHungerUnits = 100f;

    [Header("Debug")]
    public FoodData targetFData;
    public GameObject currentTargetFood = null;
    [SerializeField] private bool logEating = false;
    #endregion


    #region Internal Variables
    //Cache
    float hungerTimer = 0f;

    private Color primaryColor;
    private Color secondaryColor;


    //Events
    public event Action EatingBegins;
    public event Action EatingEnds;
    public event Action NowHungry;
    public event Action NowFull;
    public event Action WastingBegins;
    public event Action WastingEnds;
    public event Action EnergySpent;
    public event Action EnergyGained;
    #endregion

    private void Start()
    {
        primaryColor = hungerFill.GetComponent<ColorPicker>().primaryColor;
        secondaryColor = hungerFill.GetComponent<ColorPicker>().secondaryColor;
    }

    private void Update()
    {
        Metabolise();
    }

    

    //// Start Eating \\\\
    public void StartEating(GameObject _targetFood)
    {
        EatingBegins();

        //currentTargetFood = _targetFood;
        targetFData = currentTargetFood.GetComponent<FoodData>();
        targetFData.FoodDestroyed += StopEating;
        
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
        EatingEnds();
        if (targetFData != null)
        {
            targetFData.FoodDestroyed -= StopEating;
            targetFData = null;
        }

        currentTargetFood = null;
        

        //Update UI
        if (hungerBar.gameObject.activeSelf)
        {
            hungerBar.gameObject.SetActive(false);
            hungerFill.color = primaryColor;
        }
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
            if (hungerPercentage >= wastingAtPercent)
            {
                WastingBegins();
                WasteAway();
                return;
            }
            if (hungerPercentage >= hungryAtPercent)
            {
                NowHungry?.Invoke();
            }
        }
    }



    //// Gain nutritional value from the food \\\\
    public void Ingest(FoodData _fData, float _biteSize)
    {
        //Allocate Energy
        if (_fData.nutritionalValue < _biteSize)
            _biteSize = _fData.nutritionalValue;

        _fData.nutritionalValue -= _biteSize;

        if (_fData.nutritionalValue == 0)
        {
            Devour(_fData.gameObject);
            StopEating();
        }

        if (hungerUnits > 0)
            hungerUnits -= _biteSize;

        GainEnergy(_biteSize);

        //Update diet history        
        bool newFood = true;
        foreach (DietData foodType in dietHistory)
        {
            if (foodType.foodTag.Equals(_fData.tag))
            {
                newFood = false;
                foodType.energyUnits += _biteSize;
                break;
            }
        }
        if (newFood)
        {
            dietHistory.Add(new DietData(_fData.tag, _biteSize));
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
            WastingEnds();
        }
    }



    //// Destroy food item and stop eating \\\\
    private void Devour(GameObject _target)
    {
        if (logEating)
            Debug.Log(this.transform.root.name + " ate " + _target.name + " from " + _target.transform.root.name);

        //Destroy food
        if (targetFData.destroyRoot)
            Destroy(_target.transform.root.gameObject);
        else
            Destroy(_target);
    }



    //// Very Hungry, starving \\\\
    private void WasteAway()
    {
        //Eat meat if desperate for food
        if (!dietList.Contains("Meat"))
            dietList.Add("Meat");

        if (hungerPercentage >= 100)
            GetComponent<Vitality>().Die();
    }



    //// Increase and Decrease Energy \\\\
    public void SpendEnergy(float _amount)
    {
        storedEnergy -= _amount;
        
        EnergySpent();
    }
    public void GainEnergy(float _amount)
    {
        storedEnergy += _amount;

        EnergyGained();
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
