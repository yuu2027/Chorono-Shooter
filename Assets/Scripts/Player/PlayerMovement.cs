using UnityEngine;
using UnityEngine.InputSystem;

// Rigidbody2Dを必ず使用する。unityが自動的に追加してくれる
[RequireComponent (typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Move Settings")] // インスペクタービューに表示される
    [SerializeField] private float moveSpeed = 5.0f; // プレイヤーのスピード

    [Header("Move Limit")] // インスペクタービューに表示される
    [SerializeField] private Vector2 minPosition = new Vector2(-8.0f, -4.5f); // 移動できる最小値
    [SerializeField] private Vector2 maxPosition = new Vector2(8.0f, 4.5f);   // 移動できる最大値

    private Rigidbody2D rb;     // Rigidbody2Dを使用する
    private Vector2 moveInput; // どの方向に移動するか

    // オブジェクトが有効化された直後に呼ばれる関数
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2Dコンポーネントを取得

        rb.gravityScale = 0.0f; // 重力0
        rb.freezeRotation = true; //回転なし
    }

    // キー入力から移動方向を導く関数
    public void ReadInput()
    {
        Keyboard keyboard = Keyboard.current; // 現在使用されているキーボードを取得

        if (keyboard == null) // 入力がないとき
        {
            moveInput = Vector2.zero; // 移動を0
            return;
        }

        float x = 0.0f; // x座標
        float y = 0.0f; // y座標 

        // 上下左右(a：左 d：右 w：上 s：下)
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) x -= 1.0f;  // 左
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) x += 1.0f; // 右
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) y += 1.0f;    // 上
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) y -= 1.0f;  // 下

        moveInput = new Vector2(x, y); // 移動方向を代入

        // ベクトルの長さ(sqrMagnitude)が1より大きいとき1に正規化(Normalize)
        // 左上や右上などの場合、値が大きくなり早くなるため1に正規化する
        if (moveInput.sqrMagnitude > 1.0f) moveInput.Normalize();

    }

    // プレイヤーを移動させる関数
    public void Move()
    {
        // 現在地(position)+移動方向(moveInput)*移動スピード(moveSpeed)*間隔(fixedDeltaTime)
        // 1秒間にmoveSpeed進むとするため、moveSpeed * Time.fixedDeltaTime(FixedUpdate()が1回呼ばれる間隔)にする
        Vector2 nextPosition = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;

        // Mathf.Clampは、値を最小値から最大値の範囲内に収める関数
        // 移動を制限する
        nextPosition.x = Mathf.Clamp(nextPosition.x, minPosition.x, maxPosition.x);
        nextPosition.y = Mathf.Clamp(nextPosition.y, minPosition.y, maxPosition.y);

        rb.MovePosition(nextPosition); // 移動させる
    }
}
