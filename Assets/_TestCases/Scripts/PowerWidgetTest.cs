using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerEditor;

public class PowerWidgetTest : MonoBehaviour
{
    public bool condition;
    [ColorTheme("Dark")]
    public PowerSpaceField powerSpaceField = new PowerSpaceField(100);
    [HideWhenTrue(condition ="condition")]
    public PowerTextureField powerTextureField = new PowerTextureField("logo",0.4f,ScaleMode.ScaleToFit);
    [ColorTheme("Dark")]
    public PowerButton powerButton = new PowerButton("Button", "ButtonCallback",40f);


    void ButtonCallback()
    {
        Debug.LogFormat("Power Button Pressed!{0},{1}","1","2");
    }
}
