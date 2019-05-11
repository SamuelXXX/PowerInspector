using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PowerEditor
{
    [CreateAssetMenu(fileName = "ColorTheme", menuName = "PowerEditor/ColorTheme")]
    public class ColorThemePreset : ScriptableObject
    {
        #region Theme Defination
        [System.Serializable]
        public class ColorTheme
        {
            public string themeName;
            public Color fontColor = Color.white;
            public Color backgroundColor = Color.white;

            public Color runtimeFontColor;
            public Color runtimeBackgroundColor;

            public ColorTheme()
            {
                fontColor = Color.white;
                runtimeFontColor = Color.white;

                backgroundColor = new Color(0, 0, 0, 0);
                runtimeBackgroundColor = new Color(0, 0, 0, 0);
            }
        }
        #endregion

        #region Theme Data
        PowerTextureField logo = new PowerTextureField("logo", 0.4f);
        [Header("Default Color Theme")]
        public ColorTheme defaultColorTheme = new ColorTheme();

        [Header("Customized Color Theme"),PowerList(Key ="themeName")]
        public List<ColorTheme> colorThemeList = new List<ColorTheme>();
        #endregion

        #region Instance
        protected static ColorThemePreset instance;
        public static ColorThemePreset Instance
        {
            get
            {
                if (instance == null)
                    instance = Resources.Load<ColorThemePreset>("ColorTheme");

                return instance;
            }
        }
        #endregion

        #region Tool Methods
        public ColorTheme GetColorTheme(string index)
        {
            if (index == "default" || string.IsNullOrEmpty(index) || !colorThemeList.Exists(t => t.themeName == index))
            {
                return defaultColorTheme;
            }
            return colorThemeList.Find(t => t.themeName == index);
        }
        #endregion
    }
}


