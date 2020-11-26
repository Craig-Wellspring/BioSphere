using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public bool isActive = true;

    [SerializeField, Range(0, 3)] float baseStatAsDmg = 1f;

    public DamageStat damageStat;
    public enum DamageStat { Strength, Dexterity, Intellect }


    [Header("Debug")]
    [SerializeField] bool logAttacks = false;


    // Cache
    CreatureStats cStats;



    void Start()
    {
        // Register applicable stat in stat block if it does not exist already
        cStats = transform.root.GetComponentInChildren<CreatureStats>();

        if (cStats)
        {
            switch (damageStat)
            {
                case (DamageStat.Strength):
                    if (cStats.GetStat("Strength") == null)
                        cStats.AddNewStat("Strength", 1, 1);
                    break;

                case (DamageStat.Dexterity):
                    if (cStats.GetStat("Dexterity") == null)
                        cStats.AddNewStat("Dexterity", 1, 1);
                    break;

                case (DamageStat.Intellect):
                    if (cStats.GetStat("Intellect") == null)
                        cStats.AddNewStat("Intellect", 1, 1);
                    break;
            }
        }
    }

    protected void DamageVitality(Transform _targetRoot)
    {
        Vitality targetVitality = _targetRoot.GetComponentInChildren<Vitality>();
        if (targetVitality)
        {
            int damage = Mathf.RoundToInt((cStats.GetStat(damageStat.ToString()).value) * baseStatAsDmg);
            targetVitality.TakeDamage(damage);

            if (logAttacks)
                Debug.Log(transform.root.name + " struck " + _targetRoot.name + " with an attack for " + damage + " damage.");
        }
    }
}
