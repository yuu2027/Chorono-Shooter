using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour
{
    public static PlayerInputReader Instance { get; private set; }

    [SerializeField] private InputActionAsset inputActions;

    private InputActionMap playerMap;
    private InputAction moveAction;
    private InputAction shootAction;
    private InputAction slowAction;
    private InputAction pauseAction;

    public Vector2 MoveInput => moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
    public bool ShootHeld => shootAction != null && shootAction.IsPressed();
    public bool SlowHeld => slowAction != null && slowAction.IsPressed();

    public event Action PausePressed;

    private void Awake()
    {
        Instance = this;

        if (inputActions == null)
        {
            Debug.LogWarning("PlayerInputReader: InputActionAsset Ç™ñ¢ê›íËÇ≈Ç∑ÅB", this);
            return;
        }

        playerMap = inputActions.FindActionMap("Player", true);
        moveAction = playerMap.FindAction("Move", true);
        shootAction = playerMap.FindAction("Shoot", true);
        slowAction = playerMap.FindAction("Slow", true);
        pauseAction = playerMap.FindAction("Pause", true);
    }

    private void OnEnable()
    {
        playerMap?.Enable();

        if (pauseAction != null)
        {
            pauseAction.performed += OnPausePerformed;
        }
    }

    private void OnDisable()
    {
        if (pauseAction != null)
        {
            pauseAction.performed -= OnPausePerformed;
        }

        playerMap?.Disable();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        PausePressed?.Invoke();
    }
}