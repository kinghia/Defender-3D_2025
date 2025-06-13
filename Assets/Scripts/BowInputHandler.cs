using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BowController))]
public class BowInputHandler : MonoBehaviour
{
    private BowController bowController;
    private PlayerInput playerInput;
    private InputAction aimAction;

    private void Awake()
    {
        bowController = GetComponent<BowController>();
        
        // Create and setup PlayerInput component
        playerInput = gameObject.AddComponent<PlayerInput>();
        var inputActions = Resources.Load<InputActionAsset>("Input/BowControls");
        if (inputActions == null)
        {
            Debug.LogError("Could not load BowControls input actions!");
            return;
        }
        
        playerInput.actions = inputActions;
        playerInput.defaultActionMap = "Bow";
        
        // Get the aim action
        aimAction = playerInput.actions.FindAction("Bow/Aim");
        if (aimAction == null)
        {
            Debug.LogError("Could not find Aim action in BowControls!");
            return;
        }
    }

    private void OnEnable()
    {
        if (aimAction != null)
        {
            aimAction.performed += OnAimPerformed;
            aimAction.canceled += OnAimCanceled;
            aimAction.Enable();
        }
    }

    private void OnDisable()
    {
        if (aimAction != null)
        {
            aimAction.performed -= OnAimPerformed;
            aimAction.canceled -= OnAimCanceled;
            aimAction.Disable();
        }
    }

    private void OnAimPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Aim started");
        bowController.SetAiming(true);
    }

    private void OnAimCanceled(InputAction.CallbackContext context)
    {
        Debug.Log("Aim ended");
        bowController.SetAiming(false);
        bowController.ShootArrow();
    }
} 