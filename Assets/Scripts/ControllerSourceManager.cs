using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;
using UnityEngine.XR;

public class ControllerSourceManager : MonoBehaviour, IMixedRealitySourceStateHandler, IMixedRealityInputHandler
{
    // declare delegate 
    public delegate void OnControllerStatusChange(Handedness handedness);

    private IMixedRealityController detectedController;
    private IMixedRealityHand detectedHand;
    private MixedRealityPose handPose;

    private IMixedRealityController leftController, rightController;
    private IMixedRealityHand leftHand, rightHand;

    public event OnControllerStatusChange OnControllerFound;
    public event OnControllerStatusChange OnControllerLost;

    public static ControllerSourceManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        CoreServices.InputSystem.RegisterHandler<IMixedRealityInputHandler>(this);
        CoreServices.InputSystem.RegisterHandler<IMixedRealitySourceStateHandler>(this);
    }

    private void OnDisable()
    {
        CoreServices.InputSystem.UnregisterHandler<IMixedRealityInputHandler>(this);
        CoreServices.InputSystem.UnregisterHandler<IMixedRealitySourceStateHandler>(this);
    }

    public bool TryGetControllerPose(TrackedHandJoint handJointToTrack, out Vector3 position, out Quaternion rotation, bool getPointerOnly = false)
    {
        bool retrieved = false;

        position = Vector3.zero;
        rotation = Quaternion.identity;

        if (!XRSettings.enabled)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            position = ray.GetPoint(0.5f) + new Vector3(-0.01f, -0.015f, 0);
            rotation = Quaternion.Euler(Camera.main.transform.eulerAngles + new Vector3(-45, -45, 0));//Camera.main.transform.rotation + Quaternion.AngleAxis(90, Vector3.up) + Quaternion.AngleAxis(45, Vector3.left);

            retrieved = true;
        }
        else
        {
            if (getPointerOnly)
            {
                try
                {
                    position = (detectedHand ?? detectedController).InputSource.Pointers[0].Position;
                    rotation = (detectedHand ?? detectedController).InputSource.Pointers[0].Rotation;
                    retrieved = true;
                }
                catch (Exception)
                {
                    retrieved = false;
                }
            }
            else
            {
                retrieved = TryGetSpecificControllerPose(Handedness.None, handJointToTrack, out position, out rotation);
            }
        }

        return retrieved;
    }

    public bool TryGetSpecificControllerPose(Handedness handedness, TrackedHandJoint handJointToTrack, out Vector3 position, out Quaternion rotation)
    {
        bool retrieved = false;

        position = Vector3.zero;
        rotation = Quaternion.identity;

        IMixedRealityHand currentHand = detectedHand;
        IMixedRealityController currentController = detectedController;

        if (handedness == Handedness.Left)
        {
            currentHand = leftHand;
            currentController = leftController;
        }
        else if (handedness == Handedness.Right)
        {
            currentHand = rightHand;
            currentController = rightController;
        }

        if (currentHand != null && currentHand.TryGetJoint(handJointToTrack, out handPose))
        {
            position = handPose.Position;
            rotation = handPose.Rotation;
            retrieved = true;
        }
        else if (currentController != null)
        {
            try
            {
                position = currentController.InputSource.Pointers[0].Position;
                rotation = currentController.InputSource.Pointers[0].Rotation;
                retrieved = true;
            }
            catch (Exception)
            {
                retrieved = false;
            }
        }

        return retrieved;
    }

    public bool TryGetControllerPose(out Vector3 position)
    {
        return TryGetControllerPose(TrackedHandJoint.Palm, out position, out _);
    }

    public bool TryGetControllerPose(out Quaternion rotation)
    {
        return TryGetControllerPose(TrackedHandJoint.Palm, out _, out rotation);
    }

    public bool TryGetControllerPose(out Vector3 position, out Quaternion rotation)
    {
        return TryGetControllerPose(TrackedHandJoint.Palm, out position, out rotation);
    }

    public bool TryGetPointer(out Vector3 position, out Quaternion rotation)
    {
        bool retrieved = false;

        position = Vector3.zero;
        rotation = Quaternion.identity;

        try
        {
            position = (detectedHand ?? detectedController).InputSource.Pointers[0].Position;
            rotation = (detectedHand ?? detectedController).InputSource.Pointers[0].Rotation;
            retrieved = true;
        }
        catch (Exception)
        {
            retrieved = false;
        }

        return retrieved;
    }

    public bool TryGetPointer(out Vector3 position)
    {
        return TryGetPointer(out position, out _);
    }

    public bool TryGetPointer(out Quaternion rotation)
    {
        return TryGetPointer(out _, out rotation);
    }

    public bool TryGetCursor(out Vector3 position, out Quaternion rotation)
    {
        bool retrieved = false;

        position = Vector3.zero;
        rotation = Quaternion.identity;

        try
        {
            position = (detectedHand ?? detectedController).InputSource.Pointers[0].BaseCursor.Position;
            rotation = (detectedHand ?? detectedController).InputSource.Pointers[0].BaseCursor.Rotation;
            retrieved = true;
        }
        catch (Exception)
        {
            retrieved = false;
        }

        return retrieved;
    }

    public bool TryGetCursor(out Vector3 position)
    {
        return TryGetCursor(out position, out _);
    }

    public bool TryGetCursor(out Quaternion rotation)
    {
        return TryGetCursor(out _, out rotation);
    }

    internal void ShowController(bool showController)
    {
        if (detectedController != null)
        {
            try
            {
                detectedController.Visualizer.GameObjectProxy.SetActive(showController);
            }
            catch (Exception)
            {

            }
        }
    }

    private void SetController(IMixedRealityController controller)
    {
        IMixedRealityHand hand = controller as IMixedRealityHand;
        IMixedRealityController mrcontroller = controller as IMixedRealityController;

        if (hand != null)
        {
            try
            {
                detectedHand = hand;

                if (controller.ControllerHandedness == Handedness.Left)
                {
                    leftHand = hand;
                }
                else if (controller.ControllerHandedness == Handedness.Right)
                {
                    rightHand = hand;
                }
            }
            catch (Exception)
            {
                //Debug.Log("Missed a hand, retrying");
            }
        }
        else if (mrcontroller != null)
        {
            try
            {
                detectedController = mrcontroller;

                if (controller.ControllerHandedness == Handedness.Left)
                {
                    leftController = mrcontroller;
                }
                else if (controller.ControllerHandedness == Handedness.Right)
                {
                    rightController = mrcontroller;
                }
            }
            catch (Exception)
            {
                //Debug.Log("Missed a controller, retrying");
            }
        }
    }

    public void OnSourceDetected(SourceStateEventData eventData)
    {
        SetController(eventData.Controller);

        if (eventData.Controller != null)
        {
            OnControllerFound?.Invoke(eventData.Controller.ControllerHandedness);
        }
    }

    public void OnSourceLost(SourceStateEventData eventData)
    {
        if (eventData.Controller != null)
        {
            OnControllerLost?.Invoke(eventData.Controller.ControllerHandedness);
        }
    }

    public void OnInputUp(InputEventData eventData)
    {
    }

    public void OnInputDown(InputEventData eventData)
    {
        try
        {
            foreach (var p in eventData.InputSource.Pointers)
            {
                if (p.Result != null)
                {
                    SetController(p.Controller);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Could not start animation drag! " + e.Message, transform);
        }
    }
}