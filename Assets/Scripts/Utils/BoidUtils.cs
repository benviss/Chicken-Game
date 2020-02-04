using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Boid;

public class BoidUtils
{
    public static string GetFoodTypeString(FoodTypes food)
    {
        switch (food) {
            case FoodTypes.Plant:
                return "Plant";
            case FoodTypes.Bird:
                return "Bird";
            default:
                return "Woooops";
        }
    }
}