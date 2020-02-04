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
    Coroutine attackCoroutine;
    SpriteOrienter spOrient;

    Vector3 gizmoPos;
    float gizmoRad;

    public float energy;

    // Start is called before the first frame update
    void Start()
    {
        boid = GetComponent<Boid>();
        if (boid != null) {
            boid.switchState(new GrazeState(boid, this));
            attackDistance = 1;
        }
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
        Vector3 trans = spOrient.spriteTransform.localScale;

        while (attackTime > 0)
        {
            attackTime -= Time.deltaTime;

            percent = Mathf.Abs((attackTime * percentMod) - 1);
            float squish = Mathf.Lerp(1.5f, 1f, percent);
            float stretch = Mathf.Lerp(.5f, 1f, percent);

            spOrient.spriteTransform.localScale = new Vector3(squish * trans.x, stretch * trans.y, squish * trans.z);

            if (!hasAttacked && (attackTime < attackTimeStart * .5f))
            {
                hasAttacked = true;
                Attack();
            }

            yield return null;
        }

        spOrient.spriteTransform.localScale = trans;
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
