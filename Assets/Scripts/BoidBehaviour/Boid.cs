using System;
using System.Collections.Generic;
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
    private List<Boid> _neighbours;
    private float _scaling;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _velocity = Vector3.forward;
        _flock = transform.parent.GetComponent<Flock>();
        _prevPos = transform.position;
        _neighbours = new List<Boid>();
        _scaling = _flock is not null ?  _flock.alignmentFactor + _flock.cohesionFactor + _flock.alignmentFactor + _flock.boidInertia : 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (_flock is null){return;}
        _velocity = (transform.position - _prevPos) / Time.deltaTime;
        _prevPos = transform.position;
        
        if (Time.frameCount % 10 == 0) // update every 10:th frame
        {
            _neighbours = GetNeighbours();
        }
        
        
        _separation = GetSeparationDirection() * _flock.seperationFactor;
        _cohesion = GetCohesionDirection() * _flock.cohesionFactor;
        _alignment = GetAlignmentDirection() * _flock.alignmentFactor;
        _velocity = _velocity.normalized * _flock.boidInertia;

        Debug.DrawLine(transform.position, transform.position + _separation, Color.red);
        Debug.DrawLine(transform.position, transform.position + _cohesion, Color.green);
        Debug.DrawLine(transform.position, transform.position +_alignment, Color.blue);
        Debug.DrawLine(transform.position, transform.position + _velocity, Color.yellow);
        Debug.DrawLine(transform.position, transform.position + Vector3.forward * _flock.influenceRadius, Color.magenta);
        

        var direction = (_separation + _cohesion + _alignment + _velocity) / _scaling;
        
        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, _flock.speed * Time.deltaTime); 
    }

    public Vector3 GetVelocity()
    {
        return _velocity;
    }
    
    private List<Boid> GetNeighbours()
    {
        var neighbours = new List<Boid>();
        foreach (var boid in _flock.boids)
        {
            if ((transform.position - boid.transform.position).magnitude < _flock.influenceRadius && boid.GetInstanceID() != transform.GetInstanceID())
            {
                neighbours.Add(boid);
            }
        }
        return neighbours;
    }
    
    private Vector3 GetSeparationDirection()
    {
        var totalSeparation = Vector3.zero;
        foreach (var boid in _neighbours)
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
        var alignmentDirection = _neighbours.Aggregate(Vector3.zero, (current, boid) => current + boid.GetVelocity());
        return alignmentDirection.normalized;
    }

    private Vector3 GetCohesionDirection()
    {
        var avgPos = _neighbours
            .Select(boid => boid.transform.position)
            .Aggregate(Vector3.zero, (current, pos) => current + pos) / (float) _neighbours.Count();
        return (avgPos - transform.position).normalized;
    }
}
