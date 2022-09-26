using System;
using UnityEngine;
using System.Collections;
using Siccity.GLTFUtility;
using UnityEngine.Networking;

namespace ReadyPlayerMe
{
    /// <summary>
    ///     Loads avatar models from URL and instantates to the current scene.
    /// </summary>
    public class AvatarLoader
    {
        // Avatar download timeout
        public int Timeout { get; set; } = 20;

        /// <summary>
        ///     Load Avatar GameObject from given GLB url.
        /// </summary>
        /// <param name="url">GLB Url acquired from readyplayer.me</param>
        /// <param name="onAvatarImported">Callback method that returns reference to Avatar GameObject</param>
        /// <param name="onAvatarLoaded">Callback method that returns reference to avatar game object and avatars meta data</param>
        public void LoadAvatar(string url, Action<GameObject> onAvatarImported = null, Action<GameObject, AvatarMetaData> onAvatarLoaded = null)
        {
            LoadOperation operation = new LoadOperation();
            operation.Timeout = Timeout;
            operation.LoadAvatar(url, onAvatarImported, onAvatarLoaded);
        }

        /// <summary>
        /// LoadOperation is a simplified avatar loader without local download and caching of models.
        /// Operations are encapsulated not to lose the data of the avatar since they load asynchronously.
        /// </summary>
        class LoadOperation : AvatarLoaderBase
        {
            // Avatar GLB model bytes in memory.
            private byte[] avatarBytes;
            private AvatarUri uri;

            // Makes web request for downloading avatar model into memory and imports the model.
            protected override IEnumerator LoadAvatarAsync(AvatarUri uri)
            {
                this.uri = uri;

                yield return DownloadAvatar(uri).Run();

#if !UNITY_EDITOR && UNITY_WEBGL
                GameObject avatar = Importer.LoadFromBytes(avatarBytes, new ImportSettings() { useLegacyClips = true });
                OnImportFinished(avatar);
#else
                Importer.ImportGLBAsync(avatarBytes, new ImportSettings() { useLegacyClips = true }, OnImportFinished);
#endif
            }

            // Download avatar model into memory and cache bytes
            protected override IEnumerator DownloadAvatar(AvatarUri uri)
            {
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    Debug.LogError("AvatarLoader.LoadAvatarAsync: Please check your internet connection.");
                }
                else
                {
                    using (UnityWebRequest request = new UnityWebRequest(uri.AbsoluteUrl))
                    {
                        request.downloadHandler = new DownloadHandlerBuffer();

                        yield return request.SendWebRequest();

                        if (request.downloadedBytes == 0)
                        {
                            Debug.LogError("AvatarLoader.LoadAvatarAsync: Please check your internet connection.");
                        }
                        else
                        {
                            avatarBytes = request.downloadHandler.data;
                        }
                    }
                }
            }

            // GLTF Utility Callback for finished model load operation
            private void OnImportFinished(GameObject avatar)
            {
                avatar.name = "Avatar";
                PrepareAvatarAsync(avatar).Run();
                OnAvatarImported?.Invoke(avatar);
            }

            private IEnumerator PrepareAvatarAsync(GameObject avatar)
            {
                yield return DownloadMetaData(uri, avatar).Run();
                RestructureAndSetAnimator(avatar);
                SetAvatarAssetNames(avatar);
                OnAvatarLoaded?.Invoke(avatar, avatarMetaData);
            }
        }
    }
}
