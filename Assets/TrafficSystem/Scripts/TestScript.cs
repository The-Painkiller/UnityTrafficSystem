using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestScript : MonoBehaviour
{
    [SerializeField]
    private Transform _destination;

    [SerializeField]
    private Transform[] _objects;

    [SerializeField]
    private NavMeshAgent _agent;

    private Vector3 _objectDirection01;
    private Vector3 _objectDirection02;

    void Start()
    {
        
    }

    [ContextMenu("Drive")]
    public void Drive()
    {
        _agent.SetDestination(_destination.position);
    }

    // Update is called once per frame
    void Update()
    {
        _objectDirection01 = _objects[1].position - _objects[0].position;
        _objectDirection02 = _objects[1].position - _objects[2].position;

        Debug.Log(Vector3.Angle(_objectDirection01, _objectDirection02));
    }
}
