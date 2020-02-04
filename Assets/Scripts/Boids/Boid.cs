//COURTESY OF SEBASTIAN LAGUE A HERO AMONG MEN
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour, IActor, IEats
{
    [HideInInspector]
    public BoidSettings settings;

    // State
    [HideInInspector]
    public Vector3 position;
    [HideInInspector]
    public Vector3 forward;
    [HideInInspector]
    public Vector3 velocity;

    // To update:
    [HideInInspector]
    public Vector3 avgFlockHeading;
    [HideInInspector]
    public Vector3 avgAvoidanceHeading;
    [HideInInspector]
    public Vector3 centreOfFlockmates;
    [HideInInspector]
    public int numPerceivedFlockmates;

    // Cached
    public Material material;
    public Transform cachedTransform;
    public Transform target;
    public Vector3 targetPosition;

    public Transform flockTarget;

    public float followRange;
    public float grazeRange;

    public float energy;

    float attackDistance = .7f;
    float damage = 1;
    float attackCooldown = .1f;
    float lastAttack = 0;
    bool attacking;

    Coroutine attackCoroutine;
    public SpriteOrienter spOrient;

    Vector3 gizmoPos;
    float gizmoRad;

    public enum FoodTypes
    {
        Plant,
        Bird
    }
    
    StateMachine stateMachine = new StateMachine();

    void Start()
    {
        //stateMachine.ChangeState(new BoidFlockState(this));
    }

    void Awake()
    {
        spOrient = GetComponent<SpriteOrienter>();
        //material = transform.GetComponentInChildren<MeshRenderer>().material;
        cachedTransform = transform;
    }

    public void Initialize(BoidSettings settings, Transform target)
    {
        this.target = target;
        this.flockTarget = target;
        this.settings = settings;

        position = cachedTransform.position;
        forward = cachedTransform.forward;

        float startSpeed = (settings.minSpeed + settings.maxSpeed) / 2;
        velocity = transform.forward * startSpeed;
    }

    public void SetColour(Color col)
    {
        //if (material != null) {
        //    material.color = col;
        //}
    }

    public void UpdateBoid()
    {
        stateMachine.Update();

        transform.rotation = Quaternion.Euler(40, 0, 0);
    }

    public float getVelocityX()
    {
        return velocity.x;
    }

    public void switchState(IState state)
    {
        stateMachine.ChangeState(state);
    }

    public void MoveBoid()
    {
        Vector3 acceleration = Vector3.zero;

        if (target != null)
        {
            Vector3 offsetToTarget = (target.position - position);
            acceleration = SteerTowards(offsetToTarget) * settings.targetWeight;
        } else {
            Vector3 offsetToTarget = (targetPosition - position);
            acceleration = SteerTowards(offsetToTarget) * settings.targetWeight;

        }

        if (numPerceivedFlockmates != 0) {
            centreOfFlockmates /= numPerceivedFlockmates;

            Vector3 offsetToFlockmatesCentre = (centreOfFlockmates - position);

            var alignmentForce = SteerTowards(avgFlockHeading) * settings.alignWeight;
            var cohesionForce = SteerTowards(offsetToFlockmatesCentre) * settings.cohesionWeight;
            var seperationForce = SteerTowards(avgAvoidanceHeading) * settings.seperateWeight;

            acceleration += alignmentForce;
            acceleration += cohesionForce;
            acceleration += seperationForce;
        }

        if (IsHeadingForCollision()) {
            Vector3 collisionAvoidDir = ObstacleRays();
            Vector3 collisionAvoidForce = SteerTowards(collisionAvoidDir) * settings.avoidCollisionWeight;
            acceleration += collisionAvoidForce;
        }
        velocity *= .9f;
        velocity += acceleration * Time.deltaTime;
        velocity.y = 0;
        float speed = velocity.magnitude;
        Vector3 dir = velocity / speed;
        speed = Mathf.Clamp(speed, settings.minSpeed, settings.maxSpeed);
        velocity = dir * speed;

        cachedTransform.position += velocity * Time.deltaTime;
        cachedTransform.forward = dir;
        position = cachedTransform.position;
        position.y = 0;
        forward = dir;
    }

    bool IsHeadingForCollision()
    {
        RaycastHit hit;
        if (Physics.SphereCast(position, settings.boundsRadius, forward, out hit, settings.collisionAvoidDst, settings.obstacleMask)) {
            return true;
        } else { }
        return false;
    }

    Vector3 ObstacleRays()
    {
        Vector3[] rayDirections = BoidHelper.directions;

        for (int i = 0; i < rayDirections.Length; i++) {
            Vector3 dir = cachedTransform.TransformDirection(rayDirections[i]);
            Ray ray = new Ray(position, dir);
            if (!Physics.SphereCast(ray, settings.boundsRadius, settings.collisionAvoidDst, settings.obstacleMask)) {
                return dir;
            }
        }

        return forward;
    }

    Vector3 SteerTowards(Vector3 vector)
    {
        Vector3 v = vector.normalized * settings.maxSpeed - velocity;
        return Vector3.ClampMagnitude(v, settings.maxSteerForce);
    }

    public void GetFood(float foodAmount)
    {
        energy += foodAmount;
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

        while (attackTime > 0) {
            attackTime -= Time.deltaTime;

            percent = Mathf.Abs((attackTime * percentMod) - 1);
            float squish = Mathf.Lerp(1.5f, 1f, percent);
            float stretch = Mathf.Lerp(.5f, 1f, percent);

            spOrient.spriteTransform.localScale = new Vector3(squish * trans.x, stretch * trans.y, squish * trans.z);

            if (!hasAttacked && (attackTime < attackTimeStart * .5f)) {
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(gizmoPos, gizmoRad);
    }
}
