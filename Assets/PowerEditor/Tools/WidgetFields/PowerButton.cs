using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerEditor
{
    [System.Serializable]
    public class PowerButton : PowerWidget
    {
        public string button { get; private set; }
        public string handler { get; private set; }
        public float height { get; private set; }

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



