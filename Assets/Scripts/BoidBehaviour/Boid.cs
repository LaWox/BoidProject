using System;
using System.Linq;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

public class Boid : MonoBehaviour
{
    private Vector3 _velocity;
    private Vector3 _prevPos;

    private Flock _flock;
    private Vector3 _separation = Vector3.zero;
    private Vector3 _cohesion = Vector3.zero;
    private Vector3 _alignment = Vector3.zero;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _velocity = Vector3.forward;
        _flock = transform.parent.GetComponent<Flock>();
        _prevPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (_flock is null){return;}
        _velocity = (transform.position - _prevPos) / Time.deltaTime;
        _prevPos = transform.position;
        
        
        _separation = GetSeparationDirection() * _flock.seperationFactor;
        _cohesion = GetCohesionDirection() * _flock.cohesionFactor;
        _alignment = GetAlignmentDirection() * _flock.alignmentFactor;
        _velocity = _velocity.normalized * _flock.boidInertia;

        Debug.DrawLine(_separation, transform.position, Color.red);
        Debug.DrawLine(_cohesion, transform.position, Color.green);
        Debug.DrawLine(_alignment, transform.position, Color.blue);
        Debug.DrawLine(_velocity, transform.position, Color.yellow);

        var direction = (_separation + _cohesion + _alignment + _velocity).normalized;
        
        transform.position = Vector3.MoveTowards(transform.position, direction, _flock.speed * Time.deltaTime); 
    }

    public Vector3 GetVelocity()
    {
        return _velocity;
    }
    
    private Vector3 GetSeparationDirection()
    {
        var totalSeparation = Vector3.zero;
        foreach (var boid in _flock.boids)
        {
            var separationVector = transform.position - boid.transform.position;
            var magnitude = separationVector.magnitude;
            if (magnitude > 0)
            {
                totalSeparation += separationVector.normalized / magnitude;
            }
        }
        return totalSeparation;
    }

    private Vector3 GetAlignmentDirection()
    {
        return (_flock.GetFlockVelocity()).normalized;
    }

    private Vector3 GetCohesionDirection()
    {
        return (_flock.GetAvgPosition() - transform.position).normalized;
    }
}
