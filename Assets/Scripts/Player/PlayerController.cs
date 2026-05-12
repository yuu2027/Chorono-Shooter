using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // PlayerMovementクラスを取得
    [SerializeField] private PlayerMovement playerMovement;

    private void Amake()
    {
        if (playerMovement == null) playerMovement = GetComponent<PlayerMovement>(); // PlayerMovementコンポーネントの取得
    }

    void Update()
    {
        playerMovement.ReadInput(); // キーボード入力を取得
    }

    private void FixedUpdate()
    {
        playerMovement.Move(); // プレイヤーを移動させる
    }
}
