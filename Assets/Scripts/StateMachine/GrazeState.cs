﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrazeState : IState
{
    Boid owner;
    IBoidActor boidActor;
    Transform foodTarget;
    IBoidActor targetFoodActor;

    public GrazeState(Boid owner, IBoidActor boidActor) { this.owner = owner; this.boidActor = boidActor; }

    public void Enter()
    {
    }

    public void Execute()
    {

        if (foodTarget != null) {
            Debug.DrawLine(owner.transform.position, foodTarget.position, Color.red);
            
        } else {
            if (boidActor.GetFoodType() == "Bird") {
                Debug.Log("Coyote has no food");
            }
        }


        //find food target if none already
        if ((foodTarget == null) && (owner.targetPosition == null)) {
            findFood();
        }

        if (Random.Range(1, 100) == 1) {
            findFood();
        }

        Vector3 targetPos;
        if (foodTarget != null)
        {
            targetPos = foodTarget.position;
        }
        else
        {
            targetPos = owner.targetPosition;
        }


        if (Vector3.Distance(owner.transform.position, targetPos) < 1 ) {
            //consume food target if next to it
            boidActor.TryAttack();
        } else {
            Debug.Log($"searching for {boidActor.GetFoodType()}");
            owner.MoveBoid();
        }
    }

    public void Exit()
    {
    }

    public void findFood()
    {
        var foods = Physics.OverlapSphere(owner.transform.position, owner.grazeRange, LayerMask.GetMask(boidActor.GetFoodType()));
        if (foods.Length == 0) {
            owner.switchState(new BoidFlockState(owner, boidActor));
            Debug.Log("exit state");
            return;
        }
        List<GameObject> aliveFoods = new List<GameObject>();
        foreach (var item in foods) {
            //this is bad fix it later
            var foodActor = foodTarget.GetComponent<IBoidActor>();
            if (foodActor != null && !foodActor.IsDead()) {
                var aliveFood = item.gameObject;

                aliveFoods.Add(aliveFood);
            }
        }

        GameObject randomFood = aliveFoods[Random.Range(0, aliveFoods.Count - 1)];
        targetFoodActor = foodTarget.GetComponent<IBoidActor>();//very bad to do it twice 

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
