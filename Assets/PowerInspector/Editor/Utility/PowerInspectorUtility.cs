using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using System;
using System.Reflection;

namespace PowerInspector.Editor
{
    public static class PowerInspectorUtility
    {
        #region Special Layout
        /// <summary>
        /// Do String Selector Layout
        /// </summary>
        /// <param name="position"></param>
        /// <param name="value"></param>
        /// <param name="stringList"></param>
        /// <param name="label"></param>
        /// <param name="option"></param>
        /// <param name="readOnly"></param>
        /// <returns></returns>
        public static int DrawIntegerSelector(Rect position, int value, List<string> stringList, GUIContent label)
        {
            if (stringList == null)
                return value;

            int ret = EditorGUI.Popup(position, label.text, value, stringList.ToArray());

            return ret;
        }

        /// <summary>
        /// Do String Selector Layout
        /// </summary>
        /// <param name="position"></param>
        /// <param name="value"></param>
        /// <param name="stringList"></param>
        /// <param name="label"></param>
        /// <param name="option"></param>
        /// <param name="readOnly"></param>
        /// <returns></returns>
        public static string DrawStringSelector(Rect position, string value, List<string> stringList, GUIContent label)
        {
            if (stringList == null)
                return value;

            if (!stringList.Contains(value))
                return value;

            int index = stringList.IndexOf(value);
            index = DrawIntegerSelector(position, index, stringList, label);
            return stringList[index];
        }
        #endregion

        #region String Code Conversion
        /// <summary>
        /// Convert color code like "#FFFFFF" or "(100,100,100)" to concret color instance Unity supported 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static Color ColorCode2Color(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Color.white;
            }
            Regex colorExp1 = new Regex(@"^#[\da-fA-F]{6}$");
            Regex colorExp2 = new Regex(@"^\(\d*,\d*,\d*\)$");

            code = code.Replace(" ", "");
            if (colorExp1.IsMatch(code))
            {
                ushort[] rgb = new ushort[3];
                rgb[0] = Convert.ToUInt16(code.Substring(1, 2), 16);
                rgb[1] = Convert.ToUInt16(code.Substring(3, 2), 16);
                rgb[2] = Convert.ToUInt16(code.Substring(5, 2), 16);

                return new Color((float)rgb[0] / 255f, (float)rgb[1] / 255f, (float)rgb[2] / 255f);
            }

            if (colorExp2.IsMatch(code))
            {
                code = code.Replace("(", "");
                code = code.Replace(")", "");

                string[] rgb = code.Split(',');
                return new Color((float)(Convert.ToUInt16(rgb[0] == "" ? "0" : rgb[0], 10)) / 255f,
                                 (float)(Convert.ToUInt16(rgb[1] == "" ? "0" : rgb[1], 10)) / 255f,
                                 (float)(Convert.ToUInt16(rgb[2] == "" ? "0" : rgb[2], 10)) / 255f);
            }

            Debug.LogWarningFormat("Syntax error in color code '{0}'", code);
            return Color.white;

        }

        /// <summary>
        /// Check if the color code is valid to use
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static bool CheckColorCodeValid(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return false;
            }
            Regex colorExp1 = new Regex(@"^#[\da-fA-F]{6}$");
            Regex colorExp2 = new Regex(@"^\(\d*,\d*,\d*\)$");

            code = code.Replace(" ", "");
            if (colorExp1.IsMatch(code))
            {
                return true;
            }

            if (colorExp2.IsMatch(code))
            {
                return true;
            }

            return false;
        }
        #endregion

        #region Recursive Reflection Tools
        public class PathDecorator
        {
            protected string fullPath;
            public string currentPathPoint { get; private set; }

            protected PathDecorator childWrapper;
            public PathDecorator(string path)
            {
                if (string.IsNullOrEmpty(path))
                    return;
                childWrapper = null;
                this.fullPath = path;
                if (path.Contains("."))
                {
                    int index = path.IndexOf('.');
                    currentPathPoint = path.Substring(0, index);
                    string nextPath = path.Substring(index + 1, path.Length - index - 1);
                    childWrapper = new PathDecorator(nextPath);
                }
                else
                {
                    currentPathPoint = path;
                    childWrapper = null;
                }
            }

            public PathDecorator EnterNextPathPoint()
            {
                return childWrapper;
            }
        }

        public class PathWrapper
        {
            protected PathDecorator current;
            public string CurrentPathPoint
            {
                get
                {
                    if (current == null)
                        return null;

                    return current.currentPathPoint;
                }
            }

            public PathWrapper(string path)
            {
                if (string.IsNullOrEmpty(path))
                    return;
                current = new PathDecorator(path);
            }

            public bool Next()
            {
                if (current == null)
                    return false;

                current = current.EnterNextPathPoint();
                if (current == null)
                    return false;

                return true;
            }
        }

        public static MemberInfo GetMemberInfoRecursively(Type searchType, string path,
            BindingFlags bindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            if (searchType == null)
                return null;

            try
            {
                PathWrapper pathWrapper = new PathWrapper(path);
                MemberInfo currentMemberInfo = null;
                MemberInfo[] memberInfoGroup;

                while (pathWrapper.CurrentPathPoint != null)
                {
                    memberInfoGroup = searchType.GetMember(pathWrapper.CurrentPathPoint, bindingFlags);
                    if (memberInfoGroup == null || memberInfoGroup.Length == 0)
                        return null;
                    currentMemberInfo = memberInfoGroup[0];



                    if (currentMemberInfo is FieldInfo)
                    {
                        searchType = (currentMemberInfo as FieldInfo).FieldType;
                        if (searchType.IsGenericType)
                        {
                            searchType = searchType.GetGenericArguments()[0];
                            pathWrapper.Next();//Skip "Array"
                            pathWrapper.Next();//Skip "data[n]"
                        }

                    }
                    else
                    {
                        break;//Terminate iterating when encounter non-field member
                    }
                    pathWrapper.Next();
                }
                if (pathWrapper.Next())
                {
                    //Not finished yet
                    return null;
                }

                return currentMemberInfo;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return null;
            }
        }

        public static T GetMemberInfoResursively<T>(Type searchType, string path,
            BindingFlags bindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where T : MemberInfo
        {
            MemberInfo memberInfo = GetMemberInfoRecursively(searchType, path, bindingFlags);

            if (memberInfo is T)
                return memberInfo as T;

            return null;
        }

        public static object GetFieldObjectRecursively(object target, string path)
        {
            if (target == null)
                return null;

            Type searchType = target.GetType();
            if (searchType == null)
                return null;

            try
            {
                PathWrapper pathWrapper = new PathWrapper(path);
                FieldInfo currentFieldInfo = null;

                while (pathWrapper.CurrentPathPoint != null)
                {
                    if (searchType.IsGenericType)
                    {
                        searchType = searchType.GetGenericArguments()[0];

                        pathWrapper.Next();//Skip "Array"
                        string ppoint = pathWrapper.CurrentPathPoint;
                        ppoint = ppoint.Replace("data[", "");
                        ppoint = ppoint.Replace("]", "");
                        int index = int.Parse(ppoint);
                        target = (target as IList)[index];
                        pathWrapper.Next();//Skip "data[n]"

                        if (string.IsNullOrEmpty(pathWrapper.CurrentPathPoint))
                        {
                            return target;
                        }
                        else
                        {
                            currentFieldInfo = searchType.GetField(pathWrapper.CurrentPathPoint);
                            if (currentFieldInfo == null)
                                return null;

                            target = currentFieldInfo.GetValue((target as IList)[index]);
                        }

                    }
                    else
                    {
                        currentFieldInfo = searchType.GetField(pathWrapper.CurrentPathPoint);
                        if (currentFieldInfo == null)
                            return null;

                        target = currentFieldInfo.GetValue(target);
                    }

                    if (target == null)
                        return null;

                    searchType = target.GetType();

                    pathWrapper.Next();
                }

                return target;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return null;
            }
        }

        public static object GetFieldObjectRecursively<T>(object target, string path)
        {
            object obj = GetFieldObjectRecursively(target, path);
            if (obj is T)
                return obj;
            return null;
        }

        public static object InvokeMethodRecursively(object target, string path,
                                                    BindingFlags bindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                                    params object[] parameters)
        {
            if (string.IsNullOrEmpty(path))
            {
                //Debug.LogError("Invoke Method Recursively Failed!");
                return null;
            }

            string prefixPath = "";
            string methodName = "";
            if (path.Contains("."))
            {
                int index = path.LastIndexOf('.');
                prefixPath = path.Substring(0, index);
                methodName = path.Substring(index + 1, path.Length - index - 1);
            }
            else
            {
                methodName = path;
            }

            object field = GetFieldObjectRecursively(target, prefixPath);
            if (field != null)
            {
                MethodInfo method = field.GetType().GetMethod(methodName, bindingFlags);
                if (method != null)
                    return method.Invoke(field, parameters);
            }

            return null;
        }

        public static string GetPrefixPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return "";
            }
            if (path.Contains("."))
            {
                int index = path.LastIndexOf('.');
                return path.Substring(0, index + 1);
            }
            else
            {
                return "";
            }
        }

        public static string GetMemberName(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return "";
            }
            if (path.Contains("."))
            {
                int index = path.LastIndexOf('.');
                return path.Substring(index + 1, path.Length - index - 1);
            }
            else
            {
                return "";
            }
        }

        public static SerializedProperty GetSiblingProperty(SerializedObject so, SerializedProperty sp, string siblingPropertyName)
        {
            string path = sp.propertyPath;
            if (path.Contains("."))
            {
                int lastDotIndex = path.LastIndexOf('.');
                path = path.Substring(0, lastDotIndex + 1);//Include last '.' character
                path += siblingPropertyName;
                return so.FindProperty(path);
            }
            else
            {
                return so.FindProperty(siblingPropertyName);
            }
        }
        #endregion

        #region Error/Exception Display
        /// <summary>
        /// Display an error message in inspector
        /// </summary>
        /// <param name="position"></param>
        /// <param name="message"></param>
        public static float DrawErrorMessage(Rect position, string message, float height = 0)
        {
            if (height < EditorGUIUtility.singleLineHeight)
                height = EditorGUIUtility.singleLineHeight;
            position.height = height;
            DrawMessage(position, message, Color.red, Color.white);
            return height;
        }

        /// <summary>
        /// Display an warning message in inspector
        /// </summary>
        /// <param name="position"></param>
        /// <param name="message"></param>
        public static float DrawWarningMessage(Rect position, string message, float height = 0)
        {
            if (height < EditorGUIUtility.singleLineHeight)
                height = EditorGUIUtility.singleLineHeight;
            position.height = height;
            DrawMessage(position, message, Color.yellow, Color.black);
            return height;
        }

        static void DrawMessage(Rect position, string message, Color? backgroundColor = null, Color? contentColor = null)
        {
            Color cacheBC = GUI.backgroundColor;
            Color cacheCC = GUI.contentColor;

            GUI.backgroundColor = backgroundColor == null ? GUI.backgroundColor : backgroundColor.Value;
            GUI.contentColor = contentColor == null ? GUI.contentColor : contentColor.Value;

            EditorGUI.DrawRect(position, GUI.backgroundColor);
            float labelYOffset = (position.height - EditorGUIUtility.singleLineHeight) / 2f;

            position.y += labelYOffset;
            EditorGUI.LabelField(position, message);


            GUI.backgroundColor = cacheBC;
            GUI.contentColor = cacheCC;
        }
        #endregion
    }
}


