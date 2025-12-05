using UnityEngine;

public class FireAttack : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject fireProjectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10f;
    public float projectileLifeTime = 5f;

    [Header("Cooldown")]
    public float cooldown = 2f;
    private float nextAllowedTime = 0f;

    private Character owner;
    private SpriteRenderer ownerRenderer;

    private void Awake()
    {
        owner = GetComponent<Character>();
        ownerRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryAttack();
        }
    }

    private void TryAttack()
    {
        if (Time.time < nextAllowedTime) return;

        nextAllowedTime = Time.time + cooldown;

        ShootProjectile();
    }

    private void ShootProjectile()
    {
        GameObject obj = Instantiate(fireProjectilePrefab, firePoint.position, firePoint.rotation);

        // Add projectile logic directly
        var projectile = obj.AddComponent<ProjectileLogic>();

        // Determine direction by SpriteRenderer.flipX
        float direction = ownerRenderer != null && ownerRenderer.flipX ? -1f : 1f;

        projectile.Initialize(owner, projectileSpeed * direction, projectileLifeTime);
    }
    private class ProjectileLogic : MonoBehaviour
    {
        private Character owner;
        private float speed;
        private float lifeTime;

        private float timer;

        private int fixedDamage = 2; // damage = 2

        private SpriteRenderer sprite;

        public void Initialize(Character owner, float speed, float lifeTime)
        {
            this.owner = owner;
            this.speed = speed;
            this.lifeTime = lifeTime;

            sprite = GetComponent<SpriteRenderer>();

            // กลับด้าน sprite ตามทิศ
            if (sprite != null)
            {
                sprite.flipX = speed < 0;
            }
        }

        private void Update()
        {
            // เดินไปทางขวาหรือซ้ายตาม speed ตลอดเวลา
            transform.Translate(Vector3.right * speed * Time.deltaTime);

            // นับเวลา
            timer += Time.deltaTime;

            // ถ้าเกิน lifeTime (เช่น 5 วิ) ให้ลบกระสุน
            if (timer >= lifeTime)
                Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Character target = other.GetComponent<Character>();

            if (target != null && target != owner)
            {
                target.TakeDamage(fixedDamage);
                Destroy(gameObject);
            }
        }
    }
}
