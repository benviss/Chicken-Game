using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBoidActor
{
    string GetFoodType();
    void TryAttack();
}
