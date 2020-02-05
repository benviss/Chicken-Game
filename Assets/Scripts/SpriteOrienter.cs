using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteOrienter : MonoBehaviour
{
    public Sprite leftSprite;
    public Sprite rightSprite;
    SpriteRenderer sprite;

    bool isFacingRight = true;
    internal Transform spriteTransform;
    public IActor owner;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        spriteTransform = sprite.gameObject.transform;
        sprite.sprite = rightSprite;
        owner = GetComponent<IActor>();
        spriteTransform.rotation = Quaternion.Euler(40, 0, 0);
    }

    // Update is called once per frame
    void Update()
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
