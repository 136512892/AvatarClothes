using System;
using UnityEngine;
using System.Collections.Generic;

namespace Metaverse
{
    [Serializable]
    public class AvatarHairData
    {
        public Sprite thumb;

        public Mesh hairMesh;

        public Material hairMaterial;
    }

    [CreateAssetMenu]
    public class AvatarHairDataConfig : ScriptableObject
    {
        public List<AvatarHairData> data = new List<AvatarHairData>(0);
    }
}