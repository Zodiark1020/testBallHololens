using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabColorChange : MonoBehaviour, IMixedRealityPointerHandler
{
    [SerializeField]
    public Material material;
    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        Material grabbableObjectMaterial = this.GetComponent<Material>();

        if(eventData.Pointer is SpherePointer)
        {
            grabbableObjectMaterial = material;
        }
    }


    public void OnPointerClicked(MixedRealityPointerEventData eventData) { }
    public void OnPointerDragged(MixedRealityPointerEventData eventData) { }
    public void OnPointerUp(MixedRealityPointerEventData eventData) { }
}

