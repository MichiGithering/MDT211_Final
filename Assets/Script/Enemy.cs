using UnityEngine;

public enum AIState
{
    Idle,
    Chase,
    Attack,
    Dead
}

public class Enemy : Character
{
    [Header("Enemy Settings")]
    public int ExpDrop = 20;
    public float DetectRange = 5f;
    public float AttackRange = 1.5f;

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
            case AIState.Patrol:
                Patrol();
                break;
            case AIState.Chase:
                Chase(target ? target.GetComponent<Player>() : null);
                break;
            case AIState.Attack:
                AttackTarget(target ? target.GetComponent<Character>() : null);
                break;
        }
    }

  
    public override void AttackTarget(Character player)
    {
        if (IsDead) return;
        if (player == null || player.IsDead) return;

        float distance = Vector2.Distance(transform.position, player.transform.position);
        if (distance > AttackRange) return;

       
        Vector3 scale = transform.localScale;
        if (player.transform.position.x < transform.position.x)
            scale.x = Mathf.Abs(scale.x) * -1f;
        else
            scale.x = Mathf.Abs(scale.x);
        transform.localScale = scale;

        float dmg = Attack;
        player.TakeDamage(dmg);
        Debug.Log($"{Name} attacked {player.Name} for {dmg} damage.");
    }

    public override void Attack(Player player)
    {
        AttackTarget(player);
    }

    public void Chase(Player player)
    {
        if (IsDead) return;
        if (player == null || player.IsDead) return;

        Vector2 dir = (player.transform.position - transform.position);
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
