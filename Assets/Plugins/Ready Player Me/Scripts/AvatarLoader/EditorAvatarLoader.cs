using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace ReadyPlayerMe
{
    /// <summary>
    ///     Loads avatar models from URL and instantates to the current scene.
    ///     For use in editor. Models can be cached.
    /// </summary>
    public class EditorAvatarLoader: AvatarLoaderBase
    {
        // If a model with given GUID is already downloaded skip download
        public bool UseModelCaching = false;
        // Save destination of the avatar models under Application.persistentDataPath
        private const string SaveFolder = "Resources/Avatars";

        // Makes web request for downloading avatar model and imports the model.
        protected override IEnumerator LoadAvatarAsync(AvatarUri uri)
        {
            if (!UseModelCaching || !File.Exists(uri.ModelPath))
            {
                yield return DownloadAvatar(uri).Run();
            }

            GameObject avatar = InstantiateAvatar(uri);
            yield return DownloadMetaData(uri, avatar).Run();

            RestructureAndSetAnimator(avatar);
            SetAvatarAssetNames(avatar);

            OnAvatarLoaded?.Invoke(avatar, avatarMetaData);
        }

        // Download avatar glb file and store it in SaveFolder.
        protected override IEnumerator DownloadAvatar(AvatarUri uri)
        {
            if (!Directory.Exists($"{ Application.dataPath }/{ SaveFolder }"))
            {
                Directory.CreateDirectory($"{ Application.dataPath }/{ SaveFolder }");
            }

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.LogError("AvatarLoader.LoadAvatarAsync: Please check your internet connection.");
            }
            else
            {
                using (UnityWebRequest request = new UnityWebRequest(uri.AbsoluteUrl))
                {
                    request.downloadHandler = new DownloadHandlerFile(uri.ModelPath);
                    request.timeout = Timeout;

                    yield return request.SendWebRequest();

                    if (request.downloadedBytes == 0)
                    {
                        Debug.LogError("AvatarLoader.LoadAvatarAsync: Please check your internet connection.");
                    }
                    else
                    {
                        // Wait until file write to local is completed
                        yield return new WaitUntil(() =>
                        {
                            return (new FileInfo(uri.ModelPath).Length == (long)request.downloadedBytes);
                        });
                    }
                }
            }
        }

        /// <summary>
        ///     Refresh downloaded glb model and instantiate it in the scene.
        /// </summary>
        private GameObject InstantiateAvatar(AvatarUri uri)
        {
            #if UNITY_EDITOR
                AssetDatabase.ImportAsset($"Assets/{SaveFolder}/{uri.ModelName}");
            #endif

            string name = $"Avatar-{(uint)uri.AbsoluteName.GetHashCode()}";

            GameObject oldInstance = GameObject.Find(name);
            if (oldInstance)
            {
                Object.DestroyImmediate(oldInstance);
            }

            GameObject avatarPrefab = Resources.Load<GameObject>($"Avatars/{uri.AbsoluteName}");
            GameObject avatar = Object.Instantiate(avatarPrefab);
            avatar.name = name;

            return avatar;
        }
    }
}
