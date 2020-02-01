using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantManager : MonoBehaviour
{
    private static PlantManager _instance;
    public static PlantManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public bool CanSpawn
    {
        get
        {
            return Plants.Count < maxPlants;
        }
    }

    public Sprite[] Sprites;
    public List<GameObject> Plants;
    private int maxPlants = 5000;

    public void AddPlant(GameObject plant)
    {
        Plants.Add(plant);
    }

    public void RemovePlant(GameObject plant)
    {
        Plants.Remove(plant);
    }
}
