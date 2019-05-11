using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;

namespace PowerEditor.Editor
{
    public class PowerListDrawerManager
    {
        #region Instance
        protected static PowerListDrawerManager instance;
        public static PowerListDrawerManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new PowerListDrawerManager();
                return instance;
            }
        }

        PowerListDrawerManager() { }
        #endregion

        #region Power List Drawer Managing
        class PowerListDrawerWrapper
        {
            public ReorderableList reorderableList;

            public SerializedObject serializedObject;
            public SerializedProperty listProperty;
            public int activeIndex;
        }

        Dictionary<Object, Dictionary<string, PowerListDrawerWrapper>> powerListDrawerDict = new Dictionary<Object, Dictionary<string, PowerListDrawerWrapper>>();
        PowerListDrawerWrapper GetPowerListDrawer(Object target, string propertyPath)
        {
            if (target == null || string.IsNullOrEmpty(propertyPath))
                return null;

            //Get dict layer1
            if (!powerListDrawerDict.ContainsKey(target))
            {
                powerListDrawerDict.Add(target, new Dictionary<string, PowerListDrawerWrapper>());
            }

            Dictionary<string, PowerListDrawerWrapper> dict = powerListDrawerDict[target];

            SerializedObject serializedObject = new SerializedObject(target);
            SerializedProperty listProperty = serializedObject.FindProperty(propertyPath);
            if (listProperty == null)
                return null;

            //Get dict layer2
            if (!dict.ContainsKey(propertyPath))
            {
                PowerListDrawerWrapper wrapper = new PowerListDrawerWrapper();

                //Prepare data
                wrapper.serializedObject = serializedObject;
                wrapper.listProperty = listProperty;
                wrapper.reorderableList = new ReorderableList(serializedObject, listProperty, true, true, true, true);
                wrapper.activeIndex = 0;
                dict.Add(propertyPath, wrapper);
            }

            return dict[propertyPath];
        }
        #endregion

        #region Draw methods
        public float DrawPowerList(Rect position, Object targetObject, string listPropertyPath, string keyWord = null)
        {
            PowerListDrawerWrapper wrapper = GetPowerListDrawer(targetObject, listPropertyPath);

            if (wrapper == null)
            {
                return PowerEditorUtility.DrawErrorMessage(position, targetObject.name + "." + listPropertyPath + " is not a valid list-typed property path!");
            }
            wrapper.serializedObject.Update();

            //Draw reorderable list
            ReorderableList reorderableList = wrapper.reorderableList;


            //Initialize reorderable list
            reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                SerializedProperty elem = wrapper.listProperty.GetArrayElementAtIndex(index);
                if (isActive)
                {
                    wrapper.activeIndex = index;
                }
                if (keyWord == null)
                    EditorGUI.PropertyField(rect, elem);
                else
                {
                    SerializedProperty keyProperty = elem.FindPropertyRelative(keyWord);
                    if (keyProperty != null && keyProperty.propertyType == SerializedPropertyType.String)
                    {
                        EditorGUI.LabelField(rect, keyProperty.stringValue);
                    }
                    else
                    {
                        EditorGUI.PropertyField(rect, elem);
                    }
                }
            };

            reorderableList.drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, wrapper.listProperty.displayName);
            };

            reorderableList.DoList(position);
            float listHeight = reorderableList.GetHeight();

            position.y += listHeight;

            //Draw element property field under reorderable list
            wrapper.activeIndex = wrapper.activeIndex >= wrapper.listProperty.arraySize ? wrapper.listProperty.arraySize - 1 : wrapper.activeIndex;
            if (wrapper.activeIndex >= 0)
            {
                SerializedProperty elem = wrapper.listProperty.GetArrayElementAtIndex(wrapper.activeIndex);
                elem.isExpanded = true;
                EditorGUI.PropertyField(position, elem, true);

                wrapper.serializedObject.ApplyModifiedProperties();
                return listHeight + EditorGUI.GetPropertyHeight(elem);
            }
            else
            {
                wrapper.serializedObject.ApplyModifiedProperties();
                return listHeight;
            }
        }
        #endregion
    }
    /// <summary>
    /// Power list property drawer is applied to all array element, not the whole list
    /// </summary>
    [CustomPropertyDrawer(typeof(PowerListAttribute))]
    public class PowerListDrawer : PowerPropertyDrawer { }

}


