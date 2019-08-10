using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEditor;
using System.Linq;
using System;
using UnityEditorInternal;
using System.Text.RegularExpressions;

namespace PowerInspector.Editor
{
    public delegate float CustomDrawer(Rect position, SerializedProperty property, GUIContent label);

    /// <summary>
    /// Powerful editing option to define editor appearance
    /// </summary>
    public class PowerEditingOption
    {
        #region Option Data
        public Color fontColor;
        public Color backgroundColor;

        public bool readOnly;
        public bool visible;

        public CustomDrawer overrideDrawer;
        public float heightBeforeDraw;//Assigned height before draw, this value will be assigned to 'Rect' parameter and work for UI like GUI.Button
        public float heightAfterDraw;//Calculated height after draw, this value will be used to GetPropertyHeight
        #endregion

        #region Runtime data
        bool mainColorThemeAssigned = false;
        public bool firstFrameDrawn = false;
        #endregion

        public PowerEditingOption()
        {
            ResetOption();
        }

        public void ResetOption()
        {
            fontColor = GUI.contentColor;
            backgroundColor = new Color(1, 1, 1, 0);
            readOnly = false;
            visible = true;

            heightBeforeDraw = EditorGUIUtility.singleLineHeight * 1.5f;
            heightAfterDraw = 0;
            overrideDrawer = null;
            mainColorThemeAssigned = false;
        }


        public void ModifyOptionIncrement(PowerPropertyAttribute ppa, SerializedObject serializedObject, SerializedProperty property, FieldInfo fieldInfo)
        {
            #region Color Modify
            if (!mainColorThemeAssigned)
            {
                ColorThemePreset.ColorTheme colorTheme = ColorThemePreset.Instance.GetColorTheme(ppa.theme);
                //Solve base color settings
                if (colorTheme == null)
                {
                    fontColor = GUI.contentColor;
                    backgroundColor = new Color(1, 1, 1, 0);
                }
                else
                {
                    if (!Application.isPlaying)
                    {
                        fontColor = colorTheme.fontColor;
                        backgroundColor = colorTheme.backgroundColor;
                    }
                    else
                    {
                        fontColor = colorTheme.runtimeFontColor;
                        backgroundColor = colorTheme.runtimeBackgroundColor;
                    }
                }
            }

            if (ppa is ColorThemeAttribute)
            {
                ColorThemeAttribute ct = ppa as ColorThemeAttribute;

                //Additional modify color settings
                if (!Application.isPlaying)
                {
                    if (PowerInspectorUtility.CheckColorCodeValid(ct.fontColor))
                    {
                        fontColor = PowerInspectorUtility.ColorCode2Color(ct.fontColor);
                    }

                    if (PowerInspectorUtility.CheckColorCodeValid(ct.backgroundColor))
                    {
                        backgroundColor = PowerInspectorUtility.ColorCode2Color(ct.backgroundColor);
                    }
                }
                else
                {
                    if (PowerInspectorUtility.CheckColorCodeValid(ct.runtimeFontColor))
                    {
                        fontColor = PowerInspectorUtility.ColorCode2Color(ct.runtimeFontColor);
                    }

                    if (PowerInspectorUtility.CheckColorCodeValid(ct.runtimeBackgroundColor))
                    {
                        backgroundColor = PowerInspectorUtility.ColorCode2Color(ct.runtimeBackgroundColor);
                    }
                }

                mainColorThemeAssigned = true;
            }
            #endregion

            #region Access Level Modify
            if (ppa is PresetDataAttribute)
            {
                if (Application.isPlaying)
                {
                    readOnly = true;
                }
                else
                {
                    readOnly = false;
                }
            }

            if (ppa is RuntimeDataAttribute)
            {
                if (Application.isPlaying)
                {
                    readOnly = true;
                    visible = true;
                }
                else
                {
                    visible = false;
                }
            }

            if (ppa is HideWhenTrueAttribute)
            {
                HideWhenTrueAttribute hwt = ppa as HideWhenTrueAttribute;

                SerializedProperty sp = PowerInspectorUtility.GetSiblingProperty(serializedObject, property, hwt.condition);
                if (sp == null || sp.propertyType != SerializedPropertyType.Boolean)
                {
                    Debug.LogWarningFormat("Boolean-typed Property '{0}' not found in class '{1}'", hwt.condition, serializedObject.targetObject.GetType().Name);
                }
                else
                {
                    visible = !sp.boolValue;
                }
            }

            if (ppa is HideWhenFalseAttribute)
            {
                HideWhenFalseAttribute hwt = ppa as HideWhenFalseAttribute;
                SerializedProperty sp = PowerInspectorUtility.GetSiblingProperty(serializedObject, property, hwt.condition);
                if (sp == null || sp.propertyType != SerializedPropertyType.Boolean)
                {
                    Debug.LogWarningFormat("Boolean-typed Property '{0}' not found in class '{1}'", hwt.condition, serializedObject.targetObject.GetType().Name);
                }
                else
                {
                    visible = sp.boolValue;
                }
            }

            if (ppa is ReadOnlyAttribute)
            {
                readOnly = true;
            }
            #endregion

            #region Custom Drawer Modify
            if (ppa is PowerListAttribute)
            {
                heightBeforeDraw = 0f;

                string data = PowerInspectorUtility.GetMemberName(property.propertyPath);
                string listPath = "";

                Regex elementPattern = new Regex(@"data\[\d+\]");
                if (elementPattern.IsMatch(data))//PowerList works on list only
                {
                    data = data.Replace("data[", "");
                    data = data.Replace("]", "");
                    int index = int.Parse(data);

                    listPath = PowerInspectorUtility.GetPrefixPath(property.propertyPath);
                    listPath = listPath.Substring(0, listPath.Length - 1);

                    listPath = PowerInspectorUtility.GetPrefixPath(listPath);
                    listPath = listPath.Substring(0, listPath.Length - 1);
                    if (index == 0)//Only draw in first element
                    {
                        if (overrideDrawer == null)
                            overrideDrawer = (r, p, l) =>
                            {
                                return PowerListDrawerManager.Instance.DrawPowerList(r, p.serializedObject.targetObject, listPath, (ppa as PowerListAttribute).Key);
                            };
                    }
                    else
                    {
                        visible = false;
                    }
                }
                else
                {
                    if (overrideDrawer == null)
                        overrideDrawer = (r, p, l) =>
                        {
                            return PowerInspectorUtility.DrawErrorMessage(r,
                                p.displayName + ":'PowerListAttribute' should not be applied to non-list type field!",
                                (float)(1.5 * EditorGUIUtility.singleLineHeight));
                        };
                }
            }

            if (ppa is StringSelectorAttribute)
            {
                if (fieldInfo.FieldType != typeof(string))
                {
                    if (overrideDrawer == null)
                        overrideDrawer = (r, p, l) =>
                        {
                            return PowerInspectorUtility.DrawErrorMessage(r,
                                p.displayName + ":'StringSelectorAttribute' should not be applied to non-string type property!",
                                (float)(1.5 * EditorGUIUtility.singleLineHeight));
                        };
                }
                else if (overrideDrawer == null)
                {
                    StringSelectorAttribute ss = ppa as StringSelectorAttribute;
                    string listGetterMethodPath = PowerInspectorUtility.GetPrefixPath(property.propertyPath) + ss.ListGetter;
                    MethodInfo methodInfo = PowerInspectorUtility.GetMemberInfoResursively<MethodInfo>(serializedObject.targetObject.GetType(), listGetterMethodPath);

                    if (methodInfo != null && methodInfo.ReturnType == typeof(List<string>))
                    {
                        List<string> list = PowerInspectorUtility.InvokeMethodRecursively(serializedObject.targetObject, listGetterMethodPath) as List<string>;
                        if (list != null)
                        {
                            if (list.Count != 0)
                            {
                                overrideDrawer = (r, p, l) =>
                                {
                                    float height = 0f;
                                    string value = p.stringValue;
                                    bool contain = false;
                                    contain = list.Contains(value);
                                    if (!contain)//Try resolve value missing problem
                                    {
                                        Color color = GUI.backgroundColor;
                                        GUI.backgroundColor = Color.red;
                                        contain = GUI.Button(r, property.displayName + ":value missing in selecting list ,click to resolve!");
                                        height += r.height;
                                        GUI.backgroundColor = color;
                                        if (contain)
                                        {
                                            value = list[0];
                                        }
                                    }

                                    if (contain)
                                    {
                                        value = PowerInspectorUtility.DrawStringSelector(r, value, list, l);
                                        height += EditorGUIUtility.singleLineHeight;
                                    }
                                    p.stringValue = value;

                                    return height;
                                };
                            }
                            else
                            {
                                overrideDrawer = (r, p, l) =>
                                {
                                    return PowerInspectorUtility.DrawWarningMessage(r,
                                        p.displayName + ":Target list is empty!",
                                        (float)(1.5 * EditorGUIUtility.singleLineHeight));
                                };
                            }

                        }
                        else
                        {
                            overrideDrawer = (r, p, l) =>
                            {
                                return PowerInspectorUtility.DrawErrorMessage(r,
                                    p.displayName + ":Target list is null!",
                                    (float)(1.5 * EditorGUIUtility.singleLineHeight));
                            };
                        }
                    }
                    else
                    {
                        overrideDrawer = (r, p, l) =>
                        {
                            return PowerInspectorUtility.DrawErrorMessage(r,
                                p.displayName + ":Method '" + listGetterMethodPath + "' validate failed!",
                                (float)(1.5 * EditorGUIUtility.singleLineHeight));
                        };
                    }
                }
            }

            if (ppa is ElementSelectorAttribute)
            {
                if (fieldInfo.FieldType != typeof(string))
                {
                    if (overrideDrawer == null)
                        overrideDrawer = (r, p, l) =>
                        {
                            return PowerInspectorUtility.DrawErrorMessage(r,
                                p.displayName + ":'ElementSelectorAttribute' should not be applied to non-string type property!",
                                (float)(1.5 * EditorGUIUtility.singleLineHeight));
                        };
                }
                else if (overrideDrawer == null)
                {
                    ElementSelectorAttribute es = ppa as ElementSelectorAttribute;
                    string listGetterMethodPath = PowerInspectorUtility.GetPrefixPath(property.propertyPath) + es.ListGetter;
                    MethodInfo methodInfo = PowerInspectorUtility.GetMemberInfoResursively<MethodInfo>(serializedObject.targetObject.GetType(), listGetterMethodPath);

                    if (methodInfo != null)
                    {
                        object output = PowerInspectorUtility.InvokeMethodRecursively(serializedObject.targetObject, listGetterMethodPath);
                        if (output == null)
                        {
                            overrideDrawer = (r, p, l) =>
                            {
                                return PowerInspectorUtility.DrawErrorMessage(r,
                                    p.displayName + ":Target list is null!",
                                    (float)(1.5 * EditorGUIUtility.singleLineHeight));
                            };
                        }
                        else
                        {
                            if (output is IList)
                            {
                                IList olist = output as IList;

                                if (olist.Count != 0)
                                {
                                    FieldInfo displayFieldInfo = olist[0].GetType().GetField(es.DisplayKey == null ? "" : es.DisplayKey, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                                    FieldInfo refFieldInfo = olist[0].GetType().GetField(es.ReferenceKey == null ? "" : es.ReferenceKey, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                                    if (displayFieldInfo == null)
                                    {
                                        overrideDrawer = (r, p, l) =>
                                        {
                                            return PowerInspectorUtility.DrawErrorMessage(r,
                                               p.displayName + ":Display key word '" + es.DisplayKey + "' does not exist in target type!",
                                                (float)(1.5 * EditorGUIUtility.singleLineHeight));
                                        };
                                        return;
                                    }


                                    if (refFieldInfo == null)
                                    {
                                        refFieldInfo = olist[0].GetType().GetField(es.DisplayKey);
                                    }

                                    List<string> displayKeyList = new List<string>();
                                    List<string> referenceKeyList = new List<string>();
                                    foreach (var o in olist)
                                    {
                                        displayKeyList.Add(displayFieldInfo.GetValue(o).ToString());
                                    }

                                    foreach (var o in olist)
                                    {
                                        referenceKeyList.Add(refFieldInfo.GetValue(o).ToString());
                                    }

                                    overrideDrawer = (r, p, l) =>
                                    {
                                        float height = 0f;
                                        string refValue = p.stringValue;
                                        bool contain = false;
                                        contain = referenceKeyList.Contains(refValue);
                                        if (!contain)//Try resolve value missing problem
                                        {
                                            Color color = GUI.backgroundColor;
                                            GUI.backgroundColor = Color.red;
                                            contain = GUI.Button(r, property.displayName + ":value missing in selecting list ,click to resolve!");
                                            height += r.height;
                                            heightAfterDraw = heightBeforeDraw;
                                            GUI.backgroundColor = color;
                                            if (contain && referenceKeyList.Count != 0)
                                            {
                                                refValue = referenceKeyList[0];
                                            }
                                        }

                                        if (contain)
                                        {
                                            int index = referenceKeyList.IndexOf(refValue);
                                            string displayValue = displayKeyList[index];
                                            displayValue = PowerInspectorUtility.DrawStringSelector(r, displayValue, displayKeyList, l);
                                            height += EditorGUIUtility.singleLineHeight;
                                            index = displayKeyList.IndexOf(displayValue);
                                            refValue = referenceKeyList[index];
                                        }

                                        p.stringValue = refValue;
                                        return height;
                                    };
                                }
                                else
                                {
                                    overrideDrawer = (r, p, l) =>
                                    {
                                        return PowerInspectorUtility.DrawWarningMessage(r,
                                            p.displayName + ":Target list is empty!",
                                            (float)(1.5 * EditorGUIUtility.singleLineHeight));
                                    };
                                }
                            }
                            else
                            {
                                overrideDrawer = (r, p, l) =>
                                {
                                    return PowerInspectorUtility.DrawErrorMessage(r,
                                        p.displayName + ":Method '" + listGetterMethodPath + "' return type is not a list!",
                                        (float)(1.5 * EditorGUIUtility.singleLineHeight));
                                };
                            }
                        }
                    }
                    else
                    {
                        overrideDrawer = (r, p, l) =>
                        {
                            return PowerInspectorUtility.DrawErrorMessage(r,
                                p.displayName + ":Method '" + listGetterMethodPath + "' does not exist!",
                                (float)(1.5 * EditorGUIUtility.singleLineHeight));
                        };
                    }
                }
            }
            #endregion
        }

    }

    [CustomPropertyDrawer(typeof(PowerPropertyAttribute))]
    public class PowerPropertyDrawer : PropertyDrawer
    {
        #region Power Editing Option Context Management
        //One Power property drawer may be used to edit mutiple property such as array elements
        //All array elements share same property drawer but has different editing data
        protected PowerEditingOption drawOptionThisFrame = null;

        protected Dictionary<string, PowerEditingOption> drawOptionContext = new Dictionary<string, PowerEditingOption>();
        public PowerEditingOption SwitchContext(SerializedProperty property)
        {
            string context = property.propertyPath;
            if (!drawOptionContext.ContainsKey(context))
            {
                drawOptionContext.Add(context, new PowerEditingOption());
            }
            return drawOptionContext[context];
        }
        #endregion

        #region Inherited Methods
        //Must seal this method to keep sub-class share same code because we can never know which attribute drawer will be created
        //if many different type of attributes are assigned
        public sealed override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            drawOptionThisFrame = SwitchContext(property);

            //Keep draw option of last frame to draw background before reset option
            DrawBackground(position, drawOptionThisFrame, 1f);
            drawOptionThisFrame.ResetOption();

            //Modify power editing option by reading all power property attributes
            ModifyOption(property);

            //Special process of 'PowerWidget' field.
            if (fieldInfo.FieldType.IsSubclassOf(typeof(PowerWidget)))
            {
                PowerWidgetDrawingTool tool = PowerWidgetDrawingToolFactory.GetWidgetDrawingTool(property);
                if (tool != null)//Display widget when target widget drawing tool class has been found in Assembly
                {
                    drawOptionThisFrame.overrideDrawer = (r, p, l) => { tool.WidgetDrawer(r, p, fieldInfo, l); return tool.GetWidgetHeight(p, fieldInfo); };
                }
                else
                {
                    drawOptionThisFrame.overrideDrawer = (r, p, l) =>
                    {
                        return PowerInspectorUtility.DrawErrorMessage(position, fieldInfo.FieldType.Name + "' has no drawing tool!", (float)(1.5 * EditorGUIUtility.singleLineHeight));
                    };
                }
            }

            drawOptionThisFrame.heightAfterDraw = DrawPowerProperty(position, property, label, drawOptionThisFrame);
            drawOptionThisFrame.firstFrameDrawn = true;
        }

        public sealed override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            drawOptionThisFrame = SwitchContext(property);

            if (drawOptionThisFrame.firstFrameDrawn)
            {
                return drawOptionThisFrame.heightAfterDraw;
            }
            else
            {//This method may possibly be called before drawing, so we need to predict layout height to avoid inspector open glitch
                ModifyOption(property);
                float baseValue = base.GetPropertyHeight(property, label);
                if (drawOptionThisFrame.visible && drawOptionThisFrame.heightAfterDraw == 0)
                    return baseValue;
                return drawOptionThisFrame.heightAfterDraw;
            }

        }

        void ModifyOption(SerializedProperty property)
        {
            drawOptionThisFrame = SwitchContext(property);

            SerializedObject serializedObject = property.serializedObject;
            PowerPropertyAttribute[] powerProperties = fieldInfo.GetCustomAttributes(typeof(PowerPropertyAttribute), true) as PowerPropertyAttribute[];


            foreach (var p in powerProperties)
            {
                //Process for composited power property attribute
                if (p is CompositedPowerPropertyAttribute)
                {
                    CompositedPowerPropertyAttribute com = p as CompositedPowerPropertyAttribute;
                    PowerPropertyAttribute[] attributes = com.GetType().GetCustomAttributes(typeof(PowerPropertyAttribute), true) as PowerPropertyAttribute[];
                    foreach (var a in attributes)
                    {
                        if (a is CompositedPowerPropertyAttribute)
                        {
                            Debug.LogError("'CompositedPowerPropertyAttribute' should not be applied onto 'CompositedPowerPropertyAttribute'");
                            continue;
                        }
                        else
                        {
                            drawOptionThisFrame.ModifyOptionIncrement(a, serializedObject, property, fieldInfo);
                        }
                    }
                }
                else
                {
                    drawOptionThisFrame.ModifyOptionIncrement(p, serializedObject, property, fieldInfo);
                }
            }
        }
        #endregion

        #region Tool Methods

        /// <summary>
        /// Powerful property drawing method
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <param name="option">Color Theme Option</param>
        /// <returns></returns>
        public static float DrawPowerProperty(Rect position, SerializedProperty property, GUIContent label, PowerEditingOption option)
        {
            if (!option.visible)
            {
                return 0f;
            }

            float retHeight = 0f;

            position.height = option.heightBeforeDraw;

            Color color = GUI.contentColor;

            GUI.contentColor = option.fontColor;

            EditorGUI.BeginDisabledGroup(option.readOnly);
            if (option.overrideDrawer == null)
            {
                EditorGUI.PropertyField(position, property, label, true);
                retHeight = EditorGUI.GetPropertyHeight(property);
            }
            else
            {
                retHeight = option.overrideDrawer(position, property, label);
                if (GUI.changed)
                {
                    property.serializedObject.SetIsDifferentCacheDirty();
                }
            }
            EditorGUI.EndDisabledGroup();

            GUI.contentColor = color;
            return retHeight;
        }

        public static void DrawBackground(Rect position, PowerEditingOption option, float padding = 2f)
        {
            if (!option.visible)
            {
                return;
            }
            position.height = option.heightAfterDraw;
            position.x -= 20;
            position.y -= padding;
            position.width += 40;
            position.height += 2 * padding;
            EditorGUI.DrawRect(position, option.backgroundColor);
        }
        #endregion
    }
}