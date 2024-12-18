using UnityEngine;
using Random = UnityEngine.Random;

public class Flock : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Boid[] boids;
    public GameObject boidPrefab;

    [Range(1, 10)] public int totalBoids;
    [Range(0, 1)] public float seperationFactor;
    [Range(0, 10)] public float alignmentFactor;
    [Range(0, 10)] public float cohesionFactor;
    [Range(0, 10)] public float influenceRadius;
    [Range(0, 1)] public float speed;
    [Range(0, 1)] public float boidInertia;

    private void Start()
    {
        boids = new Boid[totalBoids];

        for (var i = 0; i < totalBoids; i++)
        {
            var tempPrefab = Instantiate(boidPrefab, transform);
            tempPrefab.name = "Boid" + i;
            tempPrefab.transform.position = new Vector3(
                Random.Range(0f, 1f),
                Random.Range(0f, 1f),
                Random.Range(0f, 1f)
            );

            boids[i] = tempPrefab.GetComponent<Boid>();
        }
    }
}