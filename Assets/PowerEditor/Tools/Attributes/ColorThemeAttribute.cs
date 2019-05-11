using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerEditor
{
    /// <summary>
    /// Color theme
    /// </summary>
    public class ColorThemeAttribute : PowerPropertyAttribute
    {
        /// <summary>
        /// Override font color when editing
        /// </summary>
        public string fontColor { get; set; }
        /// <summary>
        /// Override background color when editing
        /// </summary>
        public string backgroundColor { get; set; }

        /// <summary>
        /// Override runtime font color when editing
        /// </summary>
        public string runtimeFontColor { get; set; }
        /// <summary>
        /// Override runtime background color when editing
        /// </summary>
        public string runtimeBackgroundColor { get; set; }

        public ColorThemeAttribute(string colorTheme)
        {
            this.theme = colorTheme;
        }

        public ColorThemeAttribute()
        {
            this.theme = "default";
        }
    }
}


