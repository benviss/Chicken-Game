using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrazeState : IState
{
    Boid owner;
    Burb burb;
    Transform foodTarget;

    public GrazeState(Boid owner, Burb burb) { this.owner = owner; this.burb = burb; }

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

        if (Vector3.Distance(owner.transform.position, foodTarget.position) < 1 ) {
            //consume food target if next to it
            burb.TryAttack();
        } else {
            owner.MoveBoid();
        }
    }

    public void Exit()
    {
        owner.switchState(new BoidFlockState(owner));
    }

    public void findFood()
    {
        int layerMask = 1 << 9; //Layer 9
        var foods = Physics.OverlapSphere(owner.transform.position, 10, layerMask);
        if (foods.Length == 0) {
            Exit();
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
