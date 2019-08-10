using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerInspector
{
    [System.Serializable]
    public class PowerButton : PowerWidget
    {
        [System.NonSerialized]
        public string button;
        [System.NonSerialized]
        public string handler;
        [System.NonSerialized]
        public float height;

        public PowerButton(string button, string handler)
        {
            this.button = button;
            this.handler = handler;
        }

        public PowerButton(string button, string handler, float height)
        {
            this.button = button;
            this.handler = handler;
            this.height = height;
        }
    }
}



