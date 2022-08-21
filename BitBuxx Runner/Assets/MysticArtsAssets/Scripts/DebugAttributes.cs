using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugAttributes : MonoBehaviour
{
    [AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false)]
    public class MMDebugLogCommandAttribute : System.Attribute { }
}
