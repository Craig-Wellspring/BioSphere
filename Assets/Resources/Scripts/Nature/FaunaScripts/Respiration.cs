using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;

public class Respiration : MonoBehaviour
{
    [Header("State")]
    public int currentStamina = 100;

    [Space(10)]
    public bool canBreathe = true;
    public bool isExhausted = false;
    public bool isSprinting = false;

    [Header("Settings")]
    public int maxStamina = 100;
    public int stamIncreasePerLevel = 10;
    [Tooltip("Can breathe under water.")]
    public bool amphibious = false;

    float breathingRate = 0.250f;
    int staminaIncrement = 1;
    int baseStamina = 100;
    int sprintMod = 2;
    bool depleted = false;


    // Cache
    Slider staminaBar;

    AIPathAlignedToSurface pathing;
    Vitality vitality;
    CreatureStats cStats;



    void Start()
    {
        pathing = transform.root.GetComponent<AIPathAlignedToSurface>();
        vitality = GetComponent<Vitality>();
        cStats = GetComponent<CreatureStats>();

        staminaBar = transform.root.Find("Canvas").Find("Stamina Bar").GetComponent<Slider>();
        currentStamina = maxStamina;
        UpdateStaminaBar();

        // Register Max Stamina in StatBlock
        GetComponent<CreatureStats>()?.AddNewStat("Stamina", maxStamina, stamIncreasePerLevel);

        // Start Breathing
        InvokeRepeating("Breathe", breathingRate, breathingRate);
    }


    void Breathe()
    {
        if (isSprinting)
            ChangeCurrentStamina(-staminaIncrement * sprintMod);

        if (canBreathe)
        {
            // Gain Stamina
            if (currentStamina < maxStamina && !isSprinting)
                ChangeCurrentStamina(staminaIncrement);
        }
        else
        {
            // Lose Stamina until it is depleted, then use HP
            if (currentStamina > 0)
                ChangeCurrentStamina(-staminaIncrement);
            else
                vitality.TakeDamage(staminaIncrement);
        }
    }


    public void ChangeMaxStamina(int _amount)
    {
        maxStamina += _amount;
        if (staminaBar != null)
            staminaBar.maxValue = maxStamina;

        ChangeCurrentStamina(_amount);
    }

    void ChangeCurrentStamina(int _amount = 0)
    {
        currentStamina += _amount;

        if (staminaBar != null)
            UpdateStaminaBar();

        StaminaQuery();
    }

    void UpdateStaminaBar()
    {
        staminaBar.value = currentStamina;

        // Show stamina bar if not at Max
        staminaBar.gameObject.SetActive(!currentStamina.Equals(maxStamina));
    }

    void StaminaQuery()
    {
        if (!depleted)
        {
            isExhausted = currentStamina > 0 ? false : true;

            if (isExhausted)
            {
                // Become Exhausted
                ToggleSprinting(false);
                depleted = true;
            }
        }
        else
        {
            if (currentStamina >= baseStamina)
                depleted = false;
        }
    }


    float _speedMod;
    public void ToggleSprinting(bool _start)
    {
        if (_start)
        {
            if (!isExhausted)
            {
                _speedMod = cStats.GetStat("Speed").baseValue;
                pathing.maxSpeed += _speedMod;
                isSprinting = true;
            }      
            // else tell UI low stamina
        }
        else
        {
            pathing.maxSpeed -= _speedMod;
            _speedMod = 0;
            isSprinting = false;
        }
    }
}
