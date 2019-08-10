using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerInspector
{
    [System.Serializable]
    public class PowerTextureField : PowerWidget
    {
        [System.NonSerialized]
        public string link;
        [System.NonSerialized]
        public float heightRatio;
        [System.NonSerialized]
        public ScaleMode scaleMode;

        public PowerTextureField(string link, float heightRatio, ScaleMode scaleMode = ScaleMode.ScaleToFit)
        {
            this.link = link;
            this.heightRatio = heightRatio;
            this.scaleMode = scaleMode;
        }
    }
}



