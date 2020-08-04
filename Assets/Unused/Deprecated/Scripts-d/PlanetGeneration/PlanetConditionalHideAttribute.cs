using System.Collections;
using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
    AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class PlanetConditionalHideAttribute : PropertyAttribute
{
    public string conditionalSourceField;
    public int enumIndex;

    public PlanetConditionalHideAttribute()
    {
    }

    public PlanetConditionalHideAttribute(string boolVariableName)
    {
        conditionalSourceField = boolVariableName;
    }

    public PlanetConditionalHideAttribute(string enumVariableName, int enumIndex)
    {
        conditionalSourceField = enumVariableName;
        this.enumIndex = enumIndex;
    }

}
