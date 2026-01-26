using UnityEngine;

public class Food : MonoBehaviour
{
    public Collider2D gridArea;
    private Snake snake;
    public Sprite Pomme;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        snake = FindObjectOfType<Snake>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (Pomme != null) {
            spriteRenderer.sprite = Pomme;
        }
        RandomizePosition();

    }

    public void RandomizePosition()
    {
        Bounds bounds = gridArea.bounds;

        int x = Mathf.RoundToInt(Random.Range(bounds.min.x, bounds.max.x));
        int y = Mathf.RoundToInt(Random.Range(bounds.min.y, bounds.max.y));

        while (snake.Occupies(x, y))
        {
            x++;

            if (x > bounds.max.x)
            {
                x = Mathf.RoundToInt(bounds.min.x);
                y++;

                if (y > bounds.max.y) {
                    y = Mathf.RoundToInt(bounds.min.y);
                }
            }
        }

        transform.position = new Vector2(x, y);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        RandomizePosition();
    }

}