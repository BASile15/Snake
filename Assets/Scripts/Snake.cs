using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Snake : MonoBehaviour
{
    [Header("Grille")]
    public Collider2D gridArea;
    public int gridWidth = 17;
    public int gridHeight = 15;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highscoreText;
    public TextMeshProUGUI leaderboardDisplay;
    
    private int score;
    private int highscore;

    private Vector2Int direction = Vector2Int.right;
    private Vector2Int headGridPos;

    [SerializeField] private float moveDelay = 0.25f;
    private float timer;

    [Header("Sprites tête")]
    public Sprite headRight; public Sprite headUp; public Sprite headLeft; public Sprite headDown;

    [Header("Sprites corps")]
    public Sprite bodyHorizontal; public Sprite bodyVertical; public Sprite bodyTopLeft;
    public Sprite bodyTopRight; public Sprite bodyBottomLeft; public Sprite bodyBottomRight;

    [Header("Sprites queue")]
    public Sprite tailUp; public Sprite tailDown; public Sprite tailLeft; public Sprite tailRight;

    [Header("Prefab")]
    public GameObject segmentPrefab;

    private SpriteRenderer headRenderer;
    private List<Transform> segments = new List<Transform>();
    private List<Vector2Int> gridPositions = new List<Vector2Int>();

    void Start()
    {
        headRenderer = GetComponent<SpriteRenderer>();
        headRenderer.sortingOrder = 10;
        score = 0;
        
        // On charge les données via le SaveManager
        SaveData data = SaveManager.LoadData();
        highscore = data.highscore;

        UpdateScoreUI();
        UpdateLeaderboardUI();
        InitSnake();
    }

    void InitSnake()
    {
        segments.Clear();
        gridPositions.Clear();
        headGridPos = new Vector2Int(gridWidth / 2, gridHeight / 2);
        gridPositions.Add(headGridPos);
        segments.Add(transform);
        transform.position = GridToWorld(headGridPos);
        UpdateHeadSprite();

        int initialBodyLength = 1;
        for (int i = 1; i <= initialBodyLength + 1; i++)
        {
            Vector2Int pos = headGridPos - direction * i;
            gridPositions.Add(pos);
            Sprite sprite = (i == initialBodyLength + 1) ? GetTailSprite() : (direction.x != 0 ? bodyHorizontal : bodyVertical);
            Transform segment = CreateSegment(GridToWorld(pos), sprite);
            segment.GetComponent<SpriteRenderer>().sortingOrder = 9;
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
        if (keyboard == null) return;

        if (keyboard.wKey.wasPressedThisFrame && direction != Vector2Int.down) direction = Vector2Int.up;
        else if (keyboard.sKey.wasPressedThisFrame && direction != Vector2Int.up) direction = Vector2Int.down;
        else if (keyboard.aKey.wasPressedThisFrame && direction != Vector2Int.right) direction = Vector2Int.left;
        else if (keyboard.dKey.wasPressedThisFrame && direction != Vector2Int.left) direction = Vector2Int.right;
        
        UpdateHeadSprite();
    }

    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if (timer >= moveDelay) { Move(); timer = 0f; }
    }

    void Move()
    {
        Vector2Int newHeadPos = headGridPos + direction;
        if (newHeadPos.x < 0 || newHeadPos.x >= gridWidth || newHeadPos.y < 0 || newHeadPos.y >= gridHeight) { Die("mur"); return; }
        if (gridPositions.Contains(newHeadPos)) { Die("soi-même"); return; }

        gridPositions.Insert(0, newHeadPos);
        gridPositions.RemoveAt(gridPositions.Count - 1);
        headGridPos = newHeadPos;

        for (int i = 0; i < segments.Count; i++) segments[i].position = GridToWorld(gridPositions[i]);
        UpdateBodySprites();
    }

    Vector3 GridToWorld(Vector2Int gridPos)
    {
        Bounds bounds = gridArea.bounds;
        float cellWidth = bounds.size.x / gridWidth;
        float cellHeight = bounds.size.y / gridHeight;
        float x = bounds.min.x + cellWidth * (gridPos.x + 0.5f);
        float y = bounds.min.y + cellHeight * (gridPos.y + 0.5f);
        return new Vector3(x, y, 0f);
    }

    void UpdateHeadSprite()
    {
        if (direction == Vector2Int.right) headRenderer.sprite = headRight;
        else if (direction == Vector2Int.left) headRenderer.sprite = headLeft;
        else if (direction == Vector2Int.up) headRenderer.sprite = headUp;
        else if (direction == Vector2Int.down) headRenderer.sprite = headDown;
    }

    Sprite GetTailSprite()
    {
        if (direction == Vector2Int.right) return tailLeft;
        if (direction == Vector2Int.left) return tailRight;
        if (direction == Vector2Int.up) return tailDown;
        return tailUp;
    }

    void UpdateBodySprites()
    {
        for (int i = 1; i < segments.Count - 1; i++)
        {
            SpriteRenderer sr = segments[i].GetComponent<SpriteRenderer>();
            Vector2Int prev = gridPositions[i - 1]; Vector2Int curr = gridPositions[i]; Vector2Int next = gridPositions[i + 1];
            Vector2Int dirPrev = curr - prev; Vector2Int dirNext = next - curr;

            if (dirPrev.x != 0 && dirNext.x != 0) sr.sprite = bodyHorizontal;
            else if (dirPrev.y != 0 && dirNext.y != 0) sr.sprite = bodyVertical;
            else {
                if (dirPrev.x == 1 && dirNext.y == 1 || dirPrev.y == -1 && dirNext.x == -1) sr.sprite = bodyTopLeft;
                else if (dirPrev.x == -1 && dirNext.y == 1 || dirPrev.y == -1 && dirNext.x == 1) sr.sprite = bodyTopRight;
                else if (dirPrev.x == 1 && dirNext.y == -1 || dirPrev.y == 1 && dirNext.x == -1) sr.sprite = bodyBottomLeft;
                else sr.sprite = bodyBottomRight;
            }
        }
        int last = segments.Count - 1;
        SpriteRenderer tailSR = segments[last].GetComponent<SpriteRenderer>();
        Vector2Int diff = gridPositions[last - 1] - gridPositions[last];
        if (diff.x == 1) tailSR.sprite = tailLeft; else if (diff.x == -1) tailSR.sprite = tailRight;
        else if (diff.y == 1) tailSR.sprite = tailDown; else tailSR.sprite = tailUp;
    }

    public bool Occupies(int x, int y)
    {
        return gridPositions.Contains(new Vector2Int(x, y));
    }

    void Die(string reason)
    {
        Debug.Log("GAME OVER (" + reason + ")");
        
        // On délègue la sauvegarde au manager
        SaveManager.SaveScore(score);
        
        Time.timeScale = 0f;
        UpdateLeaderboardUI();
    }

    private void Grow()
    {
        Vector2Int lastGridPos = gridPositions[gridPositions.Count - 1];
        GameObject newSegment = Instantiate(this.segmentPrefab, GridToWorld(lastGridPos), Quaternion.identity);
        newSegment.GetComponent<SpriteRenderer>().sortingOrder = 9;
        segments.Add(newSegment.transform);
        gridPositions.Add(lastGridPos);
        score += 1;
        UpdateScoreUI();
        UpdateBodySprites();
    }

    private void OnTriggerEnter2D(Collider2D other) { if (other.CompareTag("Food")) Grow(); }

    void UpdateScoreUI() 
    {
        if (scoreText != null) scoreText.text = "Score: " + score;
        if (highscoreText != null) highscoreText.text = "Best: " + highscore;
    }

    public void UpdateLeaderboardUI() {
        if (leaderboardDisplay == null) return;

        SaveData data = SaveManager.LoadData();
        leaderboardDisplay.text = "TOP 10 SCORES\n\n";
        for (int i = 0; i < data.leaderboard.Count; i++) {
            leaderboardDisplay.text += $"{i + 1}. {data.leaderboard[i].scoreValue} pts ({data.leaderboard[i].date})\n";
        }
    }
}