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
    public float DetectRange = 5f;
    // Keep this small (e.g., 0.8f) so they stop close to the player
    public float AttackRange = 0.8f;

    [Header("AI")]
    public AIState currentState = AIState.Idle;
    public Transform target;

    private void Start()
    {
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                target = playerObj.transform;
        }
    }

    private void Update()
    {
        if (IsDead)
        {
            SetState(AIState.Dead);
            return;
        }

        if (target == null)
        {
            SetState(AIState.Patrol);
        }
        else
        {
            // Use Vector2.Distance to ignore Z axis (Strict 2D check)
            float distance = Vector2.Distance(transform.position, target.position);

            if (distance <= AttackRange)
                SetState(AIState.Attack);
            else if (distance <= DetectRange)
                SetState(AIState.Chase);
            else
                SetState(AIState.Patrol);
        }

        switch (currentState)
        {
            // case AIState.Patrol:
            //    Patrol(); 
            //    break;

            case AIState.Chase:
                Chase(target ? target.GetComponent<Player>() : null);
                break;

            case AIState.Attack:
                // === NEW CODE: ATTACK COOLDOWN CHECK ===
                // This checks if enough time has passed since the last attack
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    // Reset the timer to the current game time
                    lastAttackTime = Time.time;

                    // Perform the attack
                    AttackTarget(target ? target.GetComponent<Character>() : null);
                }
                break;
        }
    }

    public override void AttackTarget(Character player)
    {
        if (IsDead) return;
        if (player == null || player.IsDead) return;

        float distance = Vector2.Distance(transform.position, player.transform.position);
        if (distance > AttackRange) return;

        // Visual Flip
        Vector3 scale = transform.localScale;
        if (player.transform.position.x < transform.position.x)
            scale.x = Mathf.Abs(scale.x) * -1f; // Face Left
        else
            scale.x = Mathf.Abs(scale.x);       // Face Right
        transform.localScale = scale;

        float dmg = AttackDamage;

        player.TakeDamage(dmg);
        Debug.Log($"{Name} attacked {player.Name} for {dmg} damage.");
    }

    public void Chase(Player player)
    {
        if (IsDead) return;
        if (player == null || player.IsDead) return;

        // Calculate 2D direction only
        Vector2 dir = (Vector2)player.transform.position - (Vector2)transform.position;
        dir.Normalize();
        Move(dir);
    }

    public void DropLoot()
    {
        Debug.Log($"{Name} dropped loot and {ExpDrop} EXP.");
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
        DropLoot();
    }
}