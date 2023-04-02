using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySlime : MonoBehaviour, IDamageTaker
{
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private LayerMask attackTargets;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private Health health;
    [SerializeField] private List<Transform> patrolTransforms;

    private List<Vector2> patrolPositions;

    private int currentPatrolPositionIndex = -1;

    private Rigidbody2D rigidBody;
    private Animator animator;

    private Vector2 lastMoveDirection = Vector2.zero;

    enum States
    {
        Patrolling,
        Chasing,
        Attacking
    }

    States currentState = States.Patrolling;

    // Start is called before the first frame update
    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        InitializePatrolPositions();
    }

    void InitializePatrolPositions()
    {
        patrolPositions = new List<Vector2>();

        foreach (Transform t in patrolTransforms)
            patrolPositions.Add(t.position);
    }

    private void OnEnable()
    {
        health.OnHealthEmpty += Health_OnHealthEmpty;
    }

    private void Health_OnHealthEmpty(object sender, System.EventArgs e)
    {
        gameObject.SetActive(false);
    }
    /*
    private void OnAttackPressed(object sender, System.EventArgs e)
    {
        animator.SetTrigger("Attack");
        Singleton.Instance.SoundManager.Play(attackSound);

        float attackRadius = 1f;
        float attackDistance = 2.5f;

        if (Physics2D.CircleCast(transform.position, attackRadius, lastMoveDirection, attackDistance, attackTargets))
        {
            Debug.Log("hit");
        }
    }
    */
    private void OnDisable()
    {
        health.OnHealthEmpty -= Health_OnHealthEmpty;
    }

    // Update is called once per frame
    private void Update()
    {
        switch (currentState)
        {
            case States.Patrolling:
                HandlePatrolling();
                break;
            case States.Chasing:
                break;
            case States.Attacking:
                break;
            default:
                break;

        }
    }

    void HandlePatrolling()
    {

        if (currentPatrolPositionIndex == -1)
        {
            currentPatrolPositionIndex = 0;
            SetMovement(patrolPositions[currentPatrolPositionIndex]);
        }

        var maxDistanceFromPatrolPosition = 0.1f;
        var distanceToPatrolPosition = ((Vector2)transform.position - patrolPositions[currentPatrolPositionIndex]).magnitude;

        if (distanceToPatrolPosition <= maxDistanceFromPatrolPosition)
        {
            currentPatrolPositionIndex = (currentPatrolPositionIndex + 1) % patrolPositions.Count;
            SetMovement(patrolPositions[currentPatrolPositionIndex]);
        }
    }

    void SetMovement(Vector2 targetPosition)
    {
        var normalizedInputVector = (targetPosition - (Vector2)transform.position).normalized;

        lastMoveDirection = normalizedInputVector == Vector2.zero ? lastMoveDirection : normalizedInputVector;

        animator.SetBool("IsMoving", normalizedInputVector != Vector2.zero);
        animator.SetFloat("X Motion", lastMoveDirection.x);
        animator.SetFloat("Y Motion", lastMoveDirection.y);

        rigidBody.velocity = normalizedInputVector * walkSpeed;
    }

    public void TakeDamage(int damage, Vector3 sourcePosition, float knockBackDistance)
    {
        health.ChangeHealth(-damage);

        transform.position += (transform.position - sourcePosition).normalized * knockBackDistance;
    }
}
