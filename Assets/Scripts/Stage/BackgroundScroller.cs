using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [SerializeField] private Transform[] backgroundTiles;

    [SerializeField] private float scrollSpeed = 2.0f;
    [SerializeField] private float recycleThresholdY = -20.0f;
    [SerializeField] private float tileHeight = 20.0f;

    [SerializeField] private int backgroundSortingOrder = -100;

    private void Awake()
    {
        for (int i = 0; i < backgroundTiles.Length; i++)
        {
            SpriteRenderer spriteRenderer = backgroundTiles[i].GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = backgroundSortingOrder;
            }
        }
    }

    private void Update()
    {
        if (GameManager.Instance != null &&
            GameManager.Instance.CurrentState != GameState.Playing)
        {
            return;
        }

        float deltaTime = TimeController.EnemyDeltaTime;
        Vector3 moveAmount = Vector3.down * scrollSpeed * deltaTime;

        for (int i = 0; i < backgroundTiles.Length; i++)
        {
            backgroundTiles[i].position += moveAmount;
        }

        for (int i = 0; i < backgroundTiles.Length; i++)
        {
            if (backgroundTiles[i].position.y <= recycleThresholdY)
            {
                MoveToTop(backgroundTiles[i]);
            }
        }
    }

    private void MoveToTop(Transform background)
    {
        float topY = GetTopBackgroundY();

        Vector3 nextPosition = background.position;
        nextPosition.y = topY + tileHeight;
        background.position = nextPosition;
    }

    private float GetTopBackgroundY()
    {
        float topY = backgroundTiles[0].position.y;

        for (int i = 1; i < backgroundTiles.Length; i++)
        {
            if (backgroundTiles[i].position.y > topY)
            {
                topY = backgroundTiles[i].position.y;
            }
        }

        return topY;
    }
}