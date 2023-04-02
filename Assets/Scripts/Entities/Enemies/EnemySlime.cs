using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySlime : MonoBehaviour, IDamageTaker
{
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private LayerMask attackTargets;
    [SerializeField] private Health health;
    [SerializeField] private float playerAggroDistance, playerAttackDistance;

    [SerializeField] private State patrolState;
    [SerializeField] private State chaseState;
    [SerializeField] private State attackState;

    private Rigidbody2D rigidBody;
    private Animator animator;

    private float currentSpeed;

    public enum Speed
    {
        Walk,
        Run,
        Stop
    }

    public float PlayerAttackDistance => playerAttackDistance;

    private Vector2 lastMoveDirection = Vector2.zero;

    public Vector2 LastMoveDirection => lastMoveDirection;

    public LayerMask AttackTargets => attackTargets;

    State currentState;

    // Start is called before the first frame update
    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        SetState(patrolState);
    }

    private void OnEnable()
    {
        health.OnHealthEmpty += Health_OnHealthEmpty;
    }

    private void Health_OnHealthEmpty(object sender, System.EventArgs e)
    {
        gameObject.SetActive(false);
    }
    

    private void OnDisable()
    {
        health.OnHealthEmpty -= Health_OnHealthEmpty;
    }

    // Update is called once per frame
    private void Update()
    {
        if (IsPlayerInRange(playerAttackDistance))
        {
            SetState(attackState);
        } else if (IsPlayerInRange(playerAggroDistance))
        {
            SetState(chaseState);
        } 

        currentState.OnUpdate();
    }

    private void SetState(State newState)
    {
        if (currentState != null && currentState == newState) return;

        currentState?.OnExit();
        currentState = newState;
        currentState.OnEnter();
    }

    private bool IsPlayerInRange(float range)
    {
        var player = FindObjectOfType<Player>();
        if (player == null) return false;
        
        float distanceFromPlayer = (FindObjectOfType<Player>().transform.position - transform.position).magnitude;

        return distanceFromPlayer <= range;

    }

    public void SetMovement(Vector2 targetPosition)
    {
        var normalizedInputVector = (targetPosition - (Vector2)transform.position).normalized;

        lastMoveDirection = normalizedInputVector == Vector2.zero ? lastMoveDirection : normalizedInputVector;

        animator.SetBool("IsMoving", normalizedInputVector != Vector2.zero);
        animator.SetFloat("X Motion", lastMoveDirection.x);
        animator.SetFloat("Y Motion", lastMoveDirection.y);

        rigidBody.velocity = normalizedInputVector * currentSpeed;
    }

    public void SetSpeed(Speed speed)
    {
        switch (speed)
        {
            case (Speed.Stop):
                currentSpeed = 0f;
                break;
            case (Speed.Walk):
                currentSpeed = walkSpeed;
                break;
            case (Speed.Run):
                currentSpeed = runSpeed;
                break;
            default:
                break;
        }
    }

    public void TakeDamage(int damage, Vector3 sourcePosition, float knockBackDistance)
    {
        health.ChangeHealth(-damage);

        transform.position += (transform.position - sourcePosition).normalized * knockBackDistance;
    }
}
