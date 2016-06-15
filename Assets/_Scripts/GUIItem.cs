using UnityEngine;
using VRStandardAssets.Utils;

public class GUIItem : MonoBehaviour
{
    [SerializeField] VRInteractiveItem interactiveItem;
    Material material;
    bool isDefaultColor;

    void Start()
    {
        material = GetComponent<Renderer>().material;
        material.color = Color.white;
        isDefaultColor = true;
    }

    void OnEnable()
    {
        interactiveItem.OnClick += toggleColor;
    }

    void OnDisable()
    {
        interactiveItem.OnClick -= toggleColor;
    }

    void toggleColor()
    {
        material.color = isDefaultColor ? Color.red : Color.white;
        isDefaultColor = !isDefaultColor;
    }
}
