using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // PlayerMovementクラスを取得
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerShooter playerShooter;

    private void Awake()
    {
        if (playerMovement == null) playerMovement = GetComponent<PlayerMovement>(); // PlayerMovementコンポーネントの取得
        if (playerShooter == null) playerShooter = GetComponent<PlayerShooter>(); // PlayerShooterコンポーネントの取得
    }

    void Update()
    {
        if (playerMovement != null) playerMovement.ReadInput(); // キーボード入力を取得
        if (playerShooter != null) playerShooter.TickShoot();
    }

    private void FixedUpdate()
    {
        playerMovement.Move(); // プレイヤーを移動させる
    }
}
