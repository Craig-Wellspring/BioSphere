using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;
using System.Collections.ObjectModel;


[ExecuteAlways]
public class CreatureStats : MonoBehaviour
{
    [Header("Creature Stats")]
    public int currentLevel = 1;
    [Space(15)]
    public List<CreatureStat> statBlock = new List<CreatureStat>();


    [Header("Debug")]
    [SerializeField] bool logLevelUp = false;
    [Space(10)]
    public int unappliedLevels = 0;



    void Start()
    {
        // Initialize Stats
        statBlock = new List<CreatureStat>();
        GenerateStatBlock();
    }

    public void CopyCData(CreatureStats _targetCStats)
    {
        _targetCStats.currentLevel = currentLevel;
        _targetCStats.statBlock = statBlock;

        _targetCStats.PushOrPullOriginStats(true);
    }


    #region Level Up
    public event System.Action LevelUpBeginning;
    public void TriggerLevelUp()
    {
        unappliedLevels++;

        // Trigger beginning events 
        LevelUpBeginning?.Invoke();
        // - AI confirms LevelUp with randomly chosen stat

        // - (Ding effect)
        // - (UI indicates unappliedLevels to Player)
        // - (LevelUp UI asks the Player to choose a stat to increase and provides LevelUpConfirm button)
    }

    public event System.Action LevelUpFinishing;
    public void ConfirmLevelUp(CreatureStat _statToIncrease, int _increment = 1)
    {
        unappliedLevels--;

        // Level up
        IncreaseLevel();

        // Increase chosen stat
        if (_statToIncrease.id.Contains("Health"))
            _increment *= 2;

        _statToIncrease.IncreaseStat(_increment);
        PushOrPullOriginStats(true);

        //Trigger ending events
        LevelUpFinishing?.Invoke();
        // - morphology.AttemptTransMorph();

        // Debug
        if (logLevelUp)
            Debug.Log(transform.root.name + " advanced to level " + currentLevel + " and increased its " + _statToIncrease.id + " to " + _statToIncrease.baseValue);
    }

    public void IncreaseLevel(int _increment = 1)
    {
        currentLevel += _increment;
    }
    #endregion

    #region Stat Allocation
    void GenerateStatBlock()
    {
        // Add Speed Stat if AIPath exists
        AIPathAlignedToSurface pathing = transform.root.GetComponent<AIPathAlignedToSurface>();
        if (pathing)
            statBlock.Add(new CreatureStat("Speed", pathing.maxSpeed));

        // Add Health Stat if Vitality exists
        Vitality vitality = GetComponent<Vitality>();
        if (vitality)
            statBlock.Add(new CreatureStat("Health", vitality.maxHealth));

        // Add Perception Stat if VPerception exists
        VisualPerception vPerception = GetComponent<VisualPerception>();
        if (vPerception)
            statBlock.Add(new CreatureStat("Perception", vPerception.sightRadius));

        // Add Metabolism Stat if Metabolism exists
        Metabolism metabolism = GetComponent<Metabolism>();
        if (metabolism)
            statBlock.Add(new CreatureStat("Metabolism", metabolism.metabolismRate));
    }

    

    // Push or Pull Origin Stats. If _push, set stats in creature equal to current stats in statBlock. If !_push, set stats in statBlock equal to current stats in creature
    public void PushOrPullOriginStats(bool _push)
    {
        foreach (CreatureStat _stat in statBlock)
        {
            switch (_stat.id)
            {
                case "Speed":
                    if (_push)
                        transform.root.GetComponent<AIPathAlignedToSurface>().maxSpeed = _stat.baseValue;
                    else
                        _stat.baseValue = transform.root.GetComponent<AIPathAlignedToSurface>().maxSpeed;
                    break;

                case "Health":
                    if (_push)
                        GetComponent<Vitality>().IncreaseMaxHealth(Mathf.RoundToInt(_stat.baseValue - GetComponent<Vitality>().maxHealth));
                    else
                        _stat.baseValue = GetComponent<Vitality>().maxHealth;
                    break;

                case "Perception":
                    if (_push)
                        GetComponent<VisualPerception>().sightRadius = _stat.baseValue;
                    else
                        _stat.baseValue = GetComponent<VisualPerception>().sightRadius;
                    break;

                case "Metabolism":
                    if (_push)
                        GetComponent<Metabolism>().metabolismRate = _stat.baseValue;
                    else
                        _stat.baseValue = GetComponent<Metabolism>().metabolismRate;
                    break;
            }
        }
    }
    #endregion
}



#region Individual Stats
[Serializable]
public class CreatureStat
{
    [HideInInspector] public string id;
    public float baseValue;

    protected bool isDirty = true;
    protected float lastBaseValue;

    protected float _value;
    public virtual float value
    {
        get
        {
            if (isDirty || baseValue != lastBaseValue)
            {
                lastBaseValue = baseValue;
                _value = CalculateFinalValue();
                isDirty = false;
            }
            return _value;
        }
    }


    protected readonly List<StatModifier> statModifiers;
    public readonly ReadOnlyCollection<StatModifier> StatModifiers;

    public CreatureStat()
    {
        statModifiers = new List<StatModifier>();
        StatModifiers = statModifiers.AsReadOnly();
    }

    public CreatureStat(string _id, float _baseValue) : this()
    {
        id = _id;
        baseValue = _baseValue;
    }

    public virtual void SetStat(float _value)
    {
        baseValue = _value;
    }

    public virtual void IncreaseStat(float _value = 1)
    {
        baseValue += _value;
    }
    public virtual void DecreaseStat(float _value = 1)
    {
        baseValue -= _value;
    }

    public virtual void AddModifier(StatModifier _mod)
    {
        isDirty = true;
        statModifiers.Add(_mod);
    }


    public virtual bool RemoveModifier(StatModifier _mod)
    {
        if (statModifiers.Remove(_mod))
        {
            isDirty = true;
            return true;
        }
        return false;
    }

    public virtual bool RemoveAllModifiersFromSource(object _source)
    {
        int numRemovals = statModifiers.RemoveAll(mod => mod.source == _source);

        if (numRemovals > 0)
        {
            isDirty = true;
            return true;
        }
        return false;
    }

    protected virtual int CompareModifierOrder(StatModifier _a, StatModifier _b)
    {
        if (_a.order < _b.order)
            return -1;
        else if (_a.order > _b.order)
            return 1;
        return 0; // if (_a.order == _b.order)
    }

    protected virtual float CalculateFinalValue()
    {
        float finalValue = baseValue;
        float sumPercentAdd = 0;

        statModifiers.Sort(CompareModifierOrder);

        for (int i = 0; i < statModifiers.Count; i++)
        {
            StatModifier mod = statModifiers[i];

            if (mod.type == StatModType.Flat)
            {
                finalValue += mod.value;
            }
            else if (mod.type == StatModType.PercentAdd)
            {
                sumPercentAdd += mod.value;

                if (i + 1 >= statModifiers.Count || statModifiers[i + 1].type != StatModType.PercentAdd)
                {
                    finalValue *= 1 + sumPercentAdd;
                    sumPercentAdd = 0;
                }
            }
            else if (mod.type == StatModType.PercentMult)
            {
                finalValue *= 1 + mod.value;
            }

        }

        return (float)Math.Round(finalValue, 4);
    }
}



//// Stat Modifiers \\\\
public enum StatModType
{
    Flat = 100,
    PercentAdd = 200,
    PercentMult = 300
}

public class StatModifier
{
    public readonly float value;
    public readonly StatModType type;
    public readonly int order;
    public readonly object source;


    public StatModifier(float _value, StatModType _type, int _order, object _source)
    {
        value = _value;
        type = _type;
        order = _order;
        source = _source;
    }

    public StatModifier(float _value, StatModType _type) : this(_value, _type, (int)_type, null) { }
    public StatModifier(float _value, StatModType _type, int order) : this(_value, _type, order, null) { }
    public StatModifier(float _value, StatModType _type, object source) : this(_value, _type, (int)_type, source) { }

}
#endregion

