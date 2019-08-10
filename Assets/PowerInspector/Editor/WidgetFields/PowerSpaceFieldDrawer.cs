using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace PowerInspector.Editor
{
    public class PowerSpaceFieldDrawingTool : PowerWidgetDrawingTool
    {
        PowerSpaceField target;

        #region Override Methods
        public override float GetWidgetHeight(SerializedProperty property,FieldInfo fieldInfo)
        {
            if (target == null)
                target = fieldInfo.GetValue(property.serializedObject.targetObject) as PowerSpaceField;

            if (target == null)
                return 0;
            return target.height;
        }

        public override void WidgetDrawer(Rect position, SerializedProperty property, FieldInfo fieldInfo, GUIContent label)
        {
        }
        #endregion
    }

    [CustomPropertyDrawer(typeof(PowerSpaceField))]
    public class PowerSpaceFieldDrawer : PowerWidgetDrawer<PowerSpaceFieldDrawingTool>
    {

    }
}



