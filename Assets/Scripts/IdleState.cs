using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    Boid owner;
    Burb burb;

    public IdleState(Boid owner, Burb burb) { this.owner = owner; this.burb = burb; }

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
