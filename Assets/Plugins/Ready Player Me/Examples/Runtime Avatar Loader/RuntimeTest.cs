using UnityEngine;

namespace ReadyPlayerMe
{
    public class RuntimeTest : MonoBehaviour
    {
        [SerializeField]
        private string AvatarURL = "https://d1a370nemizbjq.cloudfront.net/209a1bc2-efed-46c5-9dfd-edc8a1d9cbe4.glb";

        private void Start()
        {
            Debug.Log($"Started loading avatar. [{Time.timeSinceLevelLoad:F2}]");
            AvatarLoader avatarLoader = new AvatarLoader();
            avatarLoader.LoadAvatar(AvatarURL, OnAvatarImported, OnAvatarLoaded);
        }

        private void OnAvatarImported(GameObject avatar)
        {
            Debug.Log($"Avatar imported. [{Time.timeSinceLevelLoad:F2}]");
        }

        private void OnAvatarLoaded(GameObject avatar, AvatarMetaData metaData)
        {
            Debug.Log($"Avatar loaded. [{Time.timeSinceLevelLoad:F2}]\n\n{metaData}");
        }
    }
}
