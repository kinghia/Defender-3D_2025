using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    private TowerData towerData;
    private float maxDistance = 100f; // Maximum distance arrow can travel
    private Vector3 startPosition;

    public void Initialize(Vector3 direction, float speed, TowerData towerData)
    {
        this.direction = direction;
        this.speed = speed;
        this.towerData = towerData;
        startPosition = transform.position;
    }

    private void Update()
    {
        // Move arrow
        transform.position += direction * speed * Time.deltaTime;

        // Check if arrow has traveled too far
        if (Vector3.Distance(startPosition, transform.position) > maxDistance)
        {
            Destroy(gameObject);
            return;
        }
    }

    private Enemy FindNearestEnemy()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        float minDistance = float.MaxValue;
        Enemy nearest = null;

        foreach (Enemy enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = enemy;
            }
        }

        return nearest;
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            // Deal damage to enemy
            EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                float damage = towerData.physicalDamage;
                enemyStats.TakeDamage(damage, DamageType.Physical);
                Debug.Log($"Arrow hit enemy! Dealt {damage} damage");

                // Show floating text
                if (FloatingTextSpawner.Instance != null)
                {
                    FloatingTextSpawner.Instance.SpawnText(
                        damage.ToString("F0"),
                        enemy.transform.position + Vector3.up,
                        FloatingTextType.Physic
                    );
                }
            }

            // Destroy arrow
            Destroy(gameObject);
        }
    }
} 