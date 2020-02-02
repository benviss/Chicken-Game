using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(PlayerController))]
public class Player : MonoBehaviour, IActor
{
    public float moveSpeed = 5;
    PlayerController controller;

    Burb burb;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<PlayerController>();
        transform.rotation = Quaternion.Euler(40, 0, 0);
        burb = GetComponent<Burb>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;

        if (Input.GetAxisRaw("Fire1") > 0)
        {
            burb.TryAttack();
        }
        controller.Move(moveVelocity);

    }

    public float getVelocityX()
    {
        return controller.velocity.x;
    }
   
}
