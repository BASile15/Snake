using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Snake : MonoBehaviour
{
    private Vector2 direction = Vector2.right; // direction initiale

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
        headRenderer.sortingOrder = 2; // tête toujours devant

        InitSnake();

        // Corps et queue derrière la tête
        for (int i = 1; i < segments.Count; i++)
        {
            SpriteRenderer sr = segments[i].GetComponent<SpriteRenderer>();
            sr.sortingOrder = 1; // corps et queue derrière la tête
        }
    }


    void InitSnake()
    {
        segments.Clear();
        segments.Add(transform); // tête

        // Définir le sprite de la tête selon direction initiale
        UpdateHeadSprite();

        Vector3 startPos = transform.position;

        // Création des segments du corps et queue dynamiquement selon direction
        for (int i = 1; i <= 3; i++)
        {
            Vector3 pos = startPos - new Vector3(direction.x * i, direction.y * i, 0f);
            Sprite sprite;

            // 3ème segment = queue
            if (i == 3)
            {
                sprite = GetTailSprite();
            }
            else
            {
                // Corps : horizontal si X != 0 sinon vertical
                sprite = (Mathf.Abs(direction.x) > 0) ? bodyHorizontal : bodyVertical;
            }

            segments.Add(CreateSegment(pos, sprite));
        }
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
            UpdateHeadSprite();
        }
        else if (keyboard.sKey.wasPressedThisFrame && direction != Vector2.up)
        {
            direction = Vector2.down;
            UpdateHeadSprite();
        }
        else if (keyboard.aKey.wasPressedThisFrame && direction != Vector2.right)
        {
            direction = Vector2.left;
            UpdateHeadSprite();
        }
        else if (keyboard.dKey.wasPressedThisFrame && direction != Vector2.left)
        {
            direction = Vector2.right;
            UpdateHeadSprite();
        }
    }

    void UpdateHeadSprite()
    {
        if (direction == Vector2.right) headRenderer.sprite = headRight;
        else if (direction == Vector2.left) headRenderer.sprite = headLeft;
        else if (direction == Vector2.up) headRenderer.sprite = headUp;
        else if (direction == Vector2.down) headRenderer.sprite = headDown;
    }

    Sprite GetTailSprite()
    {
        if (direction == Vector2.right) return tailLeft;  // queue regarde vers la tête
        else if (direction == Vector2.left) return tailRight;
        else if (direction == Vector2.up) return tailDown;
        else return tailUp;
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
        // Stocke la position actuelle des segments
        Vector3 prevPos = segments[0].position;
        Vector3 nextPos;

        // Déplacement de la tête
        segments[0].position = new Vector3(
            Mathf.Round(transform.position.x) + direction.x,
            Mathf.Round(transform.position.y) + direction.y,
            0f
        );

        // Déplacement du corps
        for (int i = 1; i < segments.Count; i++)
        {
            nextPos = segments[i].position;
            segments[i].position = prevPos;

            // Gestion sprite du corps
            SpriteRenderer sr = segments[i].GetComponent<SpriteRenderer>();

            if (i == segments.Count - 1)
            {
                // Queue
                Vector3 diff = segments[i - 1].position - segments[i].position;
                if (diff.x > 0) sr.sprite = tailLeft;
                else if (diff.x < 0) sr.sprite = tailRight;
                else if (diff.y > 0) sr.sprite = tailDown;
                else sr.sprite = tailUp;
            }
            else
            {
                // Corps
                Vector3 diff = segments[i + 1].position - segments[i].position;
                if (Mathf.Abs(diff.x) > 0) sr.sprite = bodyHorizontal;
                else sr.sprite = bodyVertical;
            }

            prevPos = nextPos;
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
