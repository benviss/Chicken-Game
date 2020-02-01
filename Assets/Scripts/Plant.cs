﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Plant : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    int displayedFoliage;
    public float growth;
    float growthRate = 3.00f;
    public float age;
    public float maxAge;
    float scale = 2;
    float nearDistance = 3;
    float maxNearPlants = 3;
    float spawnTimer = 20;

    int NearPlants;

    void Start()
    {
        NearPlants = 0;
        FindNearPlants();
        if ((NearPlants > maxNearPlants * 2) || !IsOnGround())
        {
            Kill();
        }

        displayedFoliage = Random.Range(0, PlantManager.Instance.Sprites.Length - 1);
        transform.rotation = Quaternion.Euler(0, Random.Range(-30, 30), 0);
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = PlantManager.Instance.Sprites[displayedFoliage];
        growth = .1f;
        age = 0f;
        maxAge = Random.Range(100, 300);
        StartCoroutine(Grow());
        StartCoroutine(Lifetime());
    }


    private IEnumerator Grow()
    {
        while (growth < 1)
        {
            growth += (Time.deltaTime / growthRate);
            UpdateScale();
            yield return null;
        }
        StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        float wait = Random.Range(spawnTimer * .3f, spawnTimer);
        yield return new WaitForSeconds(wait);

        while (true)
        {
            if (!IsCrowded())
            {
                TrySpawn();
            }
            else
            {
                age += spawnTimer;
            }
            yield return new WaitForSeconds(spawnTimer);
        }
    }


    private IEnumerator Lifetime()
    {
        while (age < maxAge)
        {
            age++;
            yield return new WaitForSeconds(1);
        }

        StartCoroutine(Die());
    }

    private IEnumerator Die()
    {
        float dieTime = 1f;
        float dieSpeed = 1.5f;
        Color deathColor = new Color(.9f, .85f, .85f);
        Color defaultColor = spriteRenderer.color;
        while (dieTime > 0)
        {
            dieTime -= Time.deltaTime * dieSpeed;
            growth = dieTime;
            growth = Mathf.Clamp(growth, .01f, 1);
            UpdateScale();
            spriteRenderer.color = Color.Lerp(defaultColor, deathColor, dieTime);
            yield return null;
        }

        Kill();
    }

    private void UpdateScale()
    {
        transform.localScale = transform.localScale.normalized * growth * scale;
        transform.position = new Vector3(transform.position.x, (float) transform.localScale.y * .5f, transform.position.z);
    }

    private bool IsCrowded()
    {
        FindNearPlants();
        if (NearPlants > maxNearPlants)
        {
            return true;
        }

        return false;
    }

    private void FindNearPlants()
    {
        int layerMask = LayerMask.GetMask("Plant");
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, nearDistance, layerMask);
        NearPlants = hitColliders.Length;
    }

    private bool IsOnGround()
    {
        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = LayerMask.GetMask("Ground");

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position + Vector3.up, transform.TransformDirection(Vector3.down), out hit, 10, layerMask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void TrySpawn()
    {
        if (PlantManager.Instance.CanSpawn)
        {
            float seedNum = Random.Range(1, 3);

            for (int i = 0; i < seedNum; i++)
            {
                float distance = Random.Range(1, 10);
                Vector2 vector = Random.insideUnitCircle.normalized * distance;
                Vector3 pos = new Vector3(transform.position.x + vector.x, 0, transform.position.z + vector.y);
                PlantManager.Instance.CreatePlant(pos);
            }
        }
    }

    private void Kill()
    {
        Destroy(this.gameObject);
    }
}
