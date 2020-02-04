using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Boid;

public class BoidFlockState : IState
{
    Boid owner;
    FoodTypes food;

    float lastFoodCheck;
    float foodCheckCooldown = 5;

    public BoidFlockState(Boid owner, FoodTypes food) { this.owner = owner; this.food = food; }

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
            var foods = Physics.OverlapSphere(owner.transform.position, owner.grazeRange, LayerMask.GetMask(BoidUtils.GetFoodTypeString(food)));
            if (foods.Length > 0) {
                owner.switchState(new GrazeState(owner, food));
            }
        }
        lastFoodCheck = Time.time;
    }
}
