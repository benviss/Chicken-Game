using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(PlayerController))]
public class Player : MonoBehaviour
{
    public float moveSpeed = 5;
    PlayerController controller;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;

        controller.Move(moveVelocity);
    }
}
