using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Metaverse
{
    [Serializable]
    public class AvatarOutlookData
    {
        public Sprite thumb;

        public Mesh headMesh;
        public Material headMaterial;

        public Mesh bodyMesh;
        public Material bodyMaterial;

        public Mesh topMesh;
        public Material topMaterial;

        public Mesh bottomMesh;
        public Material bottomMaterial;

        public Mesh footwearMesh;
        public Material footwearMaterial;
    }

    [CreateAssetMenu]
    public class AvatarOutlookDataConfig : ScriptableObject
    {
        public List<AvatarOutlookData> data = new List<AvatarOutlookData>(0);
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(AvatarOutlookDataConfig))]
    public class AvatarOutlookDataConfigEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            OnDragRectGUI();
        }

        private void OnDragRectGUI()
        {
            GUILayout.Space(10f);
            GUILayout.BeginHorizontal();
            GUILayout.Label(GUIContent.none, GUILayout.ExpandWidth(true));
            Rect lastRect = GUILayoutUtility.GetLastRect();
            var dropRect = new Rect(lastRect.x + 2f, lastRect.y - 2f, lastRect.width - 20f, 20f);
            bool containsMouse = dropRect.Contains(Event.current.mousePosition);
            if (containsMouse)
            {
                switch (Event.current.type)
                {
                    case EventType.DragUpdated:
                        bool flag = DragAndDrop.objectReferences.OfType<Mesh>().Any() 
                            || DragAndDrop.objectReferences.OfType<Material>().Any()
                            || DragAndDrop.objectReferences.OfType<Sprite>().Any();
                        DragAndDrop.visualMode = flag ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.Rejected;
                        Event.current.Use();
                        Repaint();
                        break;
                    case EventType.DragPerform:
                        IEnumerable<Mesh> meshes = DragAndDrop.objectReferences.OfType<Mesh>();
                        IEnumerable<Material> materials = DragAndDrop.objectReferences.OfType<Material>();
                        IEnumerable<Texture> textures = DragAndDrop.objectReferences.OfType<Texture>();

                        if (meshes.Any() || materials.Any() || textures.Any())
                        {
                            Undo.RecordObject(target, "Add");
                            AvatarOutlookData data = new AvatarOutlookData();
                            (target as AvatarOutlookDataConfig).data.Add(data);
                            foreach (var mesh in meshes)
                            {
                                if (mesh.name == "mesh_head") data.headMesh = mesh;
                                else if (mesh.name == "mesh_body") data.bodyMesh = mesh;
                                else if (mesh.name == "mesh_top") data.topMesh = mesh;
                                else if (mesh.name == "mesh_bottom") data.bottomMesh = mesh;
                                else if (mesh.name == "mesh_footwear") data.footwearMesh = mesh;
                            }

                            foreach (var material in materials)
                            {
                                if (material.name == "mat_head") data.headMaterial = material;
                                else if (material.name == "mat_body") data.bodyMaterial = material;
                                else if (material.name == "mat_top") data.topMaterial = material;
                                else if (material.name == "mat_bottom") data.bottomMaterial = material;
                                else if (material.name == "mat_footwear") data.footwearMaterial = material;
                            }

                            foreach (var texture in textures)
                            {
                                string path = AssetDatabase.GetAssetPath(texture);
                                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                                if (importer != null && importer.textureType == TextureImporterType.Sprite)
                                {
                                    data.thumb = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                                }
                            }

                            EditorUtility.SetDirty(target);
                        }
                        Event.current.Use();
                        Repaint();
                        break;
                }
            }
            Color color = GUI.color;
            GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, containsMouse ? .9f : .5f);
            GUI.Box(dropRect, "Drop Here", new GUIStyle(GUI.skin.box) { fontSize = 10 });
            GUI.color = color;
            GUILayout.EndHorizontal();
        }
    }
#endif
}