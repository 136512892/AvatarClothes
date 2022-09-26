using Metaverse;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Example : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private AvatarOutlookDataConfig config;
    [SerializeField] private SkinnedMeshRenderer head, body, top, bottom, footwear;

    private IEnumerator Start()
    {
        if (config != null)
        {
            for (int i = 0; i < config.data.Count; i++)
            {
                var data = config.data[i];
                var instance = Instantiate(itemPrefab);
                instance.transform.SetParent(itemPrefab.transform.parent, false);
                instance.GetComponent<Image>().sprite = data.thumb;
                instance.SetActive(true);
                instance.GetComponent<Toggle>().onValueChanged.AddListener(isOn =>
                {
                    if (isOn)
                    {
                        Apply(instance.transform.GetSiblingIndex() - 1);
                    }
                });
                yield return null;
            }
        }
    }

    public void Apply(int index)
    {
        head.sharedMesh = config.data[index].headMesh;
        head.sharedMaterial = config.data[index].headMaterial;

        body.sharedMesh = config.data[index].bodyMesh;
        body.sharedMaterial = config.data[index].bodyMaterial;

        top.sharedMesh = config.data[index].topMesh;
        top.sharedMaterial = config.data[index].topMaterial;

        bottom.sharedMesh = config.data[index].bottomMesh;
        bottom.sharedMaterial = config.data[index].bottomMaterial;

        footwear.sharedMesh = config.data[index].footwearMesh;
        footwear.sharedMaterial = config.data[index].footwearMaterial;
    }
}