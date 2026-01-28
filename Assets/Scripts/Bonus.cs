using UnityEngine;
using System.Collections;

public class Bonus : MonoBehaviour
{
    public Collider2D gridArea;
    private Snake snake;
    private bool eaten;
    protected int bonusIndex;
    private SpriteRenderer spriteRenderer;

    public Sprite FastBoostSprite;
    public Sprite SlowBoostSprite;

    public int gridWidth = 17;
    public int gridHeight = 15;

    private void Awake()
    {
        snake = FindFirstObjectByType<Snake>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Invoke("settleRandomBonus", 5f);
    }

    public void settleRandomBonus()
    {
        eaten = false;
        bonusIndex = UnityEngine.Random.Range(0, 2);

        if (bonusIndex == 1)
        {
            if (FastBoostSprite != null)
            {
                spriteRenderer.sprite = FastBoostSprite;
            }
        }
        else
        {
            if (SlowBoostSprite != null)
            {
                spriteRenderer.sprite = SlowBoostSprite;
            }
        }

        RandomizePosition();
    }

    public void RandomizePosition()
    {
        Bounds bounds = gridArea.bounds;

        float cellWidth = bounds.size.x / gridWidth;
        float cellHeight = bounds.size.y / gridHeight;

        int xIndex;
        int yIndex;

        do
        {
            xIndex = UnityEngine.Random.Range(0, gridWidth);
            yIndex = UnityEngine.Random.Range(0, gridHeight);
        }
        while (snake != null && snake.Occupies(xIndex, yIndex));

        float x = bounds.min.x + cellWidth * (xIndex + 0.5f);
        float y = bounds.min.y + cellHeight * (yIndex + 0.5f);

        transform.position = new Vector2(x, y);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Snake"))
        {
            eaten = true;
            transform.position = new Vector2(-30, -30);
            Invoke("settleRandomBonus", 5f);
        }
    }

    private void RespawnBonus()
    {
        if (!eaten)
        {
            transform.position = new Vector2(-30, -30);
            Invoke("settleRandomBonus", 7f);
        }
    }
}