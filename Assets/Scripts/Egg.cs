using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(Incubate());
    }


    private IEnumerator Incubate()
    {
        float wait = 10f;

        yield return new WaitForSeconds(wait);
        
        GameObject burd = Instantiate(GameManager.Instance.burbPrefab, transform.parent);
        burd.name = "Burd";
        burd.transform.position = transform.position;

        Burb b = burd.GetComponent<Burb>();

        yield return null;
        Destroy(this.gameObject);
    }
}
