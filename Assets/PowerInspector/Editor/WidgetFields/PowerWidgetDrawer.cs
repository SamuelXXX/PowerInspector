using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEditor;
using System.Linq;
using System;

namespace PowerInspector.Editor
{
    public static class PowerWidgetDrawingToolFactory
    {
        public static Dictionary<UnityEngine.Object, Dictionary<string, PowerWidgetDrawingTool>> powerDrawerToolsDict = new Dictionary<UnityEngine.Object, Dictionary<string, PowerWidgetDrawingTool>>();

        //Use cached drawing tools to avoid important memory state losing and constantly make faulty initial draw 
        public static PowerWidgetDrawingTool GetWidgetDrawingTool(SerializedProperty property)
        {
            RemoveAbondonedTools();//Clear all abondoned data


            if (property == null)
                return null;

            UnityEngine.Object target = property.serializedObject.targetObject;
            string propertyPath = property.propertyPath;

            if (target == null || string.IsNullOrEmpty(propertyPath))
                return null;

            //Get dict layer1
            if (!powerDrawerToolsDict.ContainsKey(target))
            {
                powerDrawerToolsDict.Add(target, new Dictionary<string, PowerWidgetDrawingTool>());
            }

            Dictionary<string, PowerWidgetDrawingTool> dict = powerDrawerToolsDict[target];


            //Get dict layer2
            if (!dict.ContainsKey(propertyPath))
            {
                //string widgetName = PowerEditorUtility.GetMemberInfoResursively<FieldInfo>(target.GetType(), propertyPath).FieldType.Name;
                string widgetName = property.type;
                string drawingToolName = widgetName += "DrawingTool";
                object drawingTool = typeof(PowerWidgetDrawingTool).Assembly.CreateInstance("PowerInspector.Editor." + drawingToolName);

                if (drawingTool == null)
                {
                    Debug.LogErrorFormat("Instance of class '{0}' creating failed, Check widget name!", drawingToolName);
                    return null;
                }

                if (drawingTool is PowerWidgetDrawingTool)
                {
                    dict.Add(propertyPath, drawingTool as PowerWidgetDrawingTool);
                }
                else
                {
                    Debug.LogErrorFormat("Class '{0}' is not sub-class of 'PowerWidgetDrawingTool'!", drawingToolName);
                    return null;
                }
            }
            return dict[propertyPath];
        }

        static void RemoveAbondonedTools()
        {
            //powerDrawerToolsDict.Remove(null);
        }
    }


    public class PowerWidgetDrawingTool
    {
        #region Override Methods
        public virtual float GetWidgetHeight(SerializedProperty property, FieldInfo fieldInfo)
        {
            return EditorGUI.GetPropertyHeight(property);
        }

        public virtual void WidgetDrawer(Rect position, SerializedProperty property, FieldInfo fieldInfo, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label);
        }
        #endregion
    }

    [CustomPropertyDrawer(typeof(PowerWidget))]
    public class PowerWidgetDrawer<T> : PropertyDrawer where T : PowerWidgetDrawingTool
    {
        T tool = null;
        #region Inherited Methods
        public sealed override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (tool == null)
                tool = PowerWidgetDrawingToolFactory.GetWidgetDrawingTool(property) as T;

            if (tool != null)
            {
                tool.WidgetDrawer(position, property, fieldInfo, label);
            }
        }

        public sealed override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (tool == null)
                tool = PowerWidgetDrawingToolFactory.GetWidgetDrawingTool(property) as T;

            if (tool == null)
                return EditorGUIUtility.singleLineHeight;

            return tool.GetWidgetHeight(property, fieldInfo);
        }
        #endregion
    }
}