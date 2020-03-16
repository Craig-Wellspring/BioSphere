using UnityEngine;
using System.Collections.Generic;


public class Metabolism : MonoBehaviour
{
    [Header("Creature Diet")]
    public List<string> dietList;
    
    [Header("Current Hunger")]
    [Tooltip("How Hungry the Creature currently feels")]
    public float hungerLevel = 0f;
    public int hungerIndex;
    [Header("Hunger Index Settings")]
    [Tooltip("Minimum Hunger level")]
    public float fullySatiatedThreshold = 0f;
    public float veryFullThreshold = 20f;
    public float satisfiedThreshold = 40f;
    public float hungryThreshold = 60f;
    public float famishedThreshold = 80f;
    public float starvingThreshold = 100f;

    [Space(10)]
    [Tooltip("Time in Seconds it takes to gain one unit of Hunger")]
    public float metabolismRate = 1f;
    [Tooltip("Units of Hunger gained per tick")]
    public float hungerGainedPerTick = 1f;

    private float timeCounter = 0f;

    private void Update()
    {
        timeCounter += Time.deltaTime;
        if (timeCounter >= metabolismRate)
        {
            timeCounter = 0f;
            //Become a little more hungry if not Wasting
            if (hungerIndex < 6)
                hungerLevel += hungerGainedPerTick;
        }

        //Fully Satiated
        if (hungerLevel <= fullySatiatedThreshold)
            hungerIndex = 0;
        //Very Full
        else if (hungerLevel > fullySatiatedThreshold && hungerLevel <= veryFullThreshold)
            hungerIndex = 1;
        //Satisfied
        else if (hungerLevel > veryFullThreshold && hungerLevel <= satisfiedThreshold)
            hungerIndex = 2;
        //Hungry
        else if (hungerLevel > satisfiedThreshold && hungerLevel <= hungryThreshold)
            hungerIndex = 3;
        //Famished
        else if (hungerLevel > hungryThreshold && hungerLevel <= famishedThreshold)
            hungerIndex = 4;
        //Starving
        else if (hungerLevel > famishedThreshold && hungerLevel <= starvingThreshold)
            hungerIndex = 5;
        //Wasting
        else if (hungerLevel > starvingThreshold)
            hungerIndex = 6;

        if (hungerIndex == 6)
        {
            //Waste away (shrink in size)
            GetComponent<Vitality>().Die();
        }
    }


    public void Ingest(GameObject food)
    {
        //Gain nutritional value of the food
        hungerLevel -= food.transform.root.GetComponentInChildren<Food>().nutritionalValue;
    }
}
