using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace BoidBehaviour
{
    public class Flock : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public Boid[] boids;
        public GameObject boidPrefab;
        public List<Collider> obstacleColliders;

        [Range(1, 10)] public int totalBoids;
        [Range(0, 1)] public float separationFactor;
        [Range(0, 10)] public float alignmentFactor;
        [Range(0, 10)] public float cohesionFactor;
        [Range(0, 10)] public float obstacleAvoidanceFactor;
        [Range(0, 1)] public float boidInertiaFactor;
        [Range(0, 1)] public float speed;
        [Range(0, 45)] public float maxRotation;

        [Range(0, 10)] public float boidInfluenceRadius;
        [Range(0, 1)] public float obstacleInfluenceRadius;

        private void Start()
        {
            boids = new Boid[totalBoids];
            obstacleColliders = new List<Collider>();

            var obstacles = GameObject.Find("Obstacles");
            if (obstacles != null)
                foreach (Transform child in obstacles.transform)
                    obstacleColliders.Add(child.GetComponent<Collider>());

            for (var i = 0; i < totalBoids; i++)
            {
                var tempPrefab = Instantiate(boidPrefab, transform);
                tempPrefab.name = "Boid" + i;
                tempPrefab.transform.localPosition = new Vector3(
                    Random.Range(0f, 1f),
                    Random.Range(0f, 1f),
                    Random.Range(0f, 1f)
                );

                boids[i] = tempPrefab.GetComponentInChildren<Boid>();
            }
        }
    }
}