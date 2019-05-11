using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace PowerEditor.Editor
{
    public class PowerButtonDrawingTool : PowerWidgetDrawingTool
    {
        PowerButton target;
        MethodInfo buttonMethodInfo = null;

        #region Override Methods
        public override float GetWidgetHeight(SerializedProperty property, FieldInfo fieldInfo)
        {
            if (target == null)
                target = fieldInfo.GetValue(property.serializedObject.targetObject) as PowerButton;

            if (target == null)
                return EditorGUIUtility.singleLineHeight;
            return target.height < EditorGUIUtility.singleLineHeight ? EditorGUIUtility.singleLineHeight : target.height;
        }

        public override void WidgetDrawer(Rect position, SerializedProperty property, FieldInfo fieldInfo, GUIContent label)
        {
            if (target == null)
                target = fieldInfo.GetValue(property.serializedObject.targetObject) as PowerButton;

            if (buttonMethodInfo == null)
                buttonMethodInfo = property.serializedObject.targetObject.GetType().GetMethod(target.handler,
                    BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Default);

            Color backgroundColor = GUI.backgroundColor;
            string buttonContent = target == null ? "Invalid Button" : target.button;
            GUI.backgroundColor = target == null ? Color.red : backgroundColor;

            if (target != null)
                position.height = target.height < EditorGUIUtility.singleLineHeight ? EditorGUIUtility.singleLineHeight : target.height;
            else
                position.height = EditorGUIUtility.singleLineHeight;

            if (GUI.Button(position, buttonContent))
            {
                if (buttonMethodInfo != null)
                    buttonMethodInfo.Invoke(property.serializedObject.targetObject, null);
                else
                    Debug.LogErrorFormat("Button({0}) Callback({1}) Assign Failed!", buttonContent, target.handler);
            }
            GUI.backgroundColor = backgroundColor;
        }
        #endregion
    }

    [CustomPropertyDrawer(typeof(PowerButton))]
    public class PowerButtonDrawer : PowerWidgetDrawer<PowerButtonDrawingTool>
    {

    }
}



