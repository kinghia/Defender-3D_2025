using UnityEngine;
using System;

public abstract class BulletBase : MonoBehaviour
{
    public float speed = 10f;
    protected Transform target;
    public event Action<Transform> OnHit;

    public virtual void SetTarget(Transform target)
    {
        this.target = target;
    }

    // protected virtual void Update()
    // {
    //     if (target == null)
    //     {
    //         Destroy(gameObject);
    //         return;
    //     }
    //     Vector3 dir = target.position - transform.position;
    //     float distanceThisFrame = speed * Time.deltaTime;
    //     if (dir.magnitude <= distanceThisFrame)
    //     {
    //         HitTarget();
    //         return;
    //     }
    //     transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    // }

    protected virtual void HitTarget()
    {
        if (target == null)
        {
            Debug.LogWarning("Target is null when trying to hit");
            Destroy(gameObject);
            return;
        }

        OnHit?.Invoke(target);
        Destroy(gameObject);
    }

    protected virtual void Hit(Transform target) 
    {
        if (target == null)
        {
            Debug.LogWarning("Target is null when trying to hit");
            return;
        }
        
        OnHit?.Invoke(target);
    }
} 