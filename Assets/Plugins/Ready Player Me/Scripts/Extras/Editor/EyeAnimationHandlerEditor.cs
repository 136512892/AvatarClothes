using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe
{
    [CustomEditor(typeof(EyeAnimationHandler))]
    public class EyeAnimationHandlerEditor : Editor
    {
        private readonly GUIContent BlinkSpeedLabel = new GUIContent("Blink Speed", "How fast the eyes blink in seconds.");
        private readonly GUIContent BlinkRateLabel = new GUIContent("Blink Rate", "Frequency of eye blinking in seconds.");

        private SerializedProperty BlinkSpeed;
        private SerializedProperty BlinkRate;

        private void OnEnable()
        {
            BlinkSpeed = serializedObject.FindProperty("BlinkSpeed");
            BlinkRate = serializedObject.FindProperty("BlinkRate");
        }

        public override void OnInspectorGUI()
        {
            DrawPropertyField(BlinkSpeed, BlinkSpeedLabel);
            DrawPropertyField(BlinkRate, BlinkRateLabel);
        }

        private void DrawPropertyField(SerializedProperty property, GUIContent content)
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(property, content);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                (target as EyeAnimationHandler).Initialize();
            }
        }
    }
}
