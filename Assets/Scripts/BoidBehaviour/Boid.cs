using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class Boid : MonoBehaviour
{
    private Vector3 _velocity = Vector3.zero;
    private Flock _flock = null;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _flock = transform.parent?.GetComponent<Flock>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetVelocity()
    {
        return _velocity;
    }
}
