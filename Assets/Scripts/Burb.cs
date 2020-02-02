using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burb : MonoBehaviour, IEats
{
    Boid boid;
    float attackDistance = .7f;
    float damage = 1;
    float attackCooldown = .1f;
    float lastAttack = 0;
    bool attacking;
    public float grazeRange;
    Coroutine attackCoroutine;
    SpriteOrienter spOrient;

    Vector3 gizmoPos;
    float gizmoRad;

    public float energy;

    // Start is called before the first frame update
    void Start()
    {
        boid = GetComponent<Boid>();
        boid.switchState(new GrazeState(boid, this));
        spOrient = GetComponent<SpriteOrienter>();
        energy = 30;

    }

    void FixedUpdate()
    {
        energy -= Time.fixedDeltaTime;
    }

    public void TryAttack()
    {
        if (!attacking && (lastAttack + attackCooldown < Time.time)) {
            attackCoroutine = StartCoroutine(AttackAnimation());
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

        while (attackTime > 0) {
            attackTime -= Time.deltaTime;

            percent = Mathf.Abs((attackTime * percentMod) - 1);
            float squish = Mathf.Lerp(1.5f, 1f, percent);
            float stretch = Mathf.Lerp(.5f, 1f, percent);

            transform.localScale = new Vector3(squish, stretch, squish);

            if (!hasAttacked && (attackTime < attackTimeStart * .5f)) {
                hasAttacked = true;
                Attack();
            }

            yield return null;
        }

        transform.localScale = new Vector3(1, 1, 1);
        attacking = false;
        lastAttack = Time.time;
    }

    private void Attack()
    {
        // should be able to hit whatever - for now plants
        int layerMask = LayerMask.GetMask("Plant");
        Vector3 attackPos = (transform.position);
        if (spOrient.getFacingRight()) {
            attackPos.x += attackDistance;
        } else {
            attackPos.x -= attackDistance;
        }

        attackPos.y = attackDistance * .5f;
        Collider[] hitColliders = Physics.OverlapSphere(attackPos, attackDistance, layerMask);
        gizmoPos = attackPos;
        gizmoRad = attackDistance;

        foreach (Collider c in hitColliders) {
            IDamagable damagableObject = c.GetComponent<IDamagable>();

            if (damagableObject != null) {
                damagableObject.TakeHit(damage, this.GetComponent<IEats>());
            }
        }
    }

    void startGrazing()
    {
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(gizmoPos, gizmoRad);
    }


    public void GetFood(float foodAmount)
    {
        energy += foodAmount;
    }
}
