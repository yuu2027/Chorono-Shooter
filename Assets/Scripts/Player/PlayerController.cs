using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // PlayerMovementクラスを取得
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerShooter playerShooter;
    [SerializeField] private PlayerInputReader inputReader;

    private void Awake()
    {
        if (playerMovement == null) playerMovement = GetComponent<PlayerMovement>(); // PlayerMovementコンポーネントの取得
        if (playerShooter == null) playerShooter = GetComponent<PlayerShooter>();    // PlayerShooterコンポーネントの取得
        if (inputReader == null) inputReader = GetComponent<PlayerInputReader>();
    }

    private void OnEnable() // 有効になった時に呼ばれる
    {
        if (inputReader != null)
        {
            inputReader.PausePressed += HandlePausePressed;
        }
    }

    private void OnDisable() // 無効になった時に呼ばれる
    {
        if (inputReader != null)
        {
            inputReader.PausePressed -= HandlePausePressed;
        }
    }

    void Update()
    {
        if (!CanControlPlayer()) return;

        if (playerMovement != null)
        {
            Vector2 moveInput = inputReader != null ? inputReader.MoveInput : Vector2.zero;
            playerMovement.SetMoveInput(moveInput);
        }

        if (playerShooter != null)
        {
            bool shootHeld = inputReader != null && inputReader.ShootHeld;
            playerShooter.TickShoot(shootHeld);
        }
    }

    private void FixedUpdate()
    {
        if (!CanControlPlayer()) return;

        if (playerMovement != null) playerMovement.Move(); // プレイヤーを移動させる

    }

    // プレイヤーがコントロールできる状態か
    private bool CanControlPlayer()
    {
        return GameManager.Instance == null || GameManager.Instance.CurrentState == GameState.Playing;
    }

    private void HandlePausePressed()
    {
        if (GameManager.Instance == null) return;

        AudioManager.Instance?.PlaySe(SeId.Pause);
        GameManager.Instance.TogglePause();
    }
}
