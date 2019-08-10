using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using PowerInspector.Editor;
#endif
using PowerInspector;

[ColorTheme("Dark")]
[PresetData]
public class UserPresetDataAttribute : CompositedPowerPropertyAttribute
{
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(UserPresetDataAttribute))]
public class UserPresetDataAttributeDrawer : CompositedPowerPropertyDrawer
{
}
#endif


