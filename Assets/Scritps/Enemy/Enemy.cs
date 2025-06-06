using UnityEngine;

public class Enemy : MonoBehaviour
{
    private EnemyStats stats;
    private EnemyHpBar hpBar;

    void OnEnable()
    {
        Init();
    }

    public void Init()
    {
        hpBar = GetComponentInChildren<EnemyHpBar>();
        stats = GetComponent<EnemyStats>();
        
        if (stats == null)
        {
            Debug.LogError($"EnemyStats component not found on {gameObject.name}");
            return;
        }
        
        if (hpBar == null)
        {
            Debug.LogError($"EnemyHpBar component not found on {gameObject.name}");
            return;
        }

        stats.Initialize();
        hpBar.Init(stats);
    }
}
