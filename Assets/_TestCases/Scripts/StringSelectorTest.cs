using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerEditor;

[System.Serializable]
public class Element
{
    public string key;
    public int value;
}

[System.Serializable]
public class InternalStringSelector
{
    [PowerList(Key = "key")]
    public List<Element> elements = new List<Element>();

    [ElementSelector(ListGetter = "GetElementList", DisplayKey = "value", ReferenceKey = "key")]
    [ColorTheme("Dark")]
    public string elementSelector;

    List<Element> GetElementList()
    {
        return elements;
    }
}


public class StringSelectorTest : MonoBehaviour
{

    [Header("String Selector"), PowerList]
    public List<string> stringList = new List<string>();
    [StringSelector(ListGetter = "GetStringList")]
    [ColorTheme("Dark")]
    public string stringSelector;

    [Header("Element Selector"), PowerList(Key ="key")]
    public List<Element> elements = new List<Element>();
    [ElementSelector(ListGetter = "GetElementList", ReferenceKey = "value", DisplayKey = "key")]
    [ColorTheme("Dark")]
    public string elementSelector;

    [Header("Internal Element Selector")]
    public InternalStringSelector internalStringSelector = new InternalStringSelector();



    List<string> GetStringList()
    {
        return stringList;
    }


    List<Element> GetElementList()
    {
        return elements;
    }
}
