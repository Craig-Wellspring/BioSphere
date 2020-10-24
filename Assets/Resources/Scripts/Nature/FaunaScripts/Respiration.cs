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
    public int stamIncrement = 10;
    [SerializeField] float breathRate = 0.2f;
    int staminaIncrement = 1;
    int sprintMod = 3;
    float speedMod;


    // Cache
    Slider staminaBar;
    AIPathAlignedToSurface pathing;
    CreatureStats cStats;
    [Header("Debug")]
    public CreatureStat metabolismStat;


    void Start()
    {
        currentStamina = maxStamina;

        pathing = transform.root.GetComponent<AIPathAlignedToSurface>();
        cStats = GetComponent<CreatureStats>();
        if (GetComponent<Metabolism>())
        {
            foreach (CreatureStat _stat in cStats.statBlock)
                if (_stat.id.Equals("Metabolism"))
                {
                    metabolismStat = _stat;
                    break;
                }
        }

        staminaBar = transform.root.Find("Canvas").Find("Stamina Bar").GetComponent<Slider>();
        UpdateStaminaBar();

        // Register Max Stamina in StatBlock
        GetComponent<CreatureStats>()?.AddNewStat("Stamina", maxStamina, stamIncrement);

        // Start breathing
        InvokeRepeating("Breathe", breathRate, breathRate);
    }

    void UpdateStaminaBar()
    {
        staminaBar.value = currentStamina;

        // Show stamina bar if not at Max
        staminaBar.gameObject.SetActive(!currentStamina.Equals(maxStamina));
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
        if (currentStamina < maxStamina)
            currentStamina += _amount;

        if (staminaBar != null)
            UpdateStaminaBar();

        StaminaQuery();
    }


    bool StaminaQuery()
    {
        bool _exhaustion = currentStamina >= 0 ? false : true;

        // Toggle Exhaustion
        if (isExhausted != _exhaustion)
        {
            isExhausted = _exhaustion;
            if (isExhausted) // Become Exhausted
            {
                metabolismStat.AddModifier(new StatModifier(-50, StatModType.PercentAdd, 1, this));
            }
            else // Become Refreshed
            {
                metabolismStat.RemoveAllModifiersFromSource(this);
            }
        }

        return _exhaustion;
    }

    void Breathe()
    {
        if (canBreathe)
            ChangeCurrentStamina(staminaIncrement);
        else ChangeCurrentStamina(-staminaIncrement);
    }

    public void ToggleSprinting(bool _start)
    {
        if (_start && !isExhausted)
        {
            speedMod = pathing.maxSpeed;
            pathing.maxSpeed += speedMod;
            StartCoroutine("Sprinting");
        }
        else
        {
            pathing.maxSpeed -= speedMod;
            speedMod = 0;
            StopCoroutine("Sprinting");
        }
    }

    IEnumerator Sprinting()
    {
        for (; ; )
        {
            ChangeCurrentStamina(-staminaIncrement * sprintMod);
            yield return new WaitForSeconds(breathRate);
        }
    }
}
