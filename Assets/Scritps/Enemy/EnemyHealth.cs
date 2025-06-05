using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyHealth : MonoBehaviour
{
    [SerializeField] int maxHitPoints = 5;
    //[SerializeField] int maxHealth = 1;

    [SerializeField] int currentHitPoints = 0;
    public int CurrentHitPoints { get { return currentHitPoints; } }

    Enemy enemy;

    void OnEnable()
    {
        currentHitPoints = maxHitPoints;
    }

    void Start()
    {
        enemy = GetComponent<Enemy>();
        //healthBar.SetMaxHealth(maxHealth);
    }
    void OnParticleCollision(GameObject other)
    {
        ProcessHit();
        Debug.Log("da bi ban");
    }
    
    private void ProcessHit()
    {
        currentHitPoints--;

        if (currentHitPoints <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}