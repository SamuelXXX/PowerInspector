using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using PowerEditor.Editor;

[System.Serializable]
public class Class1
{
    public int v1;
    public int v2;
    public Class2 classType;

    public void Call1()
    {
        Debug.Log("Class1 called!");
    }
}

[System.Serializable]
public class Class2
{
    public int v1;
    public int v2;
    public Class3 classType;

    public void Call2()
    {
        Debug.Log("Class2 called!");
    }
}

[System.Serializable]
public class Class3
{
    public int v1;
    public int v2;

    public void Call3()
    {
        Debug.Log("Class3 called!");
    }
}

public class ReflectionTest : MonoBehaviour
{
    public Class1 class1 = new Class1();

    public string valuePath = "class1.classType.v1";
    public string methodPath = "class1.classType.classType.Call3";

    [ContextMenu("GetReflectionValue")]
    void GetReflectionValue()
    {
        object v = PowerEditorUtility.GetFieldObjectRecursively(this, valuePath);
        if (v != null)
        {
            Debug.Log($"Check succeed!{v.ToString()}");
        }
        else
        {
            Debug.LogError($"Check failed!");
        }
    }

    [ContextMenu("ReflectionMethodCall")]
    void ReflectionMethodCall()
    {
        PowerEditorUtility.InvokeMethodRecursively(this, methodPath);
    }

}


#endif


