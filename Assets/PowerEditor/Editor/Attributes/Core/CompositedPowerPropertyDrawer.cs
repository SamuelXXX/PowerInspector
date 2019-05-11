using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEditor;
using System.Linq;
using System;

namespace PowerEditor.Editor
{
    [CustomPropertyDrawer(typeof(CompositedPowerPropertyAttribute))]
    public class CompositedPowerPropertyDrawer : PowerPropertyDrawer
    {
    }
}