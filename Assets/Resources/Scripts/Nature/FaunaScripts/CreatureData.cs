using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;
using System.Collections.ObjectModel;


[ExecuteInEditMode]
public class CreatureData : MonoBehaviour
{
    [Header("Creature Stats")]
    public int currentLevel = 1;
    [Space(10)]
    public CreatureStat maxHealth, speed, perception, metabolismRate;
    [HideInInspector]
    public enum TargetCreatureStat { MaxHealth, Speed, Perception, MetabolismRate };
    [HideInInspector] public TargetCreatureStat targetCreatureStat;


    [Header("Debug")]
    [SerializeField] bool logLevelUp = false;


    [Header("Body Parts")]
    [Tooltip("The Creature's primary body collider which must be on the same layer as the root object")]
    public Collider mainBodyCollider;
    [Tooltip("The Creature's additional body colliders which must be on the CreatureAdl layer")]
    public List<Collider> adlBodyColliders;
    [Tooltip("GameObject that represents the Creature's corpse")]
    public GameObject corpse;



    // Cache
    AIDestinationSetter destinationSetter;
    AIPath aiPath;
    Seeker seeker;

    void Start()
    {
        destinationSetter = transform.root.GetComponent<AIDestinationSetter>();
        aiPath = transform.root.GetComponent<AIPathAlignedToSurface>();
        seeker = transform.root.GetComponent<Seeker>();

        RegisterBodyColliders();
        PullStatsFromOrigin();
    }

    void RegisterBodyColliders()
    {
        // Clear cache
        mainBodyCollider = null;
        adlBodyColliders = new List<Collider>();

        // Cache colliders
        Collider[] allBodyColliders = transform.root.GetComponentsInChildren<Collider>(false);
        foreach (Collider _col in allBodyColliders)
        {
            if (_col.gameObject.layer == LayerMask.NameToLayer("CreatureAdl"))
            {
                adlBodyColliders.Add(_col);
                continue;
            }
            if (_col.gameObject.layer == transform.root.gameObject.layer)
                mainBodyCollider = _col;
        }

        // Cache corpse
        corpse = transform.root.Find("Corpse").gameObject;
    }


    public event System.Action LevelUpBeginning;
    public event System.Action LevelUpFinishing;
    public void LevelUp()
    {
        // Trigger beginning events 
        LevelUpBeginning?.Invoke();
        // - AI decides which stat to increase

        // Level up
        IncreaseLevel();

        // Increase chosen stat
        IncreaseStat(targetCreatureStat);

        //Trigger ending events
        LevelUpFinishing?.Invoke();


        // Debug
        if (logLevelUp)
            Debug.Log(transform.root.name + " evolved to level " + currentLevel + " and increased its " + targetCreatureStat + " to " + CurrentTargetStat().baseValue);
    }

    public void IncreaseLevel(int _increment = 1)
    {
        currentLevel += _increment;
    }
    
    public void IncreaseStat(TargetCreatureStat _stat, float _increment = 0.5f)
    {
        switch (_stat)
        {
            // Max Health
            case TargetCreatureStat.MaxHealth:
                GetComponent<Vitality>().IncreaseMaxHealth((int)(_increment * 2));
                maxHealth.IncreaseStat((int)(_increment * 2));
                break;

            // Speed
            case TargetCreatureStat.Speed:
                transform.root.GetComponent<Pathfinding.AIPathAlignedToSurface>().maxSpeed += _increment;
                speed.IncreaseStat(_increment);
                break;

            // Perception
            case TargetCreatureStat.Perception:
                GetComponent<VisualPerception>().sightRadius += _increment;
                perception.IncreaseStat(_increment);
                break;

            // Metabolism Rate
            case TargetCreatureStat.MetabolismRate:
                GetComponent<Metabolism>().metabolismRate += _increment;
                metabolismRate.IncreaseStat(_increment);
                break;
        }
    }

    public void PullStatsFromOrigin()
    {
        // Pull MaxHealth value from origin values to stat block
        Vitality vitality = GetComponent<Vitality>();
        if (vitality)
            maxHealth.baseValue = vitality.maxHealth;

        // Pull Speed value from origin values to stat block
        speed.baseValue = transform.root.GetComponent<AIPathAlignedToSurface>().maxSpeed;

        // Pull Perception value from origin values to stat block
        VisualPerception vPerception = GetComponent<VisualPerception>();
        if (vPerception)
            perception.baseValue = vPerception.sightRadius;

        // Pull MetabolismRate value from origin values to stat block
        Metabolism metabolism = GetComponent<Metabolism>();
        if (metabolism)
            metabolismRate.baseValue = metabolism.metabolismRate;
    }

    public void PushStatsToOrigin()
    {
        // Push MaxHealth from stat block to origin
        Vitality vitality = GetComponent<Vitality>();
        if (vitality)
            vitality.maxHealth = maxHealth.baseValue;

        // Push Speed from stat block to origin
        transform.root.GetComponent<AIPathAlignedToSurface>().maxSpeed = speed.baseValue;

        // Push Perception from stat block to origin
        VisualPerception vPerception = GetComponent<VisualPerception>();
        if (vPerception)
            vPerception.sightRadius = perception.baseValue;

        // Push MetabolismRate from stat block to origin
        Metabolism metabolism = GetComponent<Metabolism>();
        if (metabolism)
            metabolism.metabolismRate = metabolismRate.baseValue;
    }

    public CreatureStat CurrentTargetStat()
    {
        CreatureStat _targetStat = null;

        switch(targetCreatureStat)
        {
            // Max Health
            case TargetCreatureStat.MaxHealth:
                _targetStat = this.maxHealth;
                break;

            // Speed
            case TargetCreatureStat.Speed:
                _targetStat = this.speed;
                break;

            // Perception
            case TargetCreatureStat.Perception:
                _targetStat = this.perception;
                break;

            // Metabolism Rate
            case TargetCreatureStat.MetabolismRate:
                _targetStat = this.metabolismRate;
                break;
        }
        return _targetStat;
    }

    public void CopyCData(CreatureData _targetCData)
    {
        // Match current level
        _targetCData.currentLevel = currentLevel;

        // Match current stats
        foreach (CreatureData.TargetCreatureStat stat in Enum.GetValues(typeof(CreatureData.TargetCreatureStat)))
        {
            _targetCData.targetCreatureStat = stat;
            targetCreatureStat = stat;
            _targetCData.CurrentTargetStat().baseValue = CurrentTargetStat().baseValue;
        }
        _targetCData.PushStatsToOrigin();
    }

    public void ClearPathing()
    {
        destinationSetter.target = null;
        aiPath.SetPath(null);
        aiPath.destination = Vector3.positiveInfinity;
        seeker.CancelCurrentPathRequest();
    }
}



//// Individual Stats \\\\
[Serializable]
public class CreatureStat
{
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

    public CreatureStat(float _baseValue) : this()
    {
        baseValue = _baseValue;
    }

    public virtual void SetStat(float _value)
    {
        baseValue = _value;
    }

    public virtual void IncreaseStat(float _value)
    {
        baseValue += _value;
    }
    public virtual void DecreaseStat(float _value)
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

