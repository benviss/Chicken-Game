using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(PlayerController))]
public class Player : MonoBehaviour, IEats
{
    public float moveSpeed = 5;
    PlayerController controller;
    float attackDistance = .7f;
    float damage = 1;
    float attackCooldown = .1f;
    float lastAttack = 0;
    bool attacking;
    public float energy;
    bool isFacingRight = true;
    Coroutine attackCoroutine;
    public Sprite leftSprite;
    public Sprite rightSprite;
    SpriteRenderer sprite;

    Vector3 gizmoPos;
    float gizmoRad;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<PlayerController>();
        leftSprite = GameManager.Instance.leftChickenSprite;
        rightSprite = GameManager.Instance.rightChickenSprite;
        sprite = GetComponentInChildren<SpriteRenderer>();
        sprite.sprite = rightSprite;
        energy = 30;
        transform.rotation = Quaternion.Euler(40, 0, 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;

        if (Input.GetAxisRaw("Fire1") > 0)
        {
            if (!attacking && (lastAttack + attackCooldown < Time.time))
            {
                attackCoroutine = StartCoroutine(AttackAnimation());
            }
        }
        controller.Move(moveVelocity);

        energy -= Time.fixedDeltaTime;

        if (moveVelocity.x != 0)
        {
            if ((moveVelocity.x > 0) && !isFacingRight)
            {
                isFacingRight = true;
                sprite.sprite = rightSprite;
            }
            else if ((moveVelocity.x < 0) && isFacingRight)
            {
                isFacingRight = false;
                sprite.sprite = leftSprite;
            }
        }
    }

    private IEnumerator AttackAnimation()
    {
        attacking = true;
        bool hasAttacked = false;
        float attackTimeStart = .2f;
        float attackTime = attackTimeStart;
        float percentMod = 2 / attackTime;
        float percent = 1;

        while (attackTime > 0)
        {
            attackTime -= Time.deltaTime;

            percent = Mathf.Abs((attackTime * percentMod) - 1);
            float squish = Mathf.Lerp(1.5f, 1f, percent);
            float stretch = Mathf.Lerp(.5f, 1f, percent);

            transform.localScale = new Vector3(squish, stretch, squish);

            if (!hasAttacked && (attackTime < attackTimeStart * .5f))
            {
                hasAttacked = true;
                Attack();
            }

            yield return null;
        }

        transform.localScale = new Vector3(1,1,1);
        attacking = false;
        lastAttack = Time.time;
    }

    private void Attack()
    {
        // should be able to hit whatever - for now plants
        int layerMask = LayerMask.GetMask("Plant");
        Vector3 attackPos = (transform.position);
        
        if (isFacingRight)
        {
            attackPos.x += attackDistance;
        }
        else
        {
            attackPos.x -= attackDistance;
        }

        attackPos.y = attackDistance * .5f;
        Collider[] hitColliders = Physics.OverlapSphere(attackPos, attackDistance, layerMask);
        gizmoPos = attackPos;
        gizmoRad = attackDistance;

        foreach (Collider c in hitColliders)
        {
            IDamagable damagableObject = c.GetComponent<IDamagable>();

            if (damagableObject != null)
            {
                damagableObject.TakeHit(damage, this.GetComponent<IEats>());
            }
        }
    }

    public void GetFood(float foodAmount)
    {
        energy += foodAmount;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(gizmoPos, gizmoRad);
    }
}
