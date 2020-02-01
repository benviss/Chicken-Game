using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testscript : MonoBehaviour
{
    float growth = 0;
    float growthRate = .5f;
    float spawnTimer = .5f;
    float age = 0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(Grow());
    }



    private IEnumerator Grow()
    {
        while (growth < 1) {
            growth += (Time.deltaTime / growthRate);
            //UpdateScale();
            yield return null;
        }

        while (true) {
            if (!IsCrowded()) {
                TrySpawn();
            } else {
                age += spawnTimer;
            }
            yield return new WaitForSeconds(spawnTimer);
        }
    }


    public bool IsCrowded()
    {
        return false;
    }

    public void TrySpawn()
    {
        Debug.Log("hmm");
    }


}
