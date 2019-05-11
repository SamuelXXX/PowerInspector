using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace PowerEditor.Editor
{
    public class PowerTextureFieldDrawingTool : PowerWidgetDrawingTool
    {
        PowerTextureField target;
        float inspectorWidth;

        #region Override Methods
        public override float GetWidgetHeight(SerializedProperty property, FieldInfo fieldInfo)
        {
            if (target == null)
                target = fieldInfo.GetValue(property.serializedObject.targetObject) as PowerTextureField;

            if (target == null)
                return 0;
            return target.heightRatio * inspectorWidth;
        }

        public override void WidgetDrawer(Rect position, SerializedProperty property, FieldInfo fieldInfo, GUIContent label)
        {
            if (target == null)
                target = fieldInfo.GetValue(property.serializedObject.targetObject) as PowerTextureField;

            if (position.width > 100f)
                inspectorWidth = position.width;

            float height = inspectorWidth * target.heightRatio;
            position.height = height;

            Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(target.link);

            if (texture == null)
                texture = Resources.Load<Texture>(target.link);

            if (texture == null)
            {
                PowerEditorUtility.DrawErrorMessage(position, "'" + target.link + "' is not a valid texture resource path!", height);
            }
            else
            {
                EditorGUI.DrawTextureTransparent(position, texture, target.scaleMode);
            }
        }
        #endregion
    }

    [CustomPropertyDrawer(typeof(PowerTextureField))]
    public class PowerTextureFieldDrawer : PowerWidgetDrawer<PowerTextureFieldDrawingTool>
    {

    }
}



