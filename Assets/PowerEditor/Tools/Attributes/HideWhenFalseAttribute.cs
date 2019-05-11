using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerEditor
{
    /// <summary>
    /// Hide when condition not fulfiled
    /// </summary>
    public class HideWhenFalseAttribute : PowerPropertyAttribute
    {
        public string condition { get; set; }
    }
}


