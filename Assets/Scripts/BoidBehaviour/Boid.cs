using System;
using System.Linq;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

public class Boid : MonoBehaviour
{
    private Vector3 _velocity;
    private Vector3 _targetVelocity = Vector3.zero;
    
    private Vector3 _targetPos = Vector3.zero;
    private Flock _flock;
    private Vector3 _separation = Vector3.zero;
    private Vector3 _cohesion = Vector3.zero;
    private Vector3 _alignment = Vector3.zero;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _velocity = transform.position;
        _flock = transform.parent.GetComponent<Flock>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_flock is null){return;}
        _velocity -= transform.position;
        _separation = GetSeparationDirection() * _flock.seperationFactor;
        _cohesion = GetCohesionDirection() * _flock.cohesionFactor;
        _alignment = GetAlignmentDirection() * _flock.alignmentFactor;

        Debug.DrawLine(_separation, transform.position, Color.red);
        Debug.DrawLine(_cohesion, transform.position, Color.green);
        Debug.DrawLine(_alignment, transform.position, Color.blue);
        
        transform.position += (_separation + _cohesion + _alignment).normalized * _flock.speed; 
    }

    public Vector3 GetVelocity()
    {
        return _velocity;
    }
    
    private Vector3 GetSeparationDirection()
    {
        var separation = Vector3.zero;
        foreach (var boid in _flock.boids)
        {
            separation += (transform.position - boid.transform.position);
        }
        return (separation - transform.position).normalized;
    }

    private Vector3 GetAlignmentDirection()
    {
        return (_flock.GetAvgVelocity() - transform.position).normalized;
    }

    private Vector3 GetCohesionDirection()
    {
        return (_flock.GetAvgPosition() - transform.position).normalized;
    }
}
