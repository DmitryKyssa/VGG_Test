using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public class ReadOnlyAttribute : PropertyAttribute
{
}