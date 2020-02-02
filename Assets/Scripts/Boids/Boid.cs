//COURTESY OF SEBASTIAN LAGUE A HERO AMONG MEN
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour, IActor
{
    [HideInInspector]
    public BoidSettings settings;

    // State
    [HideInInspector]
    public Vector3 position;
    [HideInInspector]
    public Vector3 forward;
    [HideInInspector]
    public Vector3 velocity;

    // To update:
    public Vector3 acceleration;
    [HideInInspector]
    public Vector3 avgFlockHeading;
    [HideInInspector]
    public Vector3 avgAvoidanceHeading;
    [HideInInspector]
    public Vector3 centreOfFlockmates;
    [HideInInspector]
    public int numPerceivedFlockmates;

    // Cached
    public Material material;
    public Transform cachedTransform;
    public Transform target;

    StateMachine stateMachine = new StateMachine();

    void Start()
    {
        stateMachine.ChangeState(new BoidFlockState(this));
    }

    void Awake()
    {
        //material = transform.GetComponentInChildren<MeshRenderer>().material;
        cachedTransform = transform;
    }

    public void Initialize(BoidSettings settings, Transform target)
    {
        this.target = target;
        this.settings = settings;

        position = cachedTransform.position;
        forward = cachedTransform.forward;

        float startSpeed = (settings.minSpeed + settings.maxSpeed) / 2;
        velocity = transform.forward * startSpeed;
    }

    public void SetColour(Color col)
    {
        //if (material != null) {
        //    material.color = col;
        //}
    }

    public void UpdateBoid()
    {
        stateMachine.Update();

        transform.rotation = Quaternion.Euler(40, 0, 0);
    }

    public float getVelocityX()
    {
        return velocity.x;
    }
}
