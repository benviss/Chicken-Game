using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Boid;

public class Coyote : Boid, IBoidActor
{
    FoodTypes MyFoodType = FoodTypes.Bird;

    // Start is called before the first frame update
    void Start()
    {
        this.switchState(new GrazeState(this, MyFoodType));
        energy = 30;
    }

    void FixedUpdate()
    {
        energy -= Time.fixedDeltaTime;
    }

    void startGrazing()
    {
    }

    public string GetFoodType()
    {
        return MyFoodType.ToString();
    }
}
