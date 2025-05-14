using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChessBoard : MonoBehaviour
{
    public int width = 8;
    public int height = 8;

    [Tooltip("If auto-scale is enabled, this value will be ignored")]
    public float tileSize = 1.0f;
    public bool autoScale = true;

    [Tooltip("Padding percentage (0-1) when auto-scaling")]
    [Range(0, 0.5f)]
    public float scalePadding = 0.05f;

    public Sprite whiteTileSprite;
    public Sprite blackTileSprite;

    [Header("Piece Sprites")]
    public Sprite whitePawnSprite;
    public Sprite whiteRookSprite;
    public Sprite whiteKnightSprite;
    public Sprite whiteBishopSprite;
    public Sprite whiteQueenSprite;
    public Sprite whiteKingSprite;
    public Sprite blackPawnSprite;
    public Sprite blackRookSprite;
    public Sprite blackKnightSprite;
    public Sprite blackBishopSprite;
    public Sprite blackQueenSprite;
    public Sprite blackKingSprite;

    private Dictionary<string, ChessTile> tiles = new Dictionary<string, ChessTile>();
    private ChessPiece selectedPiece;
    private List<ChessTile> highlightedTiles = new List<ChessTile>();
    private RectTransform parentRectTransform;
    private float calculatedTileSize;

    // Start is called before the first frame update
    void Start()
    {
        CalculateScaling();
        GenerateBoard();
        SetupTraditionalPieces();
    }

    // Calculates appropriate scaling based on parent container
    private void CalculateScaling()
    {
        if (!autoScale)
        {
            calculatedTileSize = tileSize;
            return;
        }

        // Get parent RectTransform if we're in a Canvas
        parentRectTransform = GetComponentInParent<RectTransform>();

        if (parentRectTransform != null)
        {
            // We're in UI Canvas context
            float availableWidth = parentRectTransform.rect.width * (1 - scalePadding * 2);
            float availableHeight = parentRectTransform.rect.height * (1 - scalePadding * 2);

            // Calculate tile size based on available space and board dimensions
            float widthBasedSize = availableWidth / width;
            float heightBasedSize = availableHeight / height;
            calculatedTileSize = Mathf.Min(widthBasedSize, heightBasedSize);

            Debug.Log($"Auto-scaling in UI context. Tile size: {calculatedTileSize}");
        }
        else
        {
            // We're in world space context
            // Try to get any Renderer to determine parent bounds
            Renderer parentRenderer = GetComponentInParent<Renderer>();
            if (parentRenderer != null)
            {
                // Use parent bounds to calculate available space
                Bounds bounds = parentRenderer.bounds;
                float availableWidth = bounds.size.x * (1 - scalePadding * 2);
                float availableHeight = bounds.size.y * (1 - scalePadding * 2);

                // Calculate tile size
                float widthBasedSize = availableWidth / width;
                float heightBasedSize = availableHeight / height;
                calculatedTileSize = Mathf.Min(widthBasedSize, heightBasedSize);

                Debug.Log(
                    $"Auto-scaling based on parent Renderer. Tile size: {calculatedTileSize}"
                );
            }
            else
            {
                // No parent to base size on, use Transform size if it's not zero
                Vector3 localScale = transform.localScale;
                if (localScale.x > 0 && localScale.y > 0)
                {
                    float availableWidth = localScale.x * (1 - scalePadding * 2);
                    float availableHeight = localScale.y * (1 - scalePadding * 2);

                    float widthBasedSize = availableWidth / width;
                    float heightBasedSize = availableHeight / height;
                    calculatedTileSize = Mathf.Min(widthBasedSize, heightBasedSize);

                    Debug.Log(
                        $"Auto-scaling based on transform scale. Tile size: {calculatedTileSize}"
                    );
                }
                else
                {
                    // No suitable parent size found, use default tile size
                    calculatedTileSize = tileSize;
                    Debug.LogWarning(
                        "Could not determine parent size for auto-scaling. Using default tile size."
                    );
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    // Then replace the HandleInput method with this:
    void HandleInput()
    {
        // Check for left mouse button click
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

            if (hit.collider != null)
            {
                // Check if we hit a tile
                ChessTile clickedTile = hit.collider.GetComponent<ChessTile>();
                if (clickedTile != null)
                {
                    // If we have a piece selected and clicked on a valid move tile
                    if (selectedPiece != null && highlightedTiles.Contains(clickedTile))
                    {
                        // Move the piece
                        MovePiece(selectedPiece, clickedTile);
                        ClearHighlights();
                        selectedPiece = null;
                    }
                    // If we clicked on a piece
                    else if (clickedTile.currentPiece != null)
                    {
                        // Deselect current piece
                        if (selectedPiece != null)
                        {
                            ClearHighlights();
                        }

                        // Select new piece
                        selectedPiece = clickedTile.currentPiece;
                        HighlightValidMoves(selectedPiece);
                    }
                    // If we clicked on an empty tile
                    else
                    {
                        ClearHighlights();
                        selectedPiece = null;
                    }
                }
            }
        }
    }

    public void GenerateBoard()
    {
        // Clear existing tiles if board is regenerated
        foreach (var tile in tiles.Values)
        {
            if (tile != null)
                DestroyImmediate(tile.gameObject);
        }
        tiles.Clear();

        // Calculate board center offset for alignment
        float boardWidth = width * calculatedTileSize;
        float boardHeight = height * calculatedTileSize;
        Vector2 boardCenter = new Vector2(boardWidth / 2, boardHeight / 2);
        Vector2 startPos = new Vector2(
            -boardCenter.x + (calculatedTileSize / 2),
            -boardCenter.y + (calculatedTileSize / 2)
        );

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Create chess tile game object
                GameObject tileObject = new GameObject($"Tile_{GetCoordinateFromPosition(x, y)}");
                tileObject.transform.parent = transform;

                // Calculate centered position
                float posX = startPos.x + (x * calculatedTileSize);
                float posY = startPos.y + (y * calculatedTileSize);
                tileObject.transform.localPosition = new Vector3(posX, posY, -1);

                // Setup collider for mouse interaction
                BoxCollider2D collider = tileObject.AddComponent<BoxCollider2D>();
                collider.size = new Vector2(tileSize, tileSize);

                // Setup tile component
                ChessTile tile = tileObject.AddComponent<ChessTile>();

                // // Ensure the SpriteRenderer is added before Initialize is called
                // SpriteRenderer renderer = tileObject.AddComponent<SpriteRenderer>();
                // tile.spriteRenderer = renderer;

                bool isWhite = (x + y) % 2 == 0;
                tile.Initialize(GetCoordinateFromPosition(x, y), isWhite);

                // Set sprite
                tile.spriteRenderer.sprite = isWhite ? whiteTileSprite : blackTileSprite;

                // IMPORTANT: Has to be lower than chess piece sorting order, or it will overlap it
                tile.spriteRenderer.sortingOrder = 2;

                // Set proper size for the tile
                tileObject.transform.localScale = new Vector3(
                    calculatedTileSize,
                    calculatedTileSize,
                    -1
                );
                // Add to dictionary for easy lookup
                tiles.Add(tile.coordinate, tile);
            }
        }
    }

    // Utility method to convert from grid position to chess notation
    public string GetCoordinateFromPosition(int x, int y)
    {
        char file = (char)('a' + x);
        int rank = y + 1;
        return $"{file}{rank}";
    }

    // Utility method to parse chess notation into grid position
    public void ParseCoordinate(string coordinate, out int x, out int y)
    {
        if (string.IsNullOrEmpty(coordinate) || coordinate.Length < 2)
        {
            x = -1;
            y = -1;
            return;
        }

        char file = coordinate[0];
        x = file - 'a';

        string rankStr = coordinate.Substring(1);
        if (int.TryParse(rankStr, out int rank))
        {
            y = rank - 1;
        }
        else
        {
            y = -1;
        }
    }

    // Get a tile by its coordinate
    public ChessTile GetTile(string coordinate)
    {
        if (tiles.TryGetValue(coordinate, out ChessTile tile))
        {
            return tile;
        }
        return null;
    }

    // Set up traditional chess piece layout
    public void SetupTraditionalPieces()
    {
        // Make sure we have a standard board size
        if (width != 8 || height != 8)
        {
            Debug.LogWarning("Standard chess setup requires an 8x8 board.");
            return;
        }

        // Setup white pieces
        SpawnPiece(ChessPieceType.Rook, true, "a1");
        SpawnPiece(ChessPieceType.Knight, true, "b1");
        SpawnPiece(ChessPieceType.Bishop, true, "c1");
        SpawnPiece(ChessPieceType.Queen, true, "d1");
        SpawnPiece(ChessPieceType.King, true, "e1");
        SpawnPiece(ChessPieceType.Bishop, true, "f1");
        SpawnPiece(ChessPieceType.Knight, true, "g1");
        SpawnPiece(ChessPieceType.Rook, true, "h1");

        // Setup white pawns
        for (int i = 0; i < width; i++)
        {
            string coord = GetCoordinateFromPosition(i, 1);
            SpawnPiece(ChessPieceType.Pawn, true, coord);
        }

        // Setup black pieces
        SpawnPiece(ChessPieceType.Rook, false, "a8");
        SpawnPiece(ChessPieceType.Knight, false, "b8");
        SpawnPiece(ChessPieceType.Bishop, false, "c8");
        SpawnPiece(ChessPieceType.Queen, false, "d8");
        SpawnPiece(ChessPieceType.King, false, "e8");
        SpawnPiece(ChessPieceType.Bishop, false, "f8");
        SpawnPiece(ChessPieceType.Knight, false, "g8");
        SpawnPiece(ChessPieceType.Rook, false, "h8");

        // Setup black pawns
        for (int i = 0; i < width; i++)
        {
            string coord = GetCoordinateFromPosition(i, 6);
            SpawnPiece(ChessPieceType.Pawn, false, coord);
        }
    }

    // Spawn a chess piece on the board
    public ChessPiece SpawnPiece(ChessPieceType type, bool isWhite, string coordinate)
    {
        ChessTile tile = GetTile(coordinate);
        if (tile == null)
        {
            Debug.LogError($"Cannot spawn piece: tile {coordinate} not found");
            return null;
        }

        GameObject pieceObject = new GameObject($"{(isWhite ? "White" : "Black")}_{type}");
        pieceObject.transform.parent = transform;

        ChessPiece piece = pieceObject.AddComponent<ChessPiece>();

        // Setup sprite renderer
        // SpriteRenderer renderer = pieceObject.GetComponent<SpriteRenderer>();
        // // IMPORTANT: Set a positive sorting order to ensure pieces are on top of tiles
        // renderer.sortingOrder = 10;

        // Get the appropriate sprite based on piece type and color
        Sprite sprite = GetPieceSprite(type, isWhite);

        // Initialize the piece
        piece.Initialize(type, isWhite, sprite, this);

        // // Scale the piece properly to fit the tile
        // // Assuming the sprite has a pixel size of 54x107
        float spriteWidth = 54f;
        float spriteHeight = 107f;

        // Scale to fit within the tile, centered
        float scaleX = calculatedTileSize * 0.8f; // Use 80% of tile width
        float scaleY = calculatedTileSize * 0.8f; // Use 80% of tile height
        float scale = Mathf.Min(scaleX, scaleY); // Use the smaller scale to maintain aspect ratio

        pieceObject.transform.localScale = new Vector3(scale, scale, -2);

        // Place the piece on the tile
        tile.PlacePiece(piece);

        return piece;
    }

    // Helper to get the proper sprite based on piece type and color
    private Sprite GetPieceSprite(ChessPieceType type, bool isWhite)
    {
        if (isWhite)
        {
            switch (type)
            {
                case ChessPieceType.Pawn:
                    return whitePawnSprite;
                case ChessPieceType.Rook:
                    return whiteRookSprite;
                case ChessPieceType.Knight:
                    return whiteKnightSprite;
                case ChessPieceType.Bishop:
                    return whiteBishopSprite;
                case ChessPieceType.Queen:
                    return whiteQueenSprite;
                case ChessPieceType.King:
                    return whiteKingSprite;
                default:
                    return whitePawnSprite; // Default
            }
        }
        else
        {
            switch (type)
            {
                case ChessPieceType.Pawn:
                    return blackPawnSprite;
                case ChessPieceType.Rook:
                    return blackRookSprite;
                case ChessPieceType.Knight:
                    return blackKnightSprite;
                case ChessPieceType.Bishop:
                    return blackBishopSprite;
                case ChessPieceType.Queen:
                    return blackQueenSprite;
                case ChessPieceType.King:
                    return blackKingSprite;
                default:
                    return blackPawnSprite; // Default
            }
        }
    }

    // Highlights all valid moves for a piece
    private void HighlightValidMoves(ChessPiece piece)
    {
        List<ChessTile> validMoves = piece.GetValidMoves();

        foreach (ChessTile tile in validMoves)
        {
            tile.Highlight(true);
            highlightedTiles.Add(tile);
        }
    }

    // Clears all highlighted tiles
    private void ClearHighlights()
    {
        foreach (ChessTile tile in highlightedTiles)
        {
            tile.Highlight(false);
        }
        highlightedTiles.Clear();
    }

    // Move a piece to a new tile
    public void MovePiece(ChessPiece piece, ChessTile targetTile)
    {
        if (piece == null || targetTile == null)
            return;

        // If there's a piece on the target tile (capture)
        if (targetTile.currentPiece != null)
        {
            Destroy(targetTile.currentPiece.gameObject);
        }

        // Remove piece from current tile
        if (piece.currentTile != null)
        {
            piece.currentTile.RemovePiece();
        }

        // Place piece on new tile and ensure Z position
        targetTile.PlacePiece(piece);

        // Ensure piece is in front of tile
        piece.transform.position = new Vector3(
            targetTile.transform.position.x,
            targetTile.transform.position.y,
            -2
        ); // Z = 0 for pieces
    }

    // Resize the board to a custom size
    public void ResizeBoard(int newWidth, int newHeight)
    {
        width = newWidth;
        height = newHeight;
        GenerateBoard();
    }

    // Method to create a custom piece with custom movement rules
    public ChessPiece CreateCustomPiece(
        string coordinate,
        bool isWhite,
        Sprite sprite,
        List<MoveRule> moveRules
    )
    {
        ChessPiece piece = SpawnPiece(ChessPieceType.Custom, isWhite, coordinate);
        if (piece != null)
        {
            piece.sprite = sprite;
            piece.GetComponent<SpriteRenderer>().sprite = sprite;
            piece.CustomizeMoveRules(moveRules);
        }
        return piece;
    }

    // handle board resizing
    public void Resize(int newWidth, int newHeight, bool regenerate = true)
    {
        width = newWidth;
        height = newHeight;

        if (autoScale)
        {
            CalculateScaling();
        }

        if (regenerate)
        {
            GenerateBoard();
        }
    }

    // Handler for scaling from UI
    void OnRectTransformDimensionsChange()
    {
        if (autoScale && gameObject.activeInHierarchy)
        {
            CalculateScaling();
            GenerateBoard();
            SetupTraditionalPieces();
        }
    }
}
