using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidFlockState : IState
{
    Boid owner; 
    IBoidActor boidActor;

    float lastFoodCheck;
    float foodCheckCooldown = 5;

    public BoidFlockState(Boid owner, IBoidActor boidActor) { this.owner = owner; this.boidActor = boidActor; }

    public void Enter()
    {

        owner.target = owner.flockTarget;
        Debug.Log("enter flock state");
        if (boidActor.GetFoodType() == "Bird") {
            Debug.Log("coyote doin stupid shit");

            owner.switchState(new GrazeState(owner, boidActor));
        }
    }

    public void Execute()
    {
        if ((lastFoodCheck + foodCheckCooldown < Time.time)) {
            CheckForFood();
        }

        owner.MoveBoid();
    }

    public void Exit()
    {
    }

    public void CheckForFood()
    {
        if (Vector3.Distance(owner.position, owner.target.position) < owner.followRange) {
            var foods = Physics.OverlapSphere(owner.transform.position, owner.grazeRange, LayerMask.GetMask(boidActor.GetFoodType()));
            if (foods.Length > 0) {
                owner.switchState(new GrazeState(owner, boidActor));
            }
        }
        lastFoodCheck = Time.time;
    }
}
