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
    public Sprite bodyTopLeft;
    public Sprite bodyTopRight;
    public Sprite bodyBottomLeft;
    public Sprite bodyBottomRight;

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
        headRenderer.sortingOrder = 10; // tête devant
        InitSnake();
    }

    void InitSnake()
    {
        // Clear segments existants
        segments.Clear();
        segments.Add(transform); // tête
        UpdateHeadSprite();

        Vector3 startPos = transform.position;

        // Crée 2 corps + 1 queue (longueur totale = 4)
        for (int i = 1; i <= 3; i++)
        {
            Vector3 pos = startPos - new Vector3(direction.x * i, direction.y * i, 0f);
            Sprite sprite;

            if (i == 3)
            {
                sprite = GetTailSprite();
            }
            else
            {
                sprite = (Mathf.Abs(direction.x) > 0) ? bodyHorizontal : bodyVertical;
            }

            Transform segment = CreateSegment(pos, sprite);
            segment.GetComponent<SpriteRenderer>().sortingOrder = 9; // corps/queue derrière la tête
            segments.Add(segment);
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
        Vector3 prevPos = segments[0].position;
        Vector3 nextPos;

        // Déplacer la tête
        segments[0].position = new Vector3(
            Mathf.Round(transform.position.x) + direction.x,
            Mathf.Round(transform.position.y) + direction.y,
            0f
        );

        // Déplacer le corps
        for (int i = 1; i < segments.Count; i++)
        {
            nextPos = segments[i].position;
            segments[i].position = prevPos;
            prevPos = nextPos;
        }

        UpdateBodySprites();
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
        if (direction == Vector2.right) return tailLeft;
        else if (direction == Vector2.left) return tailRight;
        else if (direction == Vector2.up) return tailDown;
        else return tailUp;
    }

    void UpdateBodySprites()
    {
        for (int i = 1; i < segments.Count - 1; i++)
        {
            SpriteRenderer sr = segments[i].GetComponent<SpriteRenderer>();
            Vector3 prev = segments[i - 1].position;
            Vector3 next = segments[i + 1].position;

            Vector3 dirPrev = (segments[i].position - prev).normalized;
            Vector3 dirNext = (next - segments[i].position).normalized;

            // Segment droit
            if ((dirPrev.x != 0 && dirNext.x != 0)) sr.sprite = bodyHorizontal;
            else if ((dirPrev.y != 0 && dirNext.y != 0)) sr.sprite = bodyVertical;
            else
            {
                // Coins personnalisés selon la position de la queue et direction
                if (dirPrev.x > 0 && dirNext.y > 0) sr.sprite = bodyTopLeft;     // droite → haut
                else if (dirPrev.x < 0 && dirNext.y > 0) sr.sprite = bodyTopRight; // gauche → haut
                else if (dirPrev.x > 0 && dirNext.y < 0) sr.sprite = bodyBottomLeft; // droite → bas
                else if (dirPrev.x < 0 && dirNext.y < 0) sr.sprite = bodyBottomRight; // gauche → bas
                else if (dirPrev.y > 0 && dirNext.x > 0) sr.sprite = bodyTopRight; // haut → droite
                else if (dirPrev.y > 0 && dirNext.x < 0) sr.sprite = bodyTopLeft;  // haut → gauche
                else if (dirPrev.y < 0 && dirNext.x > 0) sr.sprite = bodyBottomRight; // bas → droite
                else if (dirPrev.y < 0 && dirNext.x < 0) sr.sprite = bodyBottomLeft;  // bas → gauche
            }
        }

        // Queue
        int last = segments.Count - 1;
        SpriteRenderer tailSR = segments[last].GetComponent<SpriteRenderer>();
        Vector3 diff = segments[last - 1].position - segments[last].position;
        if (diff.x > 0) tailSR.sprite = tailLeft;
        else if (diff.x < 0) tailSR.sprite = tailRight;
        else if (diff.y > 0) tailSR.sprite = tailDown;
        else tailSR.sprite = tailUp;
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
