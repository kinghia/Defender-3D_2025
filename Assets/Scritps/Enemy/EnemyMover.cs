using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyMover : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float destroyX = -20f;
    bool movement = true;

    void OnEnable()
    {
        // Reset position when enabled
        float randomZ = Random.Range(-10f, 10f);
        transform.position = new Vector3(20f, 0f, randomZ);
    }

    void Update()
    {
        // Move left
        if (movement)
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);
        }

        // Check if out of bounds
        if (transform.position.x <= destroyX)
        {
            movement = false;
        }
    }
}
