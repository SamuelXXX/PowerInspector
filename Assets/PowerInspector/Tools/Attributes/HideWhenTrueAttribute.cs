using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerInspector
{
    /// <summary>
    /// Hide when condition fulfiled
    /// </summary>
    public class HideWhenTrueAttribute : PowerPropertyAttribute
    {
        public string condition { get; set; }
    }
}


