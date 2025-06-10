using UnityEngine;
using System;

public class ArrowBehavior : BulletBase
{
    public float arcHeight = 2f;
    private Vector3 startPos;
    private float flightTime;
    private float elapsed;
    private bool initialized = false;

    public override void SetTarget(Transform target)
    {
        base.SetTarget(target);
        startPos = transform.position;
        elapsed = 0f;
        if (target != null)
        {
            float distance = Vector3.Distance(startPos, target.position);
            flightTime = distance / speed;
            initialized = true;
            transform.LookAt(target.position);
        }
    }

    void FixedUpdate()
    {
        if (!initialized || target == null)
        {
            Destroy(gameObject);
            return;
        }
        elapsed += Time.fixedDeltaTime;
        float t = Mathf.Clamp01(elapsed / flightTime);
        Vector3 endPos = target.position;
        Vector3 currentPos = Vector3.Lerp(startPos, endPos, t);
        // Thêm độ cao parabol
        float height = Mathf.Sin(Mathf.PI * t) * arcHeight;
        currentPos.y += height;
        transform.position = currentPos;
        // Quay đầu mũi tên theo hướng bay
        if (t < 1f)
        {
            Vector3 nextPos = Vector3.Lerp(startPos, endPos, Mathf.Clamp01((elapsed + 0.05f) / flightTime));
            nextPos.y += Mathf.Sin(Mathf.PI * Mathf.Clamp01((elapsed + 0.05f) / flightTime)) * arcHeight;
            Vector3 dir = nextPos - transform.position;
            if (dir.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(dir) * Quaternion.Euler(90, 0, 0);
        }
        // Đến đích
        if (t >= 1f)
        {
            HitTarget();
        }
    }
} 