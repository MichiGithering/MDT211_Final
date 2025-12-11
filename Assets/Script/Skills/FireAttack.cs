using UnityEngine;

public class FireAttack : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject fireProjectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10f;
    public float cooldown = 2f;

    private float nextAttackTime;
    private Character owner;

    private void Awake()
    {
        owner = GetComponent<Character>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + cooldown;
            Shoot();
        }
    }

    private void Shoot()
    {
        if (!fireProjectilePrefab || !firePoint)
        {
            Debug.LogWarning("FireAttack: Missing fireProjectilePrefab or firePoint.");
            return;
        }

        GameObject obj = Instantiate(fireProjectilePrefab, firePoint.position, Quaternion.identity);

        // Get mouse world direction
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0f;
        Vector3 dir = (mouse - firePoint.position).normalized;
        if (dir == Vector3.zero) dir = Vector3.right;

        // Rotate projectile toward direction
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        obj.transform.rotation = Quaternion.Euler(0, 0, angle);

        // Add the internal ProjectileLogic class
        var proj = obj.AddComponent<ProjectileLogic>();
        proj.Setup(owner, dir * projectileSpeed);
    }

    // -------------------------------------------------------------
    // INTERNAL CLASS: ProjectileLogic stays inside FireAttack.cs
    // -------------------------------------------------------------
    private class ProjectileLogic : MonoBehaviour
    {
        private Character owner;
        private Vector3 velocity;

        private const int damage = 50;

        public void Setup(Character owner, Vector3 velocity)
        {
            this.owner = owner;
            this.velocity = velocity;

            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            if (sprite) sprite.flipX = velocity.x < 0;
        }

        private void Update()
        {
            transform.position += velocity * Time.deltaTime;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            Character target = col.GetComponent<Character>();
            if (target && target != owner)
            {
                target.TakeDamage(damage);
                Destroy(gameObject);
            }
        }

        // Destroy when leaving camera view
        private void OnBecameInvisible()
        {
            Destroy(gameObject);
        }
    }
}
