using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerEditor
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class)]
    public abstract class PowerPropertyAttribute : PropertyAttribute
    {
        /// <summary>
        /// Color theme index used by current serialized property
        /// </summary>
        public string theme { get; protected set; }


        public PowerPropertyAttribute()
        {
            this.theme = "default";
        }
    }
}

