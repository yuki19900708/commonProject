using UnityEngine;
using System;
using System.Collections.Generic;

namespace Universal.TileMapping
{
    [Serializable]
    public abstract class FilledShape : CustomShape
    {
        public bool filled;
        
        public FilledShape() : base()
        {
            filled = true;
        }
    }
}