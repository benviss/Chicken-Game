using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burb : MonoBehaviour, IEats, IBoidActor, IDamagable
{
    Boid boid;
    float attackDistance = .7f;
    float damage = 1;
    float attackCooldown = .1f;
    float lastAttack = 0;
    bool isBusy;
    float energyMultiplier;
    Coroutine pootEggCoroutine;
    Coroutine attackCoroutine;
    SpriteOrienter spOrient;
    Vector3 gizmoPos;
    float gizmoRad;

    public float energy;

    // Start is called before the first frame update
    void Start()
    {
        boid = GetComponent<Boid>();
        energyMultiplier = 1;

        if (boid != null) {
            boid.switchState(new GrazeState(boid, this));
            attackDistance = 1;
            energyMultiplier = .5f;
            StartCoroutine(GrowUpAnimation());
        }

        spOrient = GetComponent<SpriteOrienter>();
        energy = 30;
    }

    void FixedUpdate()
    {
        energy -= Time.fixedDeltaTime * energyMultiplier;

        if ((energy > 300) && (!isBusy))
        {
            pootEggCoroutine = StartCoroutine(PootEggAnimation());
        }

        if (energy < 100)
        {
            if (boid != null)
            {
                if ((boid.State == "BoidFlockState") && (Random.Range(1, 100) == 1))
                {
                    boid.switchState(new GrazeState(boid, this));
                }
            }
        }

        if (energy < 0)
        {
            if (boid != null)
            {
                if (!isBusy)
                {
                    boid.switchState(new IdleState(boid, this));
                    StartCoroutine(DieAnimation());
                }
            }
            else
            {
                //gameover
                energy += 5;
            }
        }
    }

    private IEnumerator DieAnimation()
    {
        isBusy = true;
        float totalTime = 30f;
        float t = 0;
        float percent = 0;
        Vector3 trans = transform.localScale;
        Color deathColor = new Color(0f, 0f, 0f, 0f);
        Color defaultColor = spOrient.sprite.color;
        while (percent < 1)
        {
            percent = t / totalTime;
            float fallover = Mathf.Lerp(0, 90, percent*10);
            spOrient.spriteTransform.rotation = Quaternion.Euler(40, 0, fallover);
            float scale = Mathf.Lerp(1.2f, 5, percent);
            scale = Mathf.Clamp(scale, 0, 1);
            transform.localScale = new Vector3(scale * trans.x, scale * trans.y, scale * trans.z);
            spOrient.sprite.color = Color.Lerp(defaultColor, deathColor, percent);
            t += Time.deltaTime;
            yield return null;
        }

        Kill();
    }

    private IEnumerator PootEggAnimation()
    {
        isBusy = true;
        bool hasPooted = false;
        float totalTime = .5f;
        float t = totalTime;
        float percentMod = 2 / t;
        float percent = 1;
        Vector3 trans = spOrient.spriteTransform.localScale;

        while (t > 0)
        {
            t -= Time.deltaTime;

            percent = Mathf.Abs((t * percentMod) - 1);
            float squish = Mathf.Lerp(.5f, 1f, percent);
            float stretch = Mathf.Lerp(1.5f, 1f, percent);

            spOrient.spriteTransform.localScale = new Vector3(squish * trans.x, stretch * trans.y, squish * trans.z);

            if (!hasPooted && (t < totalTime * .5f))
            {
                hasPooted = true;
                PootEgg();
            }

            yield return null;
        }

        spOrient.spriteTransform.localScale = trans;
        energy -= 250;
        isBusy = false;
        lastAttack = Time.time;
    }

    private void PootEgg()
    {
        GameObject newBurb = Instantiate(GameManager.Instance.eggPrefab, transform.parent);
        newBurb.name = "Egg";
        newBurb.transform.position = transform.position;
    }

    public void StartEgg()
    {
        Rigidbody body = GetComponent<Rigidbody>();

        transform.position += body.velocity.normalized;
        body.velocity = Vector3.zero;

        StartCoroutine(GrowUpAnimation());
    }

    private IEnumerator GrowUpAnimation()
    {
        isBusy = false;
        float totalTime = 30f;
        float t = 0;
        float percent = 0;
        Vector3 trans = transform.localScale;

        while (percent <1)
        {
            percent = t / totalTime;
            float scale = Mathf.Lerp(.3f, 1, percent);
            transform.localScale = new Vector3(scale * trans.x, scale * trans.y, scale * trans.z);
            t += Time.deltaTime;
            yield return null;
        }

        energyMultiplier = 1;
        transform.localScale = trans;
        isBusy = false;

    }

    public void TryAttack()
    {
        if (!isBusy && (lastAttack + attackCooldown < Time.time)) {
            attackCoroutine = StartCoroutine(AttackAnimation());
        }
    }

    private IEnumerator AttackAnimation()
    {
        isBusy = true;
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
        isBusy = false;
        lastAttack = Time.time;
    }

    private void Attack()
    {
        // should be able to hit whatever - for now plants
        int layerMask = LayerMask.GetMask(GetFoodType());
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

    private void Kill()
    {
        Destroy(this.gameObject);
    }

    public string GetFoodType()
    {
        return "Plant";
    }

    public void TakeHit(float damage, IEats hitter)
    {
        Debug.Log($"Bird hit energy now {energy}");
        energy -= damage;
    }

    public bool IsDead()
    {
        return energy <= 0;
    }
}
