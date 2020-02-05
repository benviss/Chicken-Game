using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    Boid owner;
    IBoidActor boidActor;

    public IdleState(Boid owner, IBoidActor boidActor) { this.owner = owner; this.boidActor = boidActor; }

    public void Enter()
    {
    }

    public void Execute()
    {
        //do nuthin
    }

    public void Exit()
    {
    }

}
