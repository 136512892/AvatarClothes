using System;
using UnityEngine;
using System.Collections.Generic;

namespace Metaverse
{
    [Serializable]
    public class AvatarGlassesData
    {
        public Sprite thumb;

        public Mesh glassesMesh;

        public Material glassesMaterial;
    }

    [CreateAssetMenu]
    public class AvatarGlassesDataConfig : ScriptableObject
    {
        public List<AvatarGlassesData> data = new List<AvatarGlassesData>(0);
    }
}