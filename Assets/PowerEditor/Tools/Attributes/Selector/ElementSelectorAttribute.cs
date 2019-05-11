using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerEditor
{
    /// <summary>
    /// Element Selector
    /// </summary>
    public class ElementSelectorAttribute : PowerPropertyAttribute
    {
        public string ListGetter { get; set; }
        public string ReferenceKey { get; set; }

        public string DisplayKey { get; set; }
        
    }
}


