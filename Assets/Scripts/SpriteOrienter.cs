using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteOrienter : MonoBehaviour
{
    public Sprite leftSprite;
    public Sprite rightSprite;
    SpriteRenderer sprite;
    bool isFacingRight = true;

    PlayerController controller;
    public IActor owner;

    // Start is called before the first frame update
    void Start()
    {
        leftSprite = GameManager.Instance.leftChickenSprite;
        rightSprite = GameManager.Instance.rightChickenSprite;
        sprite = GetComponentInChildren<SpriteRenderer>();
        sprite.sprite = rightSprite;
        owner = GetComponent<IActor>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (owner.getVelocityX() != 0) {
            if ((owner.getVelocityX() > 0) && !isFacingRight) {
                isFacingRight = true;
                sprite.sprite = rightSprite;
            } else if ((owner.getVelocityX() < 0) && isFacingRight) {
                isFacingRight = false;
                sprite.sprite = leftSprite;
            }
        }
    }

    public bool getFacingRight()
    {
        return isFacingRight;
    }
}
