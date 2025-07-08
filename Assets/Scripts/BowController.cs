using UnityEngine;
using UnityEngine.InputSystem;

public class BowController : MonoBehaviour
{
    [Header("Bow Settings")]
    [SerializeField] private float maxShootAngle = 60f; // Maximum angle in degrees from forward direction
    [SerializeField] private float arrowSpeed = 20f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private TowerData towerData; // Using existing tower data for damage values

    private Camera mainCamera;
    private Vector3 mousePosition;
    private Animator anim;
    private bool isAiming;

    private void Start()
    {
        anim = GetComponent<Animator>();    

        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found! Make sure your camera is tagged as MainCamera.");
        }
        if (firePoint == null)
        {
            Debug.LogError("Fire point not assigned to BowController!");
        }
        if (arrowPrefab == null)
        {
            Debug.LogError("Arrow prefab not assigned to BowController!");
        }
        if (towerData == null)
        {
            Debug.LogError("Tower data not assigned to BowController!");
        }
    }

    private void Update()
    {
        if (isAiming)
        {
            // Lấy vị trí chuột trong thế giới bằng Mouse3D
            Vector3 mouseWorldPosition = Mouse3D.GetMouseWorldPosition();

            // Calculate direction to mouse
            Vector3 direction = (mouseWorldPosition - transform.position).normalized;
            direction.y = 0; // Keep arrows horizontal

            // Check if direction is within allowed angle
            float angle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
            if (Mathf.Abs(angle) <= maxShootAngle)
            {
                // Rotate bow to face mouse
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }

    public void SetAiming(bool aiming)
    {
        isAiming = aiming;
        Debug.Log($"Aiming set to: {aiming}");
    }

    public void ShootArrow()
    {
        if (arrowPrefab == null || firePoint == null)
        {
            Debug.LogError("Arrow prefab hoặc fire point chưa được gán!");
            return;
        }

        // Lấy vị trí chuột trong thế giới bằng Mouse3D
        Vector3 mouseWorldPosition = Mouse3D.GetMouseWorldPosition();
        Vector3 direction = (mouseWorldPosition - transform.position).normalized;
        direction.y = 0; // Keep arrows horizontal

        // Check if direction is within allowed angle
        float angle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
        if (Mathf.Abs(angle) > maxShootAngle)
        {
            Debug.Log("Không thể bắn theo hướng này - vượt quá giới hạn góc");
            return;
        }

        // Spawn arrow
        anim.SetTrigger("Attack");
        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.LookRotation(direction));
        Arrow arrowComponent = arrow.GetComponent<Arrow>();
        if (arrowComponent != null)
        {
            arrowComponent.Initialize(direction, arrowSpeed, towerData);
            Debug.Log($"Arrow shot! Direction: {direction}, Speed: {arrowSpeed}");
        }
        else
        {
            Debug.LogError("Arrow prefab is missing Arrow component!");
        }
    }
} 