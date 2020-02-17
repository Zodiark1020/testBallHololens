using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPress : MonoBehaviour, IMixedRealityInputHandler
{
    public void OnInputDown(InputEventData eventData)
    {
        SpawnManager.Instance.SpawnObject();
    }

    public void OnInputUp(InputEventData eventData)
    {
        
    }
}
