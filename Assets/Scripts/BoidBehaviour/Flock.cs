using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Flock : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private GameObject[] _boids;
    public GameObject boidPrefab;
    
    public int totalBoids;
    public float seperation;
    public float speed;
    
    void Start()
    {
        _boids = new GameObject[totalBoids];
        for (var i = 0; i < totalBoids; i++)
        {
            _boids[i] = Instantiate(boidPrefab, new Vector3(
                Random.Range(0f, 1f), // Random x
                Random.Range(0f, 1f), // Random y
                Random.Range(0f, 1f)  // Random z
            ), Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Vector3 GetAvgVelocity()
    {
        if (_boids == null || _boids.Length == 0)
        {
            return Vector3.zero;
        }
        var directions = _boids.Select(boid => boid.GetComponent<Boid>().GetVelocity());
        var avgDirection = directions.Aggregate(Vector3.zero, (current, direction) => current + direction);
        return avgDirection / (float) totalBoids;
    }
}
