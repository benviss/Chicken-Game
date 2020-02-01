using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CameraController : MonoBehaviour
{
    Rigidbody playerBody;
    Vector3 targetPos;
    Vector3 offset = new Vector3(0, 17, -15);
    float maxForce = 1;
    float maxSpeed = 10;
    float minSpeed = .01f;
    // Start is called before the first frame update
    void Start()
    {
        playerBody = GameManager.Instance.player.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = playerBody.position + offset;
    }
}
