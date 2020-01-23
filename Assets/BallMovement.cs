using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;


public class BallMovement : MonoBehaviour, IMixedRealityInputHandler
{
    [SerializeField]
    private Rigidbody rigidBody = null;
    public float speed = 10;
    private int placementDistance = 15;
    private int maxHeight = 3;
    private Vector3 height = Vector3.zero;
    [SerializeField]
    private float fallCheckDistance = 0.2f;
    private void OnEnable()
    {
        height.y = transform.localScale.y;
        CoreServices.InputSystem.RegisterHandler<IMixedRealityInputHandler>(this);
    }

    private void OnDisable()
    {
        CoreServices.InputSystem.UnregisterHandler<IMixedRealityInputHandler>(this);
    }

    private int GetSpatialAwarenessLayer()
    {
        /*MixedRealitySpatialAwarenessMeshObserverProfile spatialMappingConfig =
            CoreServices.SpatialAwarenessSystem.ConfigurationProfile as MixedRealitySpatialAwarenessMeshObserverProfile;*/
        var spatialAwarenessService = CoreServices.SpatialAwarenessSystem;
        var dataProviderAccess = spatialAwarenessService as IMixedRealityDataProviderAccess;
        var meshObserver = dataProviderAccess.GetDataProvider<IMixedRealitySpatialAwarenessMeshObserver>();
        return meshObserver.MeshPhysicsLayer;
    }

    void FixedUpdate()
    {
        int spatialLayer = GetSpatialAwarenessLayer();

        if (Physics.Raycast(transform.position + rigidBody.velocity.normalized * fallCheckDistance + height, Vector3.down, maxHeight, 1 << spatialLayer))
        {
            //Debug.Log(spatialLayer);
            Debug.DrawRay(transform.position + rigidBody.velocity.normalized * fallCheckDistance + height, Vector3.down, Color.blue);

            if (ControllerSourceManager.Instance.TryGetPointer(out Vector3 pointerPosition, out Quaternion pointerRotation))
            {
                Vector3 pointerDirection = pointerRotation * Vector3.forward;

                if (Physics.Raycast(pointerPosition, pointerDirection, out RaycastHit rayHit, placementDistance, 1 << spatialLayer))
                {
                    Vector3 direction = (rayHit.point - transform.position).normalized;
                    if (Physics.Raycast(transform.position + direction * fallCheckDistance + height, Vector3.down, maxHeight, 1 << spatialLayer))
                    {
                        rigidBody.AddForce(direction * speed);
                        Debug.DrawRay(transform.position, direction * 10, Color.magenta);
                        Debug.DrawRay(transform.position + direction * fallCheckDistance + height, Vector3.down, Color.red);
                    }
                }
            }
        }
        else
        {
            rigidBody.velocity = Vector3.zero;
        }
    }

    public void OnInputUp(InputEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnInputDown(InputEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}
