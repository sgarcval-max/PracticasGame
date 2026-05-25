using UnityEngine;

public class AquariumFish : MonoBehaviour
{
    public FishType fishType;
    public float moveSpeed = 1.5f;
    public float changeDirectionTime = 2f;

    private Vector2 minBounds;
    private Vector2 maxBounds;

    private Vector2 targetPos;
    private float timer = 0f;
    private SpriteRenderer sr;
    private bool isHovered = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Setup(FishType type, Vector2 min, Vector2 max)
    {
        fishType = type;
        minBounds = min;
        maxBounds = max;

        sr.color = FishData.GetColor(type);

        transform.position = new Vector3(
            Random.Range(minBounds.x, maxBounds.x),
            Random.Range(minBounds.y, maxBounds.y),
            0f
        );

        PickNewTarget();
    }

    void Update()
    {
        if (isHovered) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );

        if (targetPos.x < transform.position.x)
            sr.flipX = true;
        else
            sr.flipX = false;

        timer -= Time.deltaTime;
        if (timer <= 0f || Vector2.Distance(transform.position, targetPos) < 0.1f)
        {
            PickNewTarget();
        }
    }

    void PickNewTarget()
    {
        targetPos = new Vector2(
            Random.Range(minBounds.x, maxBounds.x),
            Random.Range(minBounds.y, maxBounds.y)
        );
        timer = changeDirectionTime;
    }

    public void OnHoverEnter()
    {
        isHovered = true;
        sr.color = Color.white;
    }

    public void OnHoverExit()
    {
        isHovered = false;
        sr.color = FishData.GetColor(fishType);
    }
}