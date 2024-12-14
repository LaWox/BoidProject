using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Flock : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Boid[] boids;
    public GameObject boidPrefab;
    
    [Range(1, 10)]
    public int totalBoids;
    [Range(0, 10)]
    public float seperationFactor;
    [Range(0, 10)]
    public float alignmentFactor;
    [Range(0, 10)]
    public float cohesionFactor;
    [Range(0, 1)]
    public float speed;   
    [Range(0, 100)]
    public float boidInertia;
    
    void Start()
    {
        boids = new Boid[totalBoids];
            
        GameObject tempPrefab;
        for (var i = 0; i < totalBoids; i++)
        {
            tempPrefab = Instantiate(boidPrefab, transform);
            tempPrefab.name = "Boid" + i;
            tempPrefab.transform.position = new Vector3(
                Random.Range(0f, 1f),
                Random.Range(0f, 1f),
                Random.Range(0f, 1f)
            );
            
            boids[i] = tempPrefab.GetComponent<Boid>();
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public Vector3 GetFlockVelocity()
    {
        var directions = boids.Select(b => b.GetVelocity());
        var avgDirection = directions.Aggregate(Vector3.zero, (current, direction) => current + direction);
        return avgDirection.normalized;
    }

    public Vector3 GetAvgPosition()
    {
        var positions = boids.Select(b => b.transform.position);
        var avgPosition = positions.Aggregate(Vector3.zero, (current, position) => current + position);
        return avgPosition / (float) totalBoids;
    }
}
