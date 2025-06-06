using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyMover : MonoBehaviour
{
    [SerializeField] float destroyX = -20f;
    bool movement = true;

    EnemyStats enemyStats;

    void Awake()
    {
        enemyStats = GetComponent<EnemyStats>();
        if (enemyStats == null)
        {
            Debug.LogError("EnemyStats component not found on " + gameObject.name);
        }
    }

    void OnEnable()
    {
        // Reset position when enabled
        float randomZ = Random.Range(-10f, 10f);
        transform.position = new Vector3(20f, 0f, randomZ);
    }

    void Update()
    {
        if (enemyStats == null) return;

        // Move left
        if (movement)
        {
            transform.Translate(Vector3.left * enemyStats.GetMoveSpeed() * Time.deltaTime);
        }

        // Check if out of bounds
        if (transform.position.x <= destroyX)
        {
            movement = false;
        }
    }
}
