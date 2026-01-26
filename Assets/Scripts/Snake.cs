using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Snake : MonoBehaviour
{
    private Vector2 direction = Vector2.right;

    [SerializeField] private float moveDelay = 0.25f;
    private float timer;

    [Header("Sprites tête")]
    public Sprite headRight;
    public Sprite headUp;
    public Sprite headLeft;
    public Sprite headDown;

    [Header("Sprites corps")]
    public Sprite bodyHorizontal;
    public Sprite bodyVertical;

    [Header("Sprites queue")]
    public Sprite tailUp;
    public Sprite tailDown;
    public Sprite tailLeft;
    public Sprite tailRight;

    [Header("Prefab")]
    public GameObject segmentPrefab;

    private SpriteRenderer headRenderer;
    private List<Transform> segments = new List<Transform>();

    void Start()
    {
        headRenderer = GetComponent<SpriteRenderer>();

        InitSnake();
    }

    void InitSnake()
    {
        segments.Clear();
        segments.Add(transform); // tête

        headRenderer.sprite = headRight;

        Vector3 startPos = transform.position;

        // Corps 1
        segments.Add(CreateSegment(startPos + Vector3.left, bodyHorizontal));

        // Corps 2
        segments.Add(CreateSegment(startPos + Vector3.left * 2, bodyHorizontal));

        // Queue
        segments.Add(CreateSegment(startPos + Vector3.left * 3, tailLeft));
    }

    Transform CreateSegment(Vector3 position, Sprite sprite)
    {
        GameObject segment = Instantiate(segmentPrefab, position, Quaternion.identity);
        segment.GetComponent<SpriteRenderer>().sprite = sprite;
        return segment.transform;
    }

    void Update()
    {
        var keyboard = Keyboard.current;

        if (keyboard.wKey.wasPressedThisFrame && direction != Vector2.down)
        {
            direction = Vector2.up;
            headRenderer.sprite = headUp;
        }
        else if (keyboard.sKey.wasPressedThisFrame && direction != Vector2.up)
        {
            direction = Vector2.down;
            headRenderer.sprite = headDown;
        }
        else if (keyboard.aKey.wasPressedThisFrame && direction != Vector2.right)
        {
            direction = Vector2.left;
            headRenderer.sprite = headLeft;
        }
        else if (keyboard.dKey.wasPressedThisFrame && direction != Vector2.left)
        {
            direction = Vector2.right;
            headRenderer.sprite = headRight;
        }
    }

    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;

        if (timer >= moveDelay)
        {
            Move();
            timer = 0f;
        }
    }

    void Move()
    {
        for (int i = segments.Count - 1; i > 0; i--)
        {
            segments[i].position = segments[i - 1].position;
        }

        transform.position = new Vector3(
            Mathf.Round(transform.position.x) + direction.x,
            Mathf.Round(transform.position.y) + direction.y,
            0f
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            UnityEngine.Debug.Log("GAME OVER 💀");
            gameObject.SetActive(false);
            Time.timeScale = 0f;
        }
    }
}
