using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MeleeUnitState
{
    Idle,
    Walking,
    Attacking,
    Dead
}

public class AttackUnitMelee : MonoBehaviour
{
    public StageBuilder stageBuilder;

    public Animator animator;
    public float movementSpeed = 0.5f;

    [Tooltip("Unit position distance from the target")]
    public float unitPositionOffset = 0.5f;

    [Tooltip("How many attacks per second this unit can do")]
    public float attackRate = 0.5f;
    [Tooltip("How many attack animations this model have?")]
    public int attackAnimationsCount = 2;
    public float damage = 12f;
    public float HP = 100f;

    MeleeUnitState _state;
    MeleeUnitState state
    {
        get { return _state; }
        set
        {
            _state = value;
            OnStateChanged();
        }
    }

    int currentCell = -1;
    float nextAttackTime = 0f;
    float[] attackAnimLength;
    DefenseStructure attackTarget;
    float currentHealth;
    public bool isDead
    {
        get { return state == MeleeUnitState.Dead; }
        set
        {
            if (value == true)
            {
                state = MeleeUnitState.Dead;
            }
            else
            {
                state = MeleeUnitState.Idle;
            }

            enabled = !value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set HP
        currentHealth = HP;

        // Cache attack animations length
        attackAnimLength = new float[attackAnimationsCount];
        for (int i = 0; i < attackAnimationsCount; i++)
        {
            var animController = animator.runtimeAnimatorController;
            var clip = animController.animationClips.First(a => a.name == ("Attack" + (i + 1)));
            attackAnimLength[i] = clip.length;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case MeleeUnitState.Idle:
                {
                    // Raid started? let's walk!
                    if (stageBuilder.gameStarted)
                    {
                        state = MeleeUnitState.Walking;
                    }
                }
                break;

            case MeleeUnitState.Walking:
                {
                    int maxCells = GameConst.mapGridSizeX;
                    float cellSize = GameConst.mapCellSize.x;
                    Vector3 pos = transform.position;

                    // Get current cell first
                    if (currentCell < 1)
                    {
                        currentCell = (int)Math.Ceiling(pos.x / cellSize);
                    }

                    // Am I in the end of current map cell?
                    if ((pos.x - unitPositionOffset) > (currentCell - 1) * cellSize)
                    {
                        // let's move!
                        WalkForward();
                    }
                    else
                    {
                        // Reached cell border! Check for structure on next cell
                        var nextCell = stageBuilder.GetMapCell(currentCell - 1, (int)Math.Ceiling(pos.z / GameConst.mapCellSize.z));

                        if (nextCell.hasStructure)
                        {
                            attackTarget = nextCell.GetComponentInChildren<DefenseStructure>();
                            state = MeleeUnitState.Attacking;
                        }
                        else
                        {
                            // Keep moving to next cell
                            currentCell -= 1;
                        }
                    }

                }
                break;

            case MeleeUnitState.Attacking:
                {
                    if (Time.time > nextAttackTime)
                    {
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
            case MeleeUnitState.Idle:
            case MeleeUnitState.Attacking:
                {
                    animator.SetBool("isWalking", false);
                    animator.SetBool("isRunning", false);
                    animator.SetBool("isDead", false);
                }
                break;

            case MeleeUnitState.Walking:
                {
                    attackTarget = null;

                    // Animate according to move speed
                    float walkAnimSpeed = movementSpeed / 0.6f;

                    // Run instead of walk fast
                    if (movementSpeed > 2f)
                    {
                        animator.SetBool("isRunning", true);
                        animator.SetFloat("RunSpeed", (1f + (movementSpeed - 2f) / 1f));
                    }
                    else
                    {
                        animator.SetBool("isWalking", true);
                        animator.SetFloat("MoveSpeed", walkAnimSpeed);
                    }
                }
                break;

            case MeleeUnitState.Dead:
                {
                    animator.SetBool("isDead", true);
                }
                break;
        }
    }

    void WalkForward()
    {
        transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
    }

    void Attack()
    {
        // Check if target is valid
        if (attackTarget == null)
            return;

        // Do the damage
        attackTarget.TakeDamage(damage);

        // Check for death
        if (attackTarget.isDead)
        {
            state = MeleeUnitState.Walking;
        }

        // Animation Stuff
        // Select a random attack animation
        int rnd = UnityEngine.Random.Range(1, attackAnimationsCount + 1);

        // Speed the animation if needed
        if (1f / attackRate < attackAnimLength[rnd - 1])
        {
            animator.SetFloat("AttackSpeed", attackAnimLength[rnd - 1] / (1f / attackRate));
        }

        // Trigger the animation
        animator.SetTrigger("Attack" + rnd);
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;

        // check for death
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;


    }
}
