using UnityEngine;

// Represents a single chess tile on the board
public class ChessTile : MonoBehaviour
{
    public string coordinate;
    public Color originalColor;
    public bool isWhite;
    public ChessPiece currentPiece;
    public SpriteRenderer spriteRenderer;

    private Color highlightColor = new Color(0.0f, 0.4f, 0.0f); // green

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
    }

    public void Initialize(string coord, bool isWhiteTile)
    {
        coordinate = coord;
        isWhite = isWhiteTile;

        // Make sure we have a SpriteRenderer at this point
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            }
        }
        originalColor = spriteRenderer.color;

        // Render tile behind chess pieces
        spriteRenderer.sortingOrder = 2;
    }

    public void Highlight(bool highlight)
    {
        spriteRenderer.color = highlight ? highlightColor : originalColor;
    }

    public void PlacePiece(ChessPiece piece)
    {
        if (currentPiece != null)
        {
            // If there's already a piece here, remove it
            RemovePiece();
        }

        currentPiece = piece;
        if (piece != null)
        {
            // Ensure Z position is in front of tiles
            piece.transform.position = new Vector3(transform.position.x, transform.position.y, -2);
            piece.currentTile = this;
        }
    }

    public ChessPiece RemovePiece()
    {
        ChessPiece piece = currentPiece;
        currentPiece = null;
        return piece;
    }
}
