using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerEditor
{
    [System.Serializable]
    public class PowerTextureField : PowerWidget
    {
        public string link { get; private set; }
        public float heightRatio { get; private set; }
        public ScaleMode scaleMode { get;private set; }

        public PowerTextureField(string link,float heightRatio,ScaleMode scaleMode=ScaleMode.ScaleToFit)
        {
            this.link = link;
            this.heightRatio = heightRatio;
            this.scaleMode = scaleMode;
        }
    }
}



