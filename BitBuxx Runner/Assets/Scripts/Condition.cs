using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System; 


[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class Condition : PropertyAttribute
{
    public string ConditionBoolean = "";
    public bool Hidden = false;

    public Condition(string conditionBoolean)
    {
        this.ConditionBoolean = conditionBoolean;
        this.Hidden = false;
    }

    public Condition(string conditionBoolean, bool hideInInspector)
    {
        this.ConditionBoolean = conditionBoolean;
        this.Hidden = hideInInspector;
    }
}
