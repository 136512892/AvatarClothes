using UnityEngine;
using UnityEditor;

namespace Metaverse
{
    /// <summary>
    /// 用于提取ReadyPlayerMe的Avatar胡须资源
    /// </summary>
    public class RPMAvatarBeardCollector
    {
        [MenuItem("Metaverse/Ready Player Me/Avatar Beard Collect #5", priority = 5)]
        public static void Execute()
        {
            //如果未选中任何物体
            if (Selection.activeGameObject == null) return;
            //弹出窗口 选择资源提取的路径
            string collectPath = EditorUtility.OpenFolderPanel("Beard Collector", Application.dataPath, null);
            //如果路径为空或无效 返回
            if (string.IsNullOrEmpty(collectPath)) return;
            //AssetDatabase路径
            collectPath = collectPath.Replace(Application.dataPath, "Assets");
            AssetDatabase.Refresh();
            if (!AssetDatabase.IsValidFolder(collectPath)) return;

            //胡须
            Transform head = Selection.activeGameObject.transform.Find("Wolf3D_Beard");
            if (head != null) Collect(collectPath, head.GetComponent<SkinnedMeshRenderer>());
        }

        public static void Collect(string path, SkinnedMeshRenderer smr)
        {
            //创建Mesh网格资产
            AssetDatabase.CreateAsset(Object.Instantiate(smr.sharedMesh), string.Format("{0}/mesh_beard.asset", path));
            //创建Material材质球资产
            Material material = Object.Instantiate(smr.sharedMaterial);
            AssetDatabase.CreateAsset(material, string.Format("{0}/mat_beard.asset", path));
            //获取材质球的依赖项路径
            string[] paths = AssetDatabase.GetDependencies(AssetDatabase.GetAssetPath(material));
            //遍历依赖项路径
            for (int i = 0; i < paths.Length; i++)
            {
                //AssetDatabase路径
                string p = paths[i].Replace(Application.dataPath, "Assets");
                //根据路径加载Texture贴图资源
                Texture tex = AssetDatabase.LoadAssetAtPath<Texture>(p);
                if (tex == null) continue;
                TextureImporter textureImporter = AssetImporter.GetAtPath(p) as TextureImporter;
                //主贴图
                if (textureImporter.textureType == TextureImporterType.Default)
                {
                    AssetDatabase.MoveAsset(p, string.Format("{0}/tex_beard_d.jpg", path));
                }
                //法线贴图
                if (textureImporter.textureType == TextureImporterType.NormalMap)
                {
                    AssetDatabase.MoveAsset(p, string.Format("{0}/tex_beard_n.jpg", path));
                }
            }
        }
    }
}