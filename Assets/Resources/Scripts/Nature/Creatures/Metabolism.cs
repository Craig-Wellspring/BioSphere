using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(Vitality))]
[RequireComponent(typeof(CreatureData))]
public class Metabolism : MonoBehaviour
{
    #region Settings

    public Slider hungerSlider;
    public Slider chewSlider;

    [Header("Current Hunger")]
    [Tooltip("How Hungry the Creature currently feels")]
    public float hungerUnits = 0f;
    public float hungerPercentage = 0f;
    
    [Header("Metabolism Settings")]
    [Tooltip("Time in Seconds it takes to gain one unit of Hunger")]
    public float metabolismRate = 1f;
    [Tooltip("Units of Hunger gained per tick")]
    public float hungerGainedPerTick = 1f;
    
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
    [HideInInspector] public bool isEating = false;
    private GameObject currentTargetFood = null;
    private float hungerTimer = 0f;
    private float chewingTimer = 0f;
    private float chewTime;
    #endregion

    private void Start()
    {
        cData = GetComponent<CreatureData>();
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
    public void StartEating(GameObject targetFood)
    {
        isEating = true;
        currentTargetFood = targetFood;
        chewSlider.gameObject.SetActive(true);
        //animator.SetBool("Eating", true);

        var food = targetFood.GetComponent<FoodData>();
        chewTime = food.timeToEat;
        chewSlider.maxValue = food.timeToEat;
    }


    public void StopEating()
    {
        isEating = false;
        currentTargetFood = null;
        chewSlider.gameObject.SetActive(false);
        //animator.SetBool("Eating", false);

        chewSlider.value = 0;
        chewSlider.maxValue = 1;
    }

    //Continue eating if possible
    private void Chew()
    {
        if (currentTargetFood != null)
        {
            chewingTimer += Time.deltaTime;
            chewSlider.value = chewingTimer;
            if (chewingTimer > chewTime)
            {
                chewingTimer = 0f;
                FinishEating(currentTargetFood);
            }
        }
        else StopEating();
    }

    //Destroy food item and stop eating
    private void FinishEating(GameObject target)
    {
        StopEating();
        if (target.gameObject != null)
        {
            //Satiate
            Digest(target);
            Debug.Log(this.transform.root.name + " ate " + target.name + " from " + target.transform.root.name);
            
            //Destroy food
            if (target.GetComponent<FoodData>().destroyParent)
                Destroy(target.transform.root.gameObject);
            else
                Destroy(target);
        }
    }


    //Gain nutritional value from the food
    private void Digest(GameObject food)
    {
        float foodValue = food.GetComponent<FoodData>().nutritionalValue;
        hungerUnits -= foodValue;
        cData.energyUnits += foodValue;
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
            return true;
        else return false;
    }


    //hungerIndex > 90%, starving
    private void WasteAway()
    {
        if (hungerPercentage >= 1)
            GetComponent<Vitality>().Die();
    }
    #endregion
}
