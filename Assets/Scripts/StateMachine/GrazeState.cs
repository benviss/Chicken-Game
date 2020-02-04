using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Boid;

public class GrazeState : IState
{
    Boid owner;
    FoodTypes food;
    Transform foodTarget;

    public GrazeState(Boid owner, FoodTypes food) { this.owner = owner; this.food = food; }

    public void Enter()
    {
    }

    public void Execute()
    {
        //find food target if none already
        if (foodTarget == null) {
            findFood();
        }

        if (Random.Range(1, 100) == 1)
        {
            findFood();
        }

        if (foodTarget != null && Vector3.Distance(owner.transform.position, foodTarget.position) < 1 ) {
            //consume food target if next to it
            owner.TryAttack();
        } else {
            Debug.Log("searching");
            owner.MoveBoid();
        }
    }

    public void Exit()
    {
    }

    public void findFood()
    {
        if (food == FoodTypes.Bird) {
            var bk = LayerMask.GetMask(BoidUtils.GetFoodTypeString(food));
        }
        var foods = Physics.OverlapSphere(owner.transform.position, owner.grazeRange, LayerMask.GetMask(BoidUtils.GetFoodTypeString(food)));
        if (foods.Length == 0) {
            owner.switchState(new BoidFlockState(owner, food));
            Debug.Log("exit state");
            return;
        }

        Collider randomFood = foods[Random.Range(0, foods.Length - 1)];
        foodTarget = randomFood.gameObject.transform;
        Vector3 leftPos = foodTarget.position - Vector3.right * .5f;
        Vector3 rightPos = foodTarget.position + Vector3.right * .5f;

        float leftDist = Vector3.Distance(leftPos, owner.position);
        float rightDist = Vector3.Distance(rightPos, owner.position);

        if (leftDist < rightDist)
        {
            owner.targetPosition = leftPos;
        }
        else
        {
            owner.targetPosition = rightPos;
        }
        owner.target = null;
    }
}
