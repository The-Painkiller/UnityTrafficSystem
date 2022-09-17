using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleCarSpawn : MonoBehaviour
{
    [SerializeField]
    private Vehicle _vehicleToSpawn = null;

    [SerializeField]
    private int _numberOfVehiclesToSpawn = 2;

    [SerializeField]
    private float _timeDifferenceBetweenSpawn = 2f;

    private void Awake()
    {
        if (_vehicleToSpawn == null)
            return;

        StartCoroutine(CarSpawner());
    }

    private IEnumerator CarSpawner()
    {
        for (int i = 0; i < _numberOfVehiclesToSpawn; i++)
        {
            Instantiate<Vehicle>(_vehicleToSpawn);
            yield return new WaitForSeconds(_timeDifferenceBetweenSpawn);
        }
    }
}
