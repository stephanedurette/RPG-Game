using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageTaker
{
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private LayerMask attackTargets;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private Health health;

    private Rigidbody2D rigidBody;
    private Animator animator;

    private Vector2 lastMoveDirection = Vector2.zero;

    // Start is called before the first frame update
    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        InputManager.OnAttackPressed += OnAttackPressed;
        health.OnHealthEmpty += Health_OnHealthEmpty;
    }

    private void Health_OnHealthEmpty(object sender, System.EventArgs e)
    {
        FindObjectOfType<GameOverPanel>().TogglePanel(true);
        gameObject.SetActive(false);
    }

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

    private void OnDisable()
    {
        InputManager.OnAttackPressed -= OnAttackPressed;
        health.OnHealthEmpty -= Health_OnHealthEmpty;

    }

    // Update is called once per frame
    private void Update()
    {
        var normalizedInputVector = InputManager.MoveVector.normalized;

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
