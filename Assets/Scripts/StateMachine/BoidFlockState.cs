using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidFlockState : IState
{
    Boid owner; 
    Burb burb;

    float lastFoodCheck;
    float foodCheckCooldown = 5;

    public BoidFlockState(Boid owner, Burb burb) { this.owner = owner; this.burb = burb; }

    public void Enter()
    {
        owner.target = owner.flockTarget;
        Debug.Log("enter flock state");
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
            var foods = Physics.OverlapSphere(owner.transform.position, owner.grazeRange, LayerMask.GetMask("Plant"));
            if (foods.Length > 0) {
                owner.switchState(new GrazeState(owner, burb));
            }
        }
        lastFoodCheck = Time.time;
    }
}
