using System;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections;
using UnityEngine.Networking;

namespace ReadyPlayerMe
{
    public abstract class AvatarLoaderBase
    {
        // Avatar download timeout
        public int Timeout { get; set; } = 20;

        // OnAvatarImported callback
        protected Action<GameObject> OnAvatarImported = null;

        // OnAvatarLoaded callback
        protected Action<GameObject, AvatarMetaData> OnAvatarLoaded = null;

        // Avatar meta data
        protected AvatarMetaData avatarMetaData = null;

        // Prefix to remove from names for correction
        protected const string Prefix = "Wolf3D_";

        // Animation avatars
        protected const string MasculineAnimationAvatarName = "AnimationAvatars/MasculineAnimationAvatar";
        protected const string FeminineAnimationAvatarName = "AnimationAvatars/FeminineAnimationAvatar";

        // Animation controller
        protected const string AnimatorControllerName = "Avatar Animator";

        // Bone names
        private const string Hips = "Hips";
        private const string Armature = "Armature";
        private const string LeftUpLeg = "Hips/LeftUpLeg";
        private const string Spine = "Hips/Spine/Spine1/Spine2";

        // Version info
        private const string VersionPrefix = "V";
        private const int LegacyOutfitVersion = 1;
        private const float LegacySpineDistance = 0.35f;

        //Texture property IDs
        protected static readonly string[] ShaderProperties = new[] {
            "_MainTex",
            "_BumpMap",
            "_EmissionMap",
            "_OcclusionMap",
            "_MetallicGlossMap"
        };

        /// <summary>
        ///     Load Avatar GameObject from given GLB url.
        /// </summary>
        /// <param name="url">GLB Url acquired from readyplayer.me</param>
        /// <param name="callback">Callback method that returns reference to Avatar GameObject</param>
        public async void LoadAvatar(string url, Action<GameObject> onAvatarImported = null, Action<GameObject, AvatarMetaData> onAvatarLoaded = null)
        {
            OnAvatarImported = onAvatarImported;
            OnAvatarLoaded = onAvatarLoaded;

            AvatarUri uri = await new AvatarUri().Create(url);
            LoadAvatarAsync(uri).Run();
        }

        // Makes web request for downloading avatar model and imports the model.
        protected abstract IEnumerator LoadAvatarAsync(AvatarUri uri);
        
        // Downloading avatar model
        protected abstract IEnumerator DownloadAvatar(AvatarUri uri);

        protected virtual IEnumerator DownloadMetaData(AvatarUri uri, GameObject avatar)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(uri.MetaDataUrl))
            {
                yield return request.SendWebRequest();

                string result = request.downloadHandler.text;

                SetMetaData(result, avatar);
            }
        }

        private string AnimationAvatarSource => avatarMetaData.IsOutfitMasculine() ? MasculineAnimationAvatarName : FeminineAnimationAvatarName;

        /// <summary>
        ///     Restructure avatar bones and add gender spesific animation avatar and animator controller.
        /// </summary>
        protected void RestructureAndSetAnimator(GameObject avatar)
        {
            #region Restructure
            GameObject armature = new GameObject();
            armature.name = Armature;

            armature.transform.parent = avatar.transform;

            Transform hips = avatar.transform.Find(Hips);
            hips.parent = armature.transform;
            #endregion

            #region SetAnimator
            if (avatarMetaData.IsFullbody())
            {
                Avatar animationAvatar = Resources.Load<Avatar>(AnimationAvatarSource);
                RuntimeAnimatorController animatorController = Resources.Load<RuntimeAnimatorController>(AnimatorControllerName);

                Animator animator = avatar.AddComponent<Animator>();
                animator.runtimeAnimatorController = animatorController;
                animator.avatar = animationAvatar;
                animator.applyRootMotion = true;
            }
            #endregion
        }

        private string OutfitVersion(int version) 
        {
            switch (version)
            {
                case 2: return $"{VersionPrefix}{version}";
                default: return string.Empty;
            }
        }

        private void SetMetaData(string result, GameObject avatar)
        {
            try
            {
                avatarMetaData = JsonConvert.DeserializeObject<AvatarMetaData>(result);
            }
            catch
            {
                avatarMetaData = new AvatarMetaData();

                // Legacy avatar
                avatarMetaData.OutfitVersion = LegacyOutfitVersion;

                // Body type
                avatarMetaData.BodyType = avatar.transform.Find(LeftUpLeg) ? BodyType.Fullbody: BodyType.Halfbody;

                // Outfit Gender
                if (avatarMetaData.IsFullbody())
                {
                    Vector3 hipsPos = avatar.transform.Find(Hips).transform.position;
                    Vector3 spinePos = avatar.transform.Find(Spine).transform.position;
                    avatarMetaData.OutfitGender = (Vector3.Distance(hipsPos, spinePos) > LegacySpineDistance) ? OutfitGender.Masculine : OutfitGender.Feminine;
                }
            }
        }

        #region Set Names
        private const string AvatarPrefix = "Avatar";
        private const string MeshPrefix = "Avatar_Mesh";
        private const string RendererPrefix = "Avatar_Renderer";
        private const string MaterialPrefix = "Avatar_Material";
        private const string SkinnedMeshPrefix = "Avatar_SkinnedMesh";

        /// <summary>
        ///     Name avatar assets for make them easier to view in profiler.
        ///     Naming is 'Avatar_<Type>_<Name>'
        /// </summary>
        protected void SetAvatarAssetNames(GameObject avatar)
        {
            Renderer[] renderers = avatar.GetComponentsInChildren<SkinnedMeshRenderer>();

            foreach (var renderer in renderers)
            {
                string assetName = renderer.name.Replace(Prefix, "");

                renderer.name = $"{RendererPrefix}_{assetName}";
                renderer.sharedMaterial.name = $"{MaterialPrefix}_{assetName}";
                SetTextureNames(renderer, assetName);
                SetMeshName(renderer, assetName);
            }
        }

        /// <summary>
        ///     Set a name for the texture for finding it in the Profiler.
        /// </summary>
        /// <param name="renderer">Renderer to find the texture in.</param>
        /// <param name="assetName">Name of the asset.</param>
        /// <param name="propertyID">Property ID of the texture.</param>
        private void SetTextureNames(Renderer renderer, string assetName)
        {
            foreach (string propertyName in ShaderProperties)
            {
                int propertyID = Shader.PropertyToID(propertyName);

                if (renderer.sharedMaterial.HasProperty(propertyID))
                {
                    var texture = renderer.sharedMaterial.GetTexture(propertyID);

                    if (texture != null)
                    {
                        texture.name = $"{AvatarPrefix}{propertyName}_{assetName}";
                    }
                }
            }
        }

        /// <summary>
        ///     Set a name for the mesh for finding it in the Profiler.
        /// </summary>
        /// <param name="renderer">Renderer to find the mesh in.</param>
        /// <param name="assetName">Name of the asset.</param>
        private void SetMeshName(Renderer renderer, string assetName)
        {
            if (renderer is SkinnedMeshRenderer)
            {
                (renderer as SkinnedMeshRenderer).sharedMesh.name = $"{SkinnedMeshPrefix}_{assetName}";
                (renderer as SkinnedMeshRenderer).updateWhenOffscreen = true;
            }
            else if (renderer is MeshRenderer)
            {
                MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();

                if (meshFilter != null)
                {
                    meshFilter.sharedMesh.name = $"{MeshPrefix}_{assetName}";
                }
            }
        }
        #endregion
    }
}
