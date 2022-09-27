using Metaverse;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Example : MonoBehaviour
{
    [SerializeField] private AvatarOutlookDataConfig outlookConfig;
    [SerializeField] private AvatarHairDataConfig hairConfig;
    [SerializeField] private GameObject outlookItemPrefab;
    [SerializeField] private GameObject hairItemPrefab;

    [SerializeField] private SkinnedMeshRenderer head, body, top, bottom, footwear, hair;

    private void Start()
    {
        StartCoroutine(InitOutlook());
        StartCoroutine(InitHair());
    }

    private IEnumerator InitOutlook()
    {
        if (outlookConfig != null)
        {
            for (int i = 0; i < outlookConfig.data.Count; i++)
            {
                var data = outlookConfig.data[i];
                var instance = Instantiate(outlookItemPrefab);
                instance.transform.SetParent(outlookItemPrefab.transform.parent, false);
                instance.transform.SetSiblingIndex(outlookItemPrefab.transform.parent.childCount - 2);
                instance.GetComponent<Image>().sprite = data.thumb;
                instance.SetActive(true);
                instance.GetComponent<Toggle>().onValueChanged.AddListener(isOn =>
                {
                    if (isOn)
                    {
                        int index = instance.transform.GetSiblingIndex();
                        head.sharedMesh = outlookConfig.data[index].headMesh;
                        head.sharedMaterial = outlookConfig.data[index].headMaterial;

                        body.sharedMesh = outlookConfig.data[index].bodyMesh;
                        body.sharedMaterial = outlookConfig.data[index].bodyMaterial;

                        top.sharedMesh = outlookConfig.data[index].topMesh;
                        top.sharedMaterial = outlookConfig.data[index].topMaterial;

                        bottom.sharedMesh = outlookConfig.data[index].bottomMesh;
                        bottom.sharedMaterial = outlookConfig.data[index].bottomMaterial;

                        footwear.sharedMesh = outlookConfig.data[index].footwearMesh;
                        footwear.sharedMaterial = outlookConfig.data[index].footwearMaterial;
                    }
                });
                yield return null;
            }
        }
    }

    private IEnumerator InitHair()
    {
        if (hairConfig != null)
        {
            for (int i = 0; i < hairConfig.data.Count; i++)
            {
                var data = hairConfig.data[i];
                var instance = Instantiate(hairItemPrefab);
                instance.transform.SetParent(hairItemPrefab.transform.parent, false);
                instance.transform.SetSiblingIndex(hairItemPrefab.transform.parent.childCount - 2);
                instance.GetComponent<Image>().sprite = data.thumb;
                instance.SetActive(true);
                instance.GetComponent<Toggle>().onValueChanged.AddListener(isOn =>
                {
                    if (isOn)
                    {
                        int index = instance.transform.GetSiblingIndex();
                        hair.sharedMesh = hairConfig.data[index].hairMesh;
                        hair.sharedMaterial = hairConfig.data[index].hairMaterial;
                    }
                });
                yield return null;
            }
        }
    }
}