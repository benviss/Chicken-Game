using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CoyoteBoidManager : MonoBehaviour
{
    private static CoyoteBoidManager _instance;
    public static CoyoteBoidManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }


    const int threadGroupSize = 1024;

    public BoidSettings settings;
    public ComputeShader compute;
    public Transform target;
    public GameObject boidPrefab;

    Boid[] boids;

    void Start()
    {
        //InitilizeBoids();
    }

    public void InitilizeBoids()
    {
        if (boidPrefab.tag == "Coyote") {

        }
        var boidObjects = GameObject.FindGameObjectsWithTag(boidPrefab.tag);
        boids = boidObjects.Select(gObj => gObj.GetComponent<Boid>()).ToArray();

        foreach (Boid boid in boids) {
            if (boid == null) throw new System.Exception("Non-boid found in Boid Manager tags");

            InitBoid(boid);
        }
    }

    public void InitBoid(Boid b)
    {
        b.Initialize(settings, target);
    }

    void Update()
    {
        if (boids != null) {

            int numBoids = boids.Length;
            var boidData = new BoidData[numBoids];

            for (int i = 0; i < boids.Length; i++) {
                boidData[i].position = boids[i].position;
                boidData[i].direction = boids[i].forward;
            }

            var boidBuffer = new ComputeBuffer(numBoids, BoidData.Size);
            boidBuffer.SetData(boidData);

            compute.SetBuffer(0, "boids", boidBuffer);
            compute.SetInt("numBoids", boids.Length);
            compute.SetFloat("viewRadius", settings.perceptionRadius);
            compute.SetFloat("avoidRadius", settings.avoidanceRadius);

            int threadGroups = Mathf.CeilToInt(numBoids / (float)threadGroupSize);
            compute.Dispatch(0, threadGroups, 1, 1);

            boidBuffer.GetData(boidData);

            for (int i = 0; i < boids.Length; i++) {
                boids[i].avgFlockHeading = boidData[i].flockHeading;
                boids[i].centreOfFlockmates = boidData[i].flockCentre;
                boids[i].avgAvoidanceHeading = boidData[i].avoidanceHeading;
                boids[i].numPerceivedFlockmates = boidData[i].numFlockmates;

                boids[i].UpdateBoid();
            }

            boidBuffer.Release();
        }
    }

    public struct BoidData
    {
        public Vector3 position;
        public Vector3 direction;

        public Vector3 flockHeading;
        public Vector3 flockCentre;
        public Vector3 avoidanceHeading;
        public int numFlockmates;

        public static int Size
        {
            get {
                return sizeof(float) * 3 * 5 + sizeof(int);
            }
        }
    }

}
