using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Character : MonoBehaviour
{
    [Header("Identity")]
    [SerializeField] protected string characterName;
    [SerializeField] protected int level = 1;

    [Header("Stats")]
    [SerializeField] protected float maxHP = 100f;
    [SerializeField] protected float currentHP = 100f;
    [SerializeField] protected float attack = 10f;
    [SerializeField] protected float defense = 0f;
    [SerializeField] protected float moveSpeed = 5f;

    // NEW: Variable to control how fast the character can attack (in seconds)
    [SerializeField] protected float attackCooldown = 1.0f;

    protected Rigidbody2D rb;

    // NEW: Variable to track the exact time the last attack happened
    protected float lastAttackTime = -999f;

    public string Name => characterName;
    public int Level => level;
    public float MaxHP => maxHP;
    public float CurrentHP => currentHP;
    public float AttackDamage => attack;
    public float Defense => defense;
    public float MoveSpeed => moveSpeed;

    // NEW: Public property so other scripts can read the cooldown
    public float AttackCooldown => attackCooldown;

    public bool IsDead => currentHP <= 0f;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public abstract void AttackTarget(Character target);

    public virtual void Move(Vector2 direction)
    {
        if (IsDead) return;

        if (direction.sqrMagnitude > 0.01f)
        {
            direction.Normalize();
            Vector2 targetPosition = rb.position + (direction * moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(targetPosition);
        }
    }

    public virtual void TakeDamage(float amount)
    {
        if (IsDead) return;

        float finalDamage = Mathf.Max(0f, amount - defense);
        currentHP -= finalDamage;

        if (currentHP <= 0f)
        {
            currentHP = 0f;
            OnDeath();
        }
    }

    public virtual void Heal(float amount)
    {
        if (IsDead) return;
        if (amount <= 0f) return;

        currentHP = Mathf.Min(maxHP, currentHP + amount);
    }

    protected virtual void OnDeath()
    {
        Debug.Log($"{characterName} has died.");
    }

    public virtual void SetStats(float newMaxHP, float newAttack, float newDefense, float newMoveSpeed)
    {
        maxHP = newMaxHP;
        attack = newAttack;
        defense = newDefense;
        moveSpeed = newMoveSpeed;

        currentHP = Mathf.Clamp(currentHP, 0f, maxHP);
    }
}