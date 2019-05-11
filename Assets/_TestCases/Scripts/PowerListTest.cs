using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerEditor;


public class PowerListTest : MonoBehaviour
{
    [PowerList(Key ="key"),ColorTheme("User")]
    public List<Element> powerList = new List<Element>();

}
