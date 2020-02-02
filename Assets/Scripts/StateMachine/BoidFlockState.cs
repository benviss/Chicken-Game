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
        owner.MoveBoid();
    }

    public void Exit()
    {
        Debug.Log("exiting test state");
    }


}
