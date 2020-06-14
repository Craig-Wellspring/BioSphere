using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(Vitality))]
[RequireComponent(typeof(CreatureData))]
public class Metabolism : MonoBehaviour
{
    #region Settings

    public Slider hungerSlider;

    [Header("Current Hunger")]
    [Tooltip("How Hungry the Creature currently feels")]
    public float hungerUnits = 0f;
    public float hungerPercentage = 0f;
    public bool isEating = false;
    public bool logEating = false;
    
    [Header("Metabolism Settings")]
    [Tooltip("Time in Seconds it takes to gain one unit of Hunger")]
    public float metabolismRate = 1f;
    [Tooltip("Units of Hunger gained per tick")]
    public float hungerGainedPerTick = 1f;
    [Tooltip("How quickly this creature consumes food. 1 is default.")]
    public float chewSpeed = 1f;

    [Header("Creature Diet")]
    public List<string> dietList;

    [Header("Hunger Index Settings")]
    [Tooltip("Hunger Percentage to become Hungry")]
    public float hungryAtPercent = 10f;
    [Tooltip("Hunger Percentage to begin Wasting")]
    public float wastingAtPercent = 90f;
    [Tooltip("Die when hunger level maximum reached")]
    public float maximumHungerUnits = 100f;
    #endregion


    #region Internal Variables
    private CreatureData cData;
    private FoodData targetFData;
    private GameObject currentTargetFood = null;
    private float hungerTimer = 0f;
    private Image hungerBarColor;
    private float chewRate;
    #endregion

    private void Start()
    {
        cData = GetComponent<CreatureData>();
        hungerBarColor = hungerSlider.GetComponentInChildren<Image>();
    }

    private void Update()
    {
        Metabolise();

        if (isEating)
            Chew();

        if (IsWasting())
            WasteAway();
    }


    #region Functions
    public void StartEating(GameObject _targetFood)
    {
        isEating = true;
        currentTargetFood = _targetFood;

        hungerBarColor.color = hungerSlider.GetComponentInChildren<ColorPicker>().secondaryColor;
        //animator.SetBool("Eating", true);

        targetFData = _targetFood.GetComponent<FoodData>();
        chewRate = targetFData.chewRateModifier * chewSpeed;

        if (!cData.lifetimeDiet.Contains(_targetFood.tag))
            cData.lifetimeDiet.Add(_targetFood.tag);

        if (_targetFood.tag == "Egg")
            _targetFood.GetComponent<HatchCreature>().canHatch = false;
    }


    public void StopEating()
    {
        isEating = false;
        currentTargetFood = null;

        hungerBarColor.color = hungerSlider.GetComponentInChildren<ColorPicker>().primaryColor;
        //animator.SetBool("Eating", false);
        
    }

    //Continue eating if possible
    private void Chew()
    {
        if (currentTargetFood != null)
        {
            Ingest(targetFData, chewRate * Time.deltaTime);
            
            if (targetFData.nutritionalValue == 0)
                FinishEating(currentTargetFood);
        }
        else StopEating();
    }


    //Gain nutritional value from the food
    private void Ingest(FoodData _fData, float _biteSize)
    {
        if (_fData.nutritionalValue < _biteSize)
            _biteSize = _fData.nutritionalValue;

        _fData.nutritionalValue -= _biteSize;
        if (hungerUnits > 0)
            hungerUnits -= _biteSize;
        cData.energyUnits += _biteSize;

    }

    //Destroy food item and stop eating
    private void FinishEating(GameObject _target)
    {
        StopEating();
        if (_target.gameObject != null)
        {
            //Satiate
            if (logEating)
                Debug.Log(this.transform.root.name + " ate " + _target.name + " from " + _target.transform.root.name);
            
            //Destroy food
            if (targetFData.destroyParent)
                Destroy(_target.transform.root.gameObject);
            else
                Destroy(_target);
        }
    }
    

    //Become more hungry over time
    private void Metabolise()
    {
        hungerTimer += Time.deltaTime;
        if (hungerTimer >= metabolismRate)
        {
            hungerTimer = 0f;
            hungerPercentage = (hungerUnits / maximumHungerUnits) * 100;
            hungerSlider.value = 100 - hungerPercentage;
            if (hungerPercentage < 100)
                hungerUnits += hungerGainedPerTick;
        }
    }

    //Check if hungry
    public bool IsHungry()
    {
        if (hungerPercentage >= hungryAtPercent)
            return true;
        else return false;
    }

    //Check if wasting
    public bool IsWasting()
    {
        if (hungerPercentage >= wastingAtPercent)
        {
            if (!dietList.Contains("Meat"))
                dietList.Add("Meat");
            return true;
        }
        else return false;
    }


    //hungerIndex > 90%, starving
    private void WasteAway()
    {
        if (hungerPercentage >= 100)
            GetComponent<Vitality>().Die();
    }
    #endregion
}
