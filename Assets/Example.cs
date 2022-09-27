using Metaverse;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Example : MonoBehaviour
{
    [SerializeField] private AvatarOutlookDataConfig outlookConfig;
    [SerializeField] private AvatarHairDataConfig hairConfig;
    [SerializeField] private AvatarGlassesDataConfig glassesConfig;
    [SerializeField] private AvatarBeardDataConfig beardConfig;
    [SerializeField] private GameObject outlookItemPrefab;
    [SerializeField] private GameObject hairItemPrefab;
    [SerializeField] private GameObject glassesItemPrefab;
    [SerializeField] private GameObject beardItemPrefab;

    [SerializeField] private SkinnedMeshRenderer head, body, top, bottom, footwear, hair, glasses, beard;
    private Material currentHeadMat;
    private Material cacheHeadMat;

    private void Start()
    {
        StartCoroutine(InitOutlook());
        StartCoroutine(InitHair());
        StartCoroutine(InitGlasses());
        StartCoroutine(InitBeard());
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
                        head.sharedMaterial = currentHeadMat != null ? currentHeadMat : outlookConfig.data[index].headMaterial;
                        cacheHeadMat = outlookConfig.data[index].headMaterial;

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

    private IEnumerator InitGlasses()
    {
        if (glassesConfig != null)
        {
            for (int i = 0; i < glassesConfig.data.Count; i++)
            {
                var data = glassesConfig.data[i];
                var instance = Instantiate(glassesItemPrefab);
                instance.transform.SetParent(glassesItemPrefab.transform.parent, false);
                instance.transform.SetSiblingIndex(glassesItemPrefab.transform.parent.childCount - 2);
                instance.GetComponent<Image>().sprite = data.thumb;
                instance.SetActive(true);
                instance.GetComponent<Toggle>().onValueChanged.AddListener(isOn =>
                {
                    if (isOn)
                    {
                        int index = instance.transform.GetSiblingIndex();
                        glasses.sharedMesh = glassesConfig.data[index].glassesMesh;
                        glasses.sharedMaterial = glassesConfig.data[index].glassesMaterial;
                    }
                });
                yield return null;
            }
        }
    }

    private IEnumerator InitBeard()
    {
        if (beardConfig != null)
        {
            for (int i = 0; i < beardConfig.data.Count; i++)
            {
                var data = beardConfig.data[i];
                var instance = Instantiate(beardItemPrefab);
                instance.transform.SetParent(beardItemPrefab.transform.parent, false);
                instance.transform.SetSiblingIndex(beardItemPrefab.transform.parent.childCount - 2);
                instance.GetComponent<Image>().sprite = data.thumb;
                instance.SetActive(true);
                instance.GetComponent<Toggle>().onValueChanged.AddListener(isOn =>
                {
                    if (isOn)
                    {
                        int index = instance.transform.GetSiblingIndex();
                        switch (data.type)
                        {
                            case AvatarBeardData.Type.Mesh:
                                beard.sharedMesh = beardConfig.data[index].beardMesh;
                                beard.sharedMaterial = beardConfig.data[index].beardMaterial;
                                currentHeadMat = null;
                                if (cacheHeadMat != null) head.sharedMaterial = cacheHeadMat;
                                break;
                            case AvatarBeardData.Type.Texture:
                                head.sharedMaterial = beardConfig.data[index].beardMaterial;
                                currentHeadMat = beardConfig.data[index].beardMaterial;
                                beard.sharedMesh = null;
                                beard.sharedMaterial = null;
                                break;
                        }
                    }
                });
                yield return null;
            }
        }
    }
}