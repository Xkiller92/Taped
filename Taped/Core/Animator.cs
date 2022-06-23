using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Taped.Renderer;

namespace Taped.Core
{
    public class Animator 
    {
        internal Dictionary<int, string> textureGroup = new Dictionary<int, string>();
        internal string texturePath { get; set; }
        public bool isSpriteSheet = false;
        internal Vector4 textureCoordinatesX = new Vector4(1.0f, 1.0f, 0.0f, 0.0f);
        internal Vector4 textureCoordinatesY = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
        public int spriteindex { get; set; }

        private int tracker = 1;
        public void LoadTexture(string path, int order)
        {
            textureGroup.Add(order, path);
            texturePath = textureGroup[1];
        }

        public void changeOrder(string path, int newOrder)
        {
            foreach (var item in textureGroup)
            {
                if (item.Value == path)
                {
                    textureGroup.Remove(item.Key);
                    textureGroup.Add(newOrder, path);
                }

                break;
            }
        }

        internal void animate()
        {
            if (isSpriteSheet)
            {
               //calculate the new texture coordinates based on texture atlas data 
            }
            
            texturePath = textureGroup[tracker];
            tracker++;

            if (tracker > textureGroup.Count)
            {
                tracker = 1;
            }
        }
    }
}
