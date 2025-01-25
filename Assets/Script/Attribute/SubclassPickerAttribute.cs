using System;
using UnityEngine;

namespace Mekaiju.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class SubclassPickerAttribute : PropertyAttribute 
    { 

    }
}