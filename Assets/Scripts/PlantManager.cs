using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantManager : MonoBehaviour
{
    private static PlantManager _instance;
    public static PlantManager Instance { get { return _instance; } }
    public GameObject plantPrefab;
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
            return _canSpawn;
        }
    }

    public Sprite[] Sprites;
    public List<Plant> Plants;
    public int maxPlants = 1000;
    private bool _canSpawn = true;


    public void Start()
    {
        float seedNum = 500;

        for (int i = 0; i < seedNum; i++)
        {
            float distance = Random.Range(1, 80);
            Vector2 vector = Random.insideUnitCircle.normalized * distance;
            Vector3 pos = new Vector3(transform.position.x + vector.x, 0, transform.position.z + vector.y);
            GameObject newPlant = CreatePlant(pos);

            Plant p = newPlant.GetComponent<Plant>();
            p.growth = .95f;
        }
    }

    public GameObject CreatePlant(Vector3 pos)
    {
        GameObject newPlant = Instantiate(plantPrefab, transform);
        newPlant.name = "Plant";
        newPlant.transform.position = pos;

        return newPlant;
    }
}