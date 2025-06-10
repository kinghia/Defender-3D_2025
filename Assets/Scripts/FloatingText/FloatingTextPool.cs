using UnityEngine;
using System.Collections.Generic;

public class FloatingTextPool : MonoBehaviour
{
    [Header("Pool Settings")]
    public FloatingText floatingTextPrefab;
    public int initialPoolSize = 20;

    private readonly Queue<FloatingText> pool = new Queue<FloatingText>();

    void Awake()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewInstance();
        }
    }

    private FloatingText CreateNewInstance()
    {
        var instance = Instantiate(floatingTextPrefab, transform);
        instance.gameObject.SetActive(false);
        return instance;
    }

    public FloatingText Get()
    {
        FloatingText instance;
        if (pool.Count > 0)
        {
            instance = pool.Dequeue();
        }
        else
        {
            instance = CreateNewInstance();
        }
        instance.gameObject.SetActive(true);
        return instance;
    }

    public void ReturnToPool(FloatingText instance)
    {
        instance.gameObject.SetActive(false);
        pool.Enqueue(instance);
    }
} 