using UnityEngine;

public class PlayerColliderAdjuster : MonoBehaviour
{
    private PolygonCollider2D col;
    private SpriteRenderer sr;
    private Sprite lastSprite;
    private bool lastFlipX;

    void Awake()
    {
        col = GetComponent<PolygonCollider2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (col == null || sr == null || sr.sprite == null) return;

        if (sr.sprite != lastSprite || sr.flipX != lastFlipX)
        {
            lastSprite = sr.sprite;
            lastFlipX = sr.flipX;
            UpdateColliderShape();
        }
    }

    void UpdateColliderShape()
    {
        Sprite sprite = sr.sprite;
        int shapeCount = sprite.GetPhysicsShapeCount();

        col.pathCount = shapeCount;

        var points = new System.Collections.Generic.List<Vector2>();

        for (int i = 0; i < shapeCount; i++)
        {
            points.Clear();
            sprite.GetPhysicsShape(i, points);

            // Si está volteado invertimos la X de cada punto
            if (sr.flipX)
            {
                for (int j = 0; j < points.Count; j++)
                    points[j] = new Vector2(-points[j].x, points[j].y);
            }

            col.SetPath(i, points.ToArray());
        }
    }
}
