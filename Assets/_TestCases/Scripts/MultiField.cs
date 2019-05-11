using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerEditor;

[System.Serializable]
public class CompositedFields
{
    public int integer1 = 10;
    public float float1 = 5.1f;
    public int integer2 = 10;
    public float float2 = 3.14f;
    public int integer3 = 10;
    public Fields fields = new Fields();
}

[System.Serializable]
public class Fields
{
    //[ColorTheme(fontColor = "#FF0000", backgroundColor = "#00FF00", runtimeBackgroundColor = "(,,255)", runtimeFontColor = "(12,56,167)")]
    public int integer1 = 10;
    public float float1 = 5.1f;
    public int integer2 = 10;
    public float float2 = 3.14f;
    public int integer3 = 10;
}



public enum Week
{
    Sunday = 0,
    Monday,
    Friday
}

public class MultiField : MonoBehaviour
{
    public CompositedFields compositeFields = new CompositedFields();
    [RuntimeData]
    public int readOnly = 10;
    [ColorTheme("Dark")]
    [PresetData]
    public float presetData = 10;
    [Header("Header"), RuntimeData]
    public double runtimeData = 10;

    [ColorTheme(fontColor = "#FFFFFF", backgroundColor = "#000000", runtimeBackgroundColor = "(255,255,255)", runtimeFontColor = "(0,0,0)")]
    public Week mixColor = Week.Friday;

    [ColorTheme("Dark")]
    public Week userColor = Week.Friday;

    [Header("Condition")]
    public bool condition;
    [HideWhenTrue(condition = "condition")]
    public float hideWhenTrue;
    [HideWhenFalse(condition = "condition")]
    public float hideWhenFalse;


}
