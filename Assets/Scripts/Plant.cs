using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Plant : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    int displayedFoliage;
    public int sleepTimer;
    public float growth;
    float growthRate = 10.0f;
    public float age;
    float maxAge = 120;
    float scale = 2;
    float nearDistance = 5;
    float maxNearPlants = 2;

    List<GameObject> NearPlants;

    void Start()
    {
        PlantManager.Instance.AddPlant(this.gameObject);
        displayedFoliage = Random.Range(0, PlantManager.Instance.Sprites.Length-1);
        transform.rotation = Quaternion.Euler(0, Random.Range(-20, 20), 0);
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = PlantManager.Instance.Sprites[displayedFoliage];
        NearPlants = new List<GameObject>();
        growth = .1f;
        age = 0f;
        sleepTimer = 0;

        FindNearPlants();

        UpdateScale();
    }

    void Update()
    {
        age += Time.deltaTime;
        if (age > maxAge)
        {
            Kill();
        }

        if (sleepTimer > 0)
        {
            sleepTimer--;
            return;
        }

        if (growth < 1)
        {
            growth += (Time.deltaTime / growthRate);
            UpdateScale();
            return;
        }

        if (IsCrowded())
        {
            sleepTimer = 100;
            return;
        }

        TrySpawn();
        sleepTimer += 50;
    }

    private void UpdateScale()
    {
        transform.localScale = transform.localScale.normalized * growth * scale;
        transform.position = new Vector3(transform.position.x, (float) transform.localScale.y * .5f, transform.position.z);
    }

    private bool IsCrowded()
    {
        if (NearPlants.Count > maxNearPlants)
        {
            return true;
        }

        return false;
    }

    private void FindNearPlants()
    {
        foreach (GameObject plant in PlantManager.Instance.Plants)
        {
            if (plant == this.gameObject)
            {
                continue;
            }

            float dist = Vector3.Magnitude(plant.transform.position - transform.position);

            if (dist < nearDistance)
            {
                NearPlants.Add(plant);
            }
        }
    }

    private void NotifyNearPlants()
    {
        foreach (GameObject plant in NearPlants)
        {
            plant.GetComponent<Plant>().AddNearPlant(this.gameObject);
        }
    }

    public void AddNearPlant(GameObject plant)
    {
        NearPlants.Add(plant);
    }

    private void TrySpawn()
    {
        if (PlantManager.Instance.CanSpawn)
        {
            if (Random.Range(0, 1000) > 900)
            {
                float seedNum = Random.Range(1, 3);

                for (int i = 0; i < seedNum; i++)
                {
                    float distance = Random.Range(1, 10);
                    Vector2 vector = Random.insideUnitCircle.normalized * distance;
                    Vector3 pos = new Vector3(transform.position.x + vector.x, 0, transform.position.z + vector.y);
                    CreatePlant(pos);
                }
            }
        }
    }

    private void Kill()
    {
        PlantManager.Instance.RemovePlant(this.gameObject);
        Destroy(this.gameObject);
    }

    private void CreatePlant(Vector3 pos)
    {
        GameObject newPlant = Instantiate(this.gameObject, transform.parent);
        newPlant.name = "Plant";
        newPlant.transform.position = pos;
    }
}
