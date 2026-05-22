using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // PlayerMovementクラスを取得
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerShooter playerShooter;

    private void Awake()
    {
        if (playerMovement == null) playerMovement = GetComponent<PlayerMovement>(); // PlayerMovementコンポーネントの取得
        if (playerShooter == null) playerShooter = GetComponent<PlayerShooter>();    // PlayerShooterコンポーネントの取得
    }

    void Update()
    {
        HandleSystemInput();

        if (!CanControlPlayer()) return;

        if (playerMovement != null) playerMovement.ReadInput(); // キーボード入力を取得
        if (playerShooter != null) playerShooter.TickShoot();
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

    // リトライとポーズボタンの実装
    private void HandleSystemInput()
    {
        Keyboard keyboard = Keyboard.current;
        if(keyboard == null) return;
        if(GameManager.Instance == null) return;

        if (keyboard.escapeKey.wasPressedThisFrame)
        {
            GameManager.Instance.TogglePause();
        }

        if(keyboard.rKey.wasPressedThisFrame)
        {
            GameManager.Instance.RetryGame();
        }
    }
}
