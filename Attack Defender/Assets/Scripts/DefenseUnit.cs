using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DefenseUnitState
{
    Idle,
    Attacking,
    Dead
}

public class DefenseUnit : MonoBehaviour
{
    public Animator animator;

    public DefenseStructure structure;

    public float attackRange = 5f;

    [Tooltip("How many attacks per second this unit can do")]
    public float attackRate = 0.5f;
    [Tooltip("How many attack animations this model have?")]
    public int attackAnimationsCount = 2;
    public float damage = 16f;

    public Transform projectileOrigin;
    public string projectilePrefabName;

    DefenseUnitState _state;
    DefenseUnitState state
    {
        get { return _state; }
        set
        {
            _state = value;

            enabled = value != DefenseUnitState.Dead;

            OnStateChanged();
        }
    }

    float nextAttackTime = 0f;
    float[] attackAnimLength;
    AttackUnitMelee attackTarget;
    Vector3 attackOrigin;
    GameObject projectile;

    // Start is called before the first frame update
    void Start()
    {
        // Get attached structure data
        structure = GetComponent<DefenseStructure>();

        // Calculate attack point to center of the cell
        attackOrigin = new Vector3(
            transform.position.x,
            transform.position.y,
            transform.position.z
            );

        // Load projectile prefab if it's used
        projectile = GameConst.LoadPrefab(AssetPath.Projectile, projectilePrefabName);

        // Draw Range
        var cyl = Resources.Load<GameObject>("AttackRangeObj");
        var range = Instantiate(cyl);
        range.transform.parent = transform;
        range.transform.localPosition = Vector3.zero;
        range.transform.localScale = new Vector3(attackRange * 2, range.transform.localScale.y, attackRange * 2);

    }

    // Update is called once per frame
    void Update()
    {
        // Check if still alive
        if (structure.isDead)
        {
            state = DefenseUnitState.Dead;
        }

        switch (state)
        {
            case DefenseUnitState.Idle:
                {
                    // Find a target
                    var attackers = UnityEngine.Object.FindObjectsOfType<AttackUnitMelee>();
                    int targetId = -1;
                    float targetRange = -1f;

                    for (int i = 0; i < attackers.Length; i++)
                    {
                        var attacker = attackers[i];
                        var range = GameConst.getRange(attackOrigin, attacker.transform.position);

                        if (attacker == null)
                            continue;

                        // Attacker in range
                        if (range <= attackRange)
                        {
                            // Check if target is alive
                            if (attacker.isDead)
                                continue;

                            // Check if there's previous to compare
                            if (targetId > -1)
                            {
                                // Object is nearest, let's attack it
                                if (range < targetRange)
                                {
                                    targetId = i;
                                    targetRange = range;
                                }
                            }
                            else
                            {
                                targetId = i;
                                targetRange = range;
                            }
                        }
                    }

                    // After check every object, let's change to attack mode if needed
                    if (targetId > -1)
                    {
                        attackTarget = attackers[targetId];
                        state = DefenseUnitState.Attacking;

                        // Delay first attack to rotate before it
                        if (attackRate > 1f)
                        {
                            nextAttackTime = Time.time + 1f / attackRate;
                        }
                        else // Maximum delay of 1s (for slow units)
                        {
                            nextAttackTime = Time.time + 1f;
                        }

                        attackTarget.GetComponentsInChildren<Renderer>()[1].material.SetColor("_BaseColor", Color.yellow);
                    }

                    // Target not found? let's rotate to original position if needed
                    // if (transform.rotation.eulerAngles)
                }
                break;

            case DefenseUnitState.Attacking:
                {
                    // Check if target is valid
                    if (attackTarget == null)
                    {
                        state = DefenseUnitState.Idle;
                        return;
                    }

                    // Check if target is in range
                    if (!GameConst.isInRange(attackOrigin, attackTarget.transform.position, attackRange))
                    {
                        attackTarget.GetComponentsInChildren<Renderer>()[1].material.SetColor("_BaseColor", Color.green);
                        state = DefenseUnitState.Idle;
                        return;
                    }

                    // Look at target
                    LookToTarget();

                    // Attack delay
                    if (Time.time > nextAttackTime)
                    {
                        // Attack successful
                        Attack();
                        nextAttackTime = Time.time + 1f / attackRate;
                    }
                }
                break;
        }
    }

    void OnStateChanged()
    {
        switch (state)
        {
            case DefenseUnitState.Idle:
                {
                    // Clear attack target
                    attackTarget = null;
                }
                break;

            case DefenseUnitState.Attacking:
                {

                }
                break;

            case DefenseUnitState.Dead:
                {

                }
                break;
        }
    }

    void LookToTarget(bool smooth = true)
    {
        var lookPos = attackTarget.transform.position - transform.position;
        lookPos.y = 0; // Don't look upward

        var lookRot = Quaternion.LookRotation(lookPos);

        if (smooth)
        {
            // Synchronize rotation with attack delay
            var lookFactor = Mathf.Clamp(attackRate * 5f, 5f, 30f);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRot, lookFactor * Time.deltaTime);
        }
        else
        {
            transform.rotation = lookRot;
        }
    }

    void Attack()
    {
        // Does this defense use projectiles?
        if (projectileOrigin != null)
        {
            // Instantly look at target before shoot
            // LookToTarget(false);

            // Create projectile
            var clone = Instantiate(projectile, projectileOrigin.position, transform.rotation);
            var proj = clone.GetComponent<Projectile>();
            proj.target = attackTarget.transform;
        }

        // Damage target
        attackTarget.TakeDamage(damage);

        // Check for death
        if (attackTarget.isDead)
        {
            state = DefenseUnitState.Idle;
        }
    }

    void OnDrawGizmosSelected()
    {
        UnityEditor.Handles.DrawWireDisc(transform.position, transform.up, attackRange);
        // Gizmos.DrawWireSphere(transform.position + transform.localScale / 2, attackRange);
    }
}
