using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using System;

public class BallMovement : MonoBehaviour, IMixedRealityInputHandler
{
    [SerializeField]
    private Rigidbody rigidBody = null;
    public float speed = 7f;
    private float placementDistance = 30f;
    private int maxHeight = 20;
    private Vector3 height = Vector3.zero;
    [SerializeField]
    private float fallCheckDistance = 0.2f;
    private int spatialLayer;

    private Coroutine floorCheckCo;
    private Coroutine moveTowardsPointerCo;

    private bool isClicked = false;

    private void OnEnable()
    {
        height.y = transform.localScale.y;

        CoreServices.InputSystem.RegisterHandler<IMixedRealityInputHandler>(this);

        spatialLayer = GetSpatialAwarenessLayer();

        floorCheckCo = StartCoroutine(FloorCheckCoroutine());
    }

    private void OnDisable()
    {
        CoreServices.InputSystem.UnregisterHandler<IMixedRealityInputHandler>(this);

        if (floorCheckCo != null)
        {
            StopCoroutine(floorCheckCo);
        }
    }

    private IEnumerator FloorCheckCoroutine()
    {
        while (true)
        {
            FloorCheck();
            yield return new WaitForSeconds(0.5f);
        }
    }

    public int GetSpatialAwarenessLayer()
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
        if (Physics.Raycast(transform.position + rigidBody.velocity.normalized * fallCheckDistance + height, Vector3.down, maxHeight, 1 << spatialLayer))
        {
            Debug.DrawRay(transform.position + rigidBody.velocity.normalized * fallCheckDistance + height, Vector3.down, Color.blue);
        }
        else
        {
            rigidBody.velocity = Vector3.zero;
        }
    }

    private IEnumerator MoveTowardsPointerCoroutine()
    {
        while (true)
        {
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

            yield return new WaitForFixedUpdate();
        }
    }

    public void OnInputUp(InputEventData eventData)
    {
        if (!isClicked)
            return;

        if (moveTowardsPointerCo != null)
        {
            StopCoroutine(moveTowardsPointerCo);
            Debug.Log("Coroutine stopped");
        }

        isClicked = false;
    }

    public void OnInputDown(InputEventData eventData)
    {
        if (isClicked)
            return;

        moveTowardsPointerCo = StartCoroutine(MoveTowardsPointerCoroutine());
        isClicked = true;
        Debug.Log("Coroutine started");
    }

    private void FloorCheck()
    {
        rigidBody.isKinematic = !Physics.Raycast(transform.position + height, Vector3.down, maxHeight, 1 << spatialLayer);
        Debug.DrawRay(transform.position + height, Vector3.down, Color.cyan);
    }
}
