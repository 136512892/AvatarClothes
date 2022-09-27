using UnityEngine;
using UnityEditor;

namespace Metaverse
{
    /// <summary>
    /// 用于提取ReadyPlayerMe的Avatar服装资源
    /// </summary>
    public class RPMAvatarClothingCollector 
    {
        [MenuItem("Metaverse/Ready Player Me/Avatar Clothing Collect #1", priority = 1)]
        public static void Execute()
        {
            //如果未选中任何物体
            if (Selection.activeGameObject == null) return;
            //弹出窗口 选择资源提取的路径
            string collectPath = EditorUtility.OpenFolderPanel("Clothes Collector", Application.dataPath, null);
            //如果路径为空或无效 返回
            if (string.IsNullOrEmpty(collectPath)) return;
            //AssetDatabase路径
            collectPath = collectPath.Replace(Application.dataPath, "Assets");
            AssetDatabase.Refresh();
            if (!AssetDatabase.IsValidFolder(collectPath)) return;

            //头部
            Transform head = Selection.activeGameObject.transform.Find("Wolf3D_Head");
            if (head != null) Collect(collectPath, head.GetComponent<SkinnedMeshRenderer>(), "head");

            //身体
            Transform body = Selection.activeGameObject.transform.Find("Wolf3D_Body");
            if (body != null) Collect(collectPath, body.GetComponent<SkinnedMeshRenderer>(), "body");

            //上衣
            Transform top = Selection.activeGameObject.transform.Find("Wolf3D_Outfit_Top");
            if (top != null) Collect(collectPath, top.GetComponent<SkinnedMeshRenderer>(), "top");

            //裤子
            Transform bottom = Selection.activeGameObject.transform.Find("Wolf3D_Outfit_Bottom");
            if (bottom != null) Collect(collectPath, bottom.GetComponent<SkinnedMeshRenderer>(), "bottom");
            
            //鞋子
            Transform footwear = Selection.activeGameObject.transform.Find("Wolf3D_Outfit_Footwear");
            if (footwear != null) Collect(collectPath, footwear.GetComponent<SkinnedMeshRenderer>(), "footwear");
            
            //刷新
            AssetDatabase.Refresh();
        }

        public static void Collect(string path, SkinnedMeshRenderer smr, string suffix)
        {
            //创建Mesh网格资产
            AssetDatabase.CreateAsset(Object.Instantiate(smr.sharedMesh), string.Format("{0}/mesh_{1}.asset", path, suffix));
            //创建Material材质球资产
            Material material = Object.Instantiate(smr.sharedMaterial);
            AssetDatabase.CreateAsset(material, string.Format("{0}/mat_{1}.asset", path, suffix));
            //获取材质球的依赖项路径
            string[] paths = AssetDatabase.GetDependencies(AssetDatabase.GetAssetPath(material));
            //遍历依赖项路径
            for (int i = 0;i < paths.Length; i++)
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
                    AssetDatabase.MoveAsset(p, string.Format("{0}/tex_{1}_d.jpg", path, suffix));
                }
                //法线贴图
                if (textureImporter.textureType == TextureImporterType.NormalMap)
                {
                    AssetDatabase.MoveAsset(p, string.Format("{0}/tex_{1}_n.jpg", path, suffix));
                }
            }
        }
    }
}