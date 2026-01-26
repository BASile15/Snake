using UnityEngine;
using UnityEngine.InputSystem;

public class Snake : MonoBehaviour
{
    private Vector2 direction = Vector2.right;

    [SerializeField] private float moveDelay = 0.25f;
    private float timer;

    [Header("Sprites de la tête")]
    public Sprite headRight;
    public Sprite headUp;
    public Sprite headLeft;
    public Sprite headDown;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = headRight; // par défaut
    }

    void Update()
    {
        var keyboard = Keyboard.current;

        if (keyboard.wKey.wasPressedThisFrame && direction != Vector2.down)
        {
            direction = Vector2.up;
            spriteRenderer.sprite = headUp;
        }
        else if (keyboard.sKey.wasPressedThisFrame && direction != Vector2.up)
        {
            direction = Vector2.down;
            spriteRenderer.sprite = headDown;
        }
        else if (keyboard.aKey.wasPressedThisFrame && direction != Vector2.right)
        {
            direction = Vector2.left;
            spriteRenderer.sprite = headLeft;
        }
        else if (keyboard.dKey.wasPressedThisFrame && direction != Vector2.left)
        {
            direction = Vector2.right;
            spriteRenderer.sprite = headRight;
        }
    }

    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;

        if (timer >= moveDelay)
        {
            transform.position = new Vector3(
                Mathf.Round(transform.position.x) + direction.x,
                Mathf.Round(transform.position.y) + direction.y,
                0f
            );

            timer = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            UnityEngine.Debug.Log("GAME OVER");
            gameObject.SetActive(false);
            Time.timeScale = 0f;
        }
    }
}
