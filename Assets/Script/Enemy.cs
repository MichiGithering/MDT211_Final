using UnityEngine;

public enum AIState
{
    Idle,
    Chase,
    Attack,
    Dead,
    Patrol
}

public class Enemy : Character
{
    [Header("Enemy Settings")]
    public int ExpDrop = 20;
    public float DetectRange = 10f;
    public float AttackRange = 1.2f;

    [Header("AI State")]
    public AIState currentState = AIState.Idle;
    public Transform target;

    [Header("Patrol Settings")]
    public float patrolSpeed = 2f; // Slower speed for patrolling
    public float patrolDuration = 3f;
    private float patrolTimer;
    private Vector2 randomPatrolDir;

    private void Start()
    {
        // Auto-find player if not assigned
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player"); // MAKE SURE PLAYER HAS "Player" TAG
            if (playerObj != null)
                target = playerObj.transform;
        }

        // Pick a random direction to start
        randomPatrolDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    private void Update()
    {
        if (IsDead)
        {
            SetState(AIState.Dead);
            return;
        }

        // 1. Decide State
        if (target == null)
        {
            SetState(AIState.Patrol);
        }
        else
        {
            float distance = Vector2.Distance(transform.position, target.position);

            if (distance <= AttackRange)
                SetState(AIState.Attack);
            else if (distance <= DetectRange)
                SetState(AIState.Chase);
            else
                SetState(AIState.Patrol);
        }

        // 2. Execute State
        switch (currentState)
        {
            case AIState.Idle:
                break;

            case AIState.Patrol:
                Patrol();
                break;

            case AIState.Chase:
                // We pass the Character component if available, or null
                Chase(target.GetComponent<Character>());
                break;

            case AIState.Attack:
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    lastAttackTime = Time.time;
                    AttackTarget(target.GetComponent<Character>());
                }
                break;

            case AIState.Dead:
                break;
        }
    }

    // --- Actions ---

    public override void AttackTarget(Character player)
    {
        if (IsDead || player == null || player.IsDead) return;

        FlipSprite(player.transform.position.x);

        float dmg = AttackDamage;
        player.TakeDamage(dmg);
        Debug.Log($"{Name} attacked {player.Name} for {dmg} damage.");
    }

    public void Chase(Character player)
    {
        if (IsDead || player == null) return;

        FlipSprite(player.transform.position.x);

        Vector2 dir = (player.transform.position - transform.position).normalized;

        // This calls the Move function in Character.cs
        Move(dir);
    }

    public void Patrol()
    {
        patrolTimer += Time.deltaTime;

        if (patrolTimer > patrolDuration)
        {
            patrolTimer = 0;
            randomPatrolDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        }

        FlipSprite(transform.position.x + randomPatrolDir.x);

        // Manually move specifically for patrol (often slower than chase)
        // Or you can use: Move(randomPatrolDir);
        if (rb != null)
        {
            Vector2 targetPosition = rb.position + (randomPatrolDir * patrolSpeed * Time.fixedDeltaTime);
            rb.MovePosition(targetPosition);
        }
    }

    private void FlipSprite(float targetX)
    {
        Vector3 scale = transform.localScale;
        if (targetX < transform.position.x)
            scale.x = Mathf.Abs(scale.x) * -1f; // Face Left
        else
            scale.x = Mathf.Abs(scale.x);       // Face Right
        transform.localScale = scale;
    }

    public void SetState(AIState state)
    {
        if (currentState == state) return;
        currentState = state;
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        SetState(AIState.Dead);

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.AddScore(ExpDrop);

        Destroy(gameObject);
    }

    // VISUAL DEBUGGING
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, DetectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }
}