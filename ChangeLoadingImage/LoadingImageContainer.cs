using System;
using UnityEngine;

namespace ChangeLoadingImage
{
    public class LoadingImageContainer
    {
        public Mesh mesh;
        public Material material;
        public float scale;
        public bool showAnimation;
        
        public LoadingImageContainer (Mesh mesh, Material material, float scale, bool showAnimation)
        {
            this.showAnimation = showAnimation;
            this.scale = scale;
            this.material = material;
            this.mesh = mesh;
        }
        public override string ToString ()
        {
            return string.Format ("[LoadingImageContainer: mesh={0}, material={1}, scale={2}, showAnimation={3}]", mesh, material, scale, showAnimation);
        }
        
    }
}

