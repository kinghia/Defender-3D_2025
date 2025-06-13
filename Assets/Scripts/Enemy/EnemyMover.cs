using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyMover : MonoBehaviour
{
    private EnemyStats enemyStats;
    private bool isMoving = true;
    private Transform castleTransform;
    private float attackRange;

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
        isMoving = true;
    }

    public void Initialize(Transform castle, float range)
    {
        castleTransform = castle;
        attackRange = range;
    }

    void Update()
    {
        if (enemyStats == null || castleTransform == null) return;

        if (isMoving)
        {
            // Check if in attack range
            float distanceToCastle = Mathf.Abs(transform.position.x - castleTransform.position.x);
            if (distanceToCastle <= attackRange)
            {
                isMoving = false;
                GetComponent<Enemy>()?.StartAttacking();
                return;
            }

            // Move left on X axis only
            transform.Translate(Vector3.left * enemyStats.GetMoveSpeed() * Time.deltaTime);
        }
    }

    public void StopMoving()
    {
        isMoving = false;
    }

    public void ResumeMoving()
    {
        isMoving = true;
    }
}
