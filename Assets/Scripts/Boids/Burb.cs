using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burb : Boid, IBoidActor
{
    FoodTypes MyFoodType = FoodTypes.Plant;

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
