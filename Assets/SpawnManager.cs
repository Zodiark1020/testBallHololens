using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private static SpawnManager _instance;

    public static SpawnManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<SpawnManager>();
            }

            return _instance;
        }
    }

    [SerializeField]
    private BallMovement objectToSpawn = null;

    public int instantiatedObjects;

    public void SpawnObject()
    {
        Vector3 spawnPoint = CameraCache.Main.transform.position + CameraCache.Main.transform.forward * 0.4f;
        Instantiate(objectToSpawn, spawnPoint, Quaternion.identity);
    }

    public void IncreaseObjectNumber()
    {
        instantiatedObjects++;
    }
    public void DecreaseObjectNumber()
    {
        instantiatedObjects--;
    }
}
