using System;
using UnityEngine;
using System.Collections.Generic;

namespace Metaverse
{
    [Serializable]
    public class AvatarBeardData
    {
        public Sprite thumb;

        public enum Type
        {
            Mesh,
            Texture,
        }

        public Type type = Type.Mesh;

        public Mesh beardMesh;

        public Material beardMaterial;
    }

    [CreateAssetMenu]
    public class AvatarBeardDataConfig : ScriptableObject
    {
        public List<AvatarBeardData> data = new List<AvatarBeardData>(0);
    }
}