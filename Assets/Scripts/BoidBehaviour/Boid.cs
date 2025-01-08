using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BoidBehaviour
{
    public class Boid : MonoBehaviour
    {
        private Vector3 _velocity;
        private Vector3 _prevPos;
        private Vector3 _prevDir;

        private Flock _flock;
        private float _obstacles;

        private Vector3 _separation = Vector3.zero;
        private Vector3 _cohesion = Vector3.zero;
        private Vector3 _alignment = Vector3.zero;
        private Vector3 _obstacleAvoidance = Vector3.zero;
        private List<Boid> _neighbours;
        [SerializeField] private Animator animator;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            _velocity = Vector3.forward;
            _flock = transform.root.GetComponent<Flock>();
            _prevPos = transform.position;
            _prevDir = transform.forward;
            _neighbours = new List<Boid>();
            animator = GetComponent<Animator>();

            Invoke(nameof(PlayIdleAnimation), Random.Range(0f, 10f));
        }

        // Update is called once per frame
        private void Update()
        {
            if (_flock is null) return;
            _velocity = (transform.position - _prevPos) / Time.deltaTime;
            _prevPos = transform.position;
            _prevDir = transform.forward;

            if (Time.frameCount % 10 == 0) // update every 10:th frame
                _neighbours = GetNeighbouringBoids();

            _obstacleAvoidance = GetObstacleAvoidanceVelocity() * _flock.obstacleAvoidanceFactor;
            _separation = GetSeparationDirection() * _flock.separationFactor;
            _cohesion = GetCohesionDirection() * _flock.cohesionFactor;
            _alignment = GetAlignmentDirection() * _flock.alignmentFactor;
            _velocity = _velocity.normalized * _flock.boidInertiaFactor;

            var newDir = _separation + _cohesion + _alignment + _velocity + _obstacleAvoidance;

            var rotationPlane = Vector3.Cross(_prevDir, newDir);
            var dirAngles = Vector3.SignedAngle(_prevDir, newDir, rotationPlane);
            var dirRotation = Quaternion.AngleAxis(
                Math.Abs(dirAngles) > _flock.maxRotation ? Math.Sign(dirAngles) * _flock.maxRotation : dirAngles,
                rotationPlane);
            var clampedDir = dirRotation * _prevDir;
            var modelRotation = Quaternion.LookRotation(clampedDir.normalized);

            transform.position = Vector3.MoveTowards(transform.position, transform.position + clampedDir,
                _flock.speed * Time.deltaTime);
            transform.rotation = modelRotation;
        }

        private void PlayIdleAnimation()
        {
            animator?.Play("Idle");
        }

        private Vector3 GetVelocity()
        {
            return _velocity;
        }

        private List<Boid> GetNeighbouringBoids()
        {
            var neighbours = new List<Boid>();
            foreach (var boid in _flock.boids)
                if ((transform.position - boid.transform.position).magnitude < _flock.boidInfluenceRadius &&
                    boid.GetInstanceID() != transform.GetInstanceID())
                    neighbours.Add(boid);

            return neighbours;
        }

        private Vector3 GetObstacleAvoidanceVelocity()
        {
            return _flock.obstacleColliders
                .Select(oc => Physics.ClosestPoint(transform.position, oc,
                    oc.transform.position, oc.transform.rotation))
                .Select(point => transform.position - point)
                .Where(distance => distance.magnitude <= _flock.obstacleInfluenceRadius)
                .Aggregate(Vector3.zero, (curr, dist) => curr + dist / dist.magnitude);
        }

        private Vector3 GetSeparationDirection()
        {
            var totalSeparation = Vector3.zero;
            foreach (var boid in _neighbours)
            {
                var separationVector = transform.position - boid.transform.position;
                var magnitude = separationVector.magnitude;
                if (magnitude > 0) totalSeparation += separationVector.normalized / magnitude;
            }

            return totalSeparation;
        }

        private Vector3 GetAlignmentDirection()
        {
            var alignmentDirection =
                _neighbours.Aggregate(Vector3.zero, (current, boid) => current + boid.GetVelocity());
            return alignmentDirection.normalized;
        }

        private Vector3 GetCohesionDirection()
        {
            var avgPos = _neighbours
                .Select(boid => boid.transform.position)
                .Aggregate(Vector3.zero, (current, pos) => current + pos) / (float)_neighbours.Count();
            return (avgPos - transform.position).normalized;
        }
    }
}