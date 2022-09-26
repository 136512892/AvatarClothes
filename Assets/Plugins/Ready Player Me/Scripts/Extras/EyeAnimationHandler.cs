using UnityEngine;
using System.Collections;
using static ReadyPlayerMe.ExtensionMethods;

namespace ReadyPlayerMe
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Ready Player Me/Eye Animation Handler", 0)]
    public class EyeAnimationHandler : MonoBehaviour
    {
        [Range(0, 1)] public float BlinkSpeed = 0.1f;
        [Range(1, 10)] public float BlinkRate = 3f;
        private WaitForSeconds blinkDelay;

        private const int VerticalMargin = 15;
        private const int HorizontalMargin = 5;

        private SkinnedMeshRenderer headMesh;
        private const string EyeBlinkLeftBlendshapeName = "eyeBlinkLeft";
        private const string EyeBlinkRightBlendshapeName = "eyeBlinkRight";
        private int eyeBlinkLeftBlendshapeIndex = -1;
        private int eyeBlinkRightBlendshapeIndex = -1;

        private Transform leftEyeBone;
        private const string HalfbodyLeftEyeBoneName = "Armature/Hips/Spine/Neck/Head/LeftEye";
        private const string FullbodyLeftEyeBoneName = "Armature/Hips/Spine/Spine1/Spine2/Neck/Head/LeftEye";

        private Transform rightEyeBone;
        private const string HalfbodyRightEyeBoneName = "Armature/Hips/Spine/Neck/Head/RightEye";
        private const string FullbodyRightEyeBoneName = "Armature/Hips/Spine/Spine1/Spine2/Neck/Head/RightEye";
        private const string ArmatureHipsLeftUpLegBoneName = "Armature/Hips/LeftUpLeg";
        private const float EyeBlinkValue = 100f;

        private bool isFullbody;
        private bool hasEyeBlendshapes;

        private void Start()
        {
            headMesh = gameObject.GetMeshRenderer(MeshType.HeadMesh);
            
            eyeBlinkLeftBlendshapeIndex = headMesh.sharedMesh.GetBlendShapeIndex(EyeBlinkLeftBlendshapeName);
            eyeBlinkRightBlendshapeIndex = headMesh.sharedMesh.GetBlendShapeIndex(EyeBlinkRightBlendshapeName);
            
            hasEyeBlendshapes = (eyeBlinkLeftBlendshapeIndex > -1 && eyeBlinkRightBlendshapeIndex > -1);

            isFullbody = transform.Find(ArmatureHipsLeftUpLegBoneName);
            leftEyeBone = transform.Find(isFullbody ? FullbodyLeftEyeBoneName : HalfbodyLeftEyeBoneName);
            rightEyeBone = transform.Find(isFullbody ? FullbodyRightEyeBoneName : HalfbodyRightEyeBoneName);

            Initialize();
        }

        public void Initialize()
        {
            blinkDelay = new WaitForSeconds(BlinkSpeed);

            CancelInvoke();
            InvokeRepeating(nameof(AnimateEyes), 1, BlinkRate);
        }

        private void AnimateEyes()
        {
            RotateEyes();

            if (hasEyeBlendshapes)
            {
                BlinkEyes().Run();
            }
        }

        private void RotateEyes()
        {
            float vertical = Random.Range(-VerticalMargin, VerticalMargin);
            float horizontal = Random.Range(-HorizontalMargin, HorizontalMargin);

            Quaternion rotation = isFullbody ? 
                Quaternion.Euler(horizontal, vertical, 0) : 
                Quaternion.Euler(horizontal - 90, 0, vertical + 180);

            leftEyeBone.localRotation = rotation;
            rightEyeBone.localRotation = rotation;
        }

        private IEnumerator BlinkEyes()
        {
            headMesh.SetBlendShapeWeight(eyeBlinkLeftBlendshapeIndex, EyeBlinkValue);
            headMesh.SetBlendShapeWeight(eyeBlinkRightBlendshapeIndex, EyeBlinkValue);

            yield return blinkDelay;

            headMesh.SetBlendShapeWeight(eyeBlinkLeftBlendshapeIndex, 0);
            headMesh.SetBlendShapeWeight(eyeBlinkRightBlendshapeIndex, 0);
        }
    }
}