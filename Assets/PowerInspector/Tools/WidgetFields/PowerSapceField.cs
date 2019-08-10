using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerInspector
{
    [System.Serializable]
    public class PowerSpaceField : PowerWidget
    {
        [System.NonSerialized]
        public float height;

        public PowerSpaceField(float height)
        {
            this.height = height;
        }
    }
}



