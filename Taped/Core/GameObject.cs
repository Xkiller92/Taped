using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taped.Components;
using Taped.Renderer;

namespace Taped.Core
{
    public abstract class GameObject
    {
        public Transform transform { get; set; }
        public Animator animator { get; set; }
        public bool isEnabled { get; set; }

        public GameObject()
        {
            AddToActiveScene();
            isEnabled = true;
            transform = new Transform();
            animator = new Animator();
            try
            {
                Awake();
            }
            catch (Exception)
            {

            }
        }

        public GameObject CreateObject(GameObject go)
        {
            try
            {
                SceneManager.objects.Add(go);
                return go;
            }
            catch (Exception)
            {

                throw new Exception("not derived from GameObject");
            }
        }

        public void AddToActiveScene()
        {
            SceneManager.objects.Add(this);
        }

        public virtual void Start()
        {
            if (animator.texturePath != null)
            {
                MasterRenderer.GenerateSprite(animator.texturePath);
            }
        }

        public virtual void Awake()
        {
            if (animator.texturePath != null)
            {
                MasterRenderer.GenerateSprite(animator.texturePath);
            }
        }

        public virtual void Update(double time, KeyboardState key)
        {
            
        }
    }
}
