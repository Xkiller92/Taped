using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace Taped.Core
{
    public class Transform
    {
        public Vector3 position { get; set; }
        public float rotationInAngles { get; set; }
        public float scaleX { get; set; }
        public float scaleY { get; set; }

        public Transform()
        {
            position = new Vector3(0, 0, 0);
            scaleX = 1;
            scaleY = 1;
            rotationInAngles = 0;
        }
    }
}
