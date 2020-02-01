using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidFlockState : IState
{
    Boid owner; 

    public BoidFlockState(Boid owner) { this.owner = owner; }

    public void Enter()
    {
    }

    public void Execute()
    {
        Debug.Log("updating test state");
        UpdateBoid();
    }

    public void Exit()
    {
        Debug.Log("exiting test state");
    }


    public void UpdateBoid()
    {
        Vector3 acceleration = Vector3.zero;

        if (owner.target != null) {
            Vector3 offsetToTarget = (owner.target.position - owner.position);
            acceleration = SteerTowards(offsetToTarget) * owner.settings.targetWeight;
        }

        if (owner.numPerceivedFlockmates != 0) {
            owner.centreOfFlockmates /= owner.numPerceivedFlockmates;

            Vector3 offsetToFlockmatesCentre = (owner.centreOfFlockmates - owner.position);

            var alignmentForce = SteerTowards(owner.avgFlockHeading) * owner.settings.alignWeight;
            var cohesionForce = SteerTowards(offsetToFlockmatesCentre) * owner.settings.cohesionWeight;
            var seperationForce = SteerTowards(owner.avgAvoidanceHeading) * owner.settings.seperateWeight;

            acceleration += alignmentForce;
            acceleration += cohesionForce;
            acceleration += seperationForce;
        }

        if (IsHeadingForCollision()) {
            Vector3 collisionAvoidDir = ObstacleRays();
            Vector3 collisionAvoidForce = SteerTowards(collisionAvoidDir) * owner.settings.avoidCollisionWeight;
            acceleration += collisionAvoidForce;
        }

        owner.velocity += acceleration * Time.deltaTime;
        float speed = owner.velocity.magnitude;
        Vector3 dir = owner.velocity / speed;
        speed = Mathf.Clamp(speed, owner.settings.minSpeed, owner.settings.maxSpeed);
        owner.velocity = dir * speed;

        owner.cachedTransform.position += owner.velocity * Time.deltaTime;
        owner.cachedTransform.forward = dir;
        owner.position = owner.cachedTransform.position;
        owner.forward = dir;
    }

    bool IsHeadingForCollision()
    {
        RaycastHit hit;
        if (Physics.SphereCast(owner.position, owner.settings.boundsRadius, owner.forward, out hit, owner.settings.collisionAvoidDst, owner.settings.obstacleMask)) {
            return true;
        } else { }
        return false;
    }

    Vector3 ObstacleRays()
    {
        Vector3[] rayDirections = BoidHelper.directions;

        for (int i = 0; i < rayDirections.Length; i++) {
            Vector3 dir = owner.cachedTransform.TransformDirection(rayDirections[i]);
            Ray ray = new Ray(owner.position, dir);
            if (!Physics.SphereCast(ray, owner.settings.boundsRadius, owner.settings.collisionAvoidDst, owner.settings.obstacleMask)) {
                return dir;
            }
        }

        return owner.forward;
    }

    Vector3 SteerTowards(Vector3 vector)
    {
        Vector3 v = vector.normalized * owner.settings.maxSpeed - owner.velocity;
        return Vector3.ClampMagnitude(v, owner.settings.maxSteerForce);
    }





}
