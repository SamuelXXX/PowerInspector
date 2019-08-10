using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace PowerInspector.Editor
{
    public class ColorThemeWindowEditor : EditorWindow
    {
        ColorThemePreset target;
        SerializedObject serializedObject;
        #region Menu
        [MenuItem("Power Inspector/Open Color Theme")]
        static void OpenColorTheme()
        {
            if (ColorThemePreset.Instance == null)
            {
                Debug.LogWarning("ColorTheme file not found under Resources folder!");
                return;
            }

            ColorThemeWindowEditor window = (ColorThemeWindowEditor)GetWindow(typeof(ColorThemeWindowEditor));

            window.target = ColorThemePreset.Instance;
            window.serializedObject = new SerializedObject(window.target);
            window.titleContent.text = "PowerInspectorColorTheme";
            window.Show();
        }
        #endregion

        float lastHeight = 1000f;
        Vector2 scrollPosition = new Vector2(0, 0);
        private void OnGUI()
        {
            if (target == null || serializedObject == null)
            {
                EditorGUILayout.LabelField("Searching color theme setting file. . .");
                target = ColorThemePreset.Instance;
                if (target != null)
                    serializedObject = new SerializedObject(target);

                return;
            }

            try
            {

                serializedObject.Update();


                SerializedProperty iterator = serializedObject.FindProperty("defaultColorTheme");

                Rect position = new Rect(20, 20, 0, 0);
                position.width = this.position.width - 40f;

                if (position.width < 200f)
                    position.width = 200f;
                position.height = position.width * 0.5f;
                if (position.height > Screen.height * 0.2f)
                    position.height = Screen.height * 0.2f;



                scrollPosition = GUI.BeginScrollView(new Rect(0, 0, this.position.width, this.position.height), scrollPosition, new Rect(0, 0, position.width, lastHeight), false, false);

                EditorGUI.DrawTextureTransparent(position, Resources.Load<Texture>("PowerInspectorLogo"), ScaleMode.ScaleAndCrop);
                position.y += position.height;

                EditorGUI.PropertyField(position, iterator, true);
                position.y += EditorGUI.GetPropertyHeight(iterator);

                while (iterator.Next(false))
                {
                    //EditorGUILayout.PropertyField(iterator, true);
                    EditorGUI.PropertyField(position, iterator, true);
                    position.y += EditorGUI.GetPropertyHeight(iterator);
                    lastHeight = position.y;
                }

                GUI.EndScrollView();

                serializedObject.ApplyModifiedProperties();
            }
            catch (Exception)
            {
                serializedObject.ApplyModifiedProperties();
                return;
            }
        }
    }
}


