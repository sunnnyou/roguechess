using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChessBoard : MonoBehaviour
{
    // TODO: fix bug with destroying own pieces
    // TODO: fix bug with ai moving multiple times
    // TODO: fix bug with overlapping ai icons
    // TODO: fix bug with ai destroying pieces that werent in reach
    // TODO: add check for checkmate/stalemate
    // TODO: add custom piece with custom movement rules
    // TODO: buffs (undo action(zaa wardoo), multiple tile destroy, more reach, player second chance, multi-life chess pieces, freeze opponent piece, "invisible" pieces, clone pieces, more gold, more time)
    // TODO: consumables (one-time use buffs, one-time use pieces, chess pieces management (destroy pieces, clone))
    // TODO: add shop (buffs, custom pieces, consumables)
    public int Width = 8;
    public int Height = 8;

    [Tooltip("If auto-scale is enabled, this value will be ignored")]
    public float TileSize = 1.0f;
    public bool AutoScale = true;

    [Tooltip("Padding percentage (0-1) when auto-scaling")]
    [Range(0, 0.5f)]
    public float ScalePadding = 0.05f;

    [Header("UI References")]
    public SpriteHolder SpriteHolder;
    private bool isAITurn;

    public Sprite WhiteTileSprite;
    public Sprite BlackTileSprite;

    [Header("Piece Sprites")]
    public Sprite WhitePawnSprite;
    public Sprite WhiteRookSprite;
    public Sprite WhiteKnightSprite;
    public Sprite WhiteBishopSprite;
    public Sprite WhiteQueenSprite;
    public Sprite WhiteKingSprite;
    public Sprite BlackPawnSprite;
    public Sprite BlackRookSprite;
    public Sprite BlackKnightSprite;
    public Sprite BlackBishopSprite;
    public Sprite BlackQueenSprite;
    public Sprite BlackKingSprite;

    [Header("Materials")]
    public Material PieceMaterial;

    private Dictionary<string, ChessTile> tiles = new Dictionary<string, ChessTile>();
    private ChessPiece selectedPiece;
    private List<ChessTile> highlightedTiles = new List<ChessTile>();
    private RectTransform parentRectTransform;
    private float calculatedTileSize;

    // Game variables
    private bool isWhiteTurn = true;
    private bool gameOver;

    public bool IsWhiteTurn() => this.isWhiteTurn;

    public bool IsGameOver() => this.gameOver;

    // Start is called before the first frame update
    private void Start()
    {
        this.CalculateScaling();
        this.GenerateBoard();
        this.SetupTraditionalPieces();
    }

    // Calculates appropriate scaling based on parent container
    private void CalculateScaling()
    {
        if (!this.AutoScale)
        {
            this.calculatedTileSize = this.TileSize;
            return;
        }

        // Get parent RectTransform if we're in a Canvas
        this.parentRectTransform = this.GetComponentInParent<RectTransform>();

        if (this.parentRectTransform != null)
        {
            // We're in UI Canvas context
            float availableWidth =
                this.parentRectTransform.rect.width * (1 - (this.ScalePadding * 2));
            float availableHeight =
                this.parentRectTransform.rect.height * (1 - (this.ScalePadding * 2));

            // Calculate tile size based on available space and board dimensions
            float widthBasedSize = availableWidth / this.Width;
            float heightBasedSize = availableHeight / this.Height;
            this.calculatedTileSize = Mathf.Min(widthBasedSize, heightBasedSize);

            Debug.Log($"Auto-scaling in UI context. Tile size: {this.calculatedTileSize}");
        }
        else
        {
            // We're in world space context
            // Try to get any Renderer to determine parent bounds
            Renderer parentRenderer = this.GetComponentInParent<Renderer>();
            if (parentRenderer != null)
            {
                // Use parent bounds to calculate available space
                Bounds bounds = parentRenderer.bounds;
                float availableWidth = bounds.size.x * (1 - (this.ScalePadding * 2));
                float availableHeight = bounds.size.y * (1 - (this.ScalePadding * 2));

                // Calculate tile size
                float widthBasedSize = availableWidth / this.Width;
                float heightBasedSize = availableHeight / this.Height;
                this.calculatedTileSize = Mathf.Min(widthBasedSize, heightBasedSize);

                Debug.Log(
                    $"Auto-scaling based on parent Renderer. Tile size: {this.calculatedTileSize}"
                );
            }
            else
            {
                // No parent to base size on, use Transform size if it's not zero
                Vector3 localScale = this.transform.localScale;
                if (localScale.x > 0 && localScale.y > 0)
                {
                    float availableWidth = localScale.x * (1 - (this.ScalePadding * 2));
                    float availableHeight = localScale.y * (1 - (this.ScalePadding * 2));

                    float widthBasedSize = availableWidth / this.Width;
                    float heightBasedSize = availableHeight / this.Height;
                    this.calculatedTileSize = Mathf.Min(widthBasedSize, heightBasedSize);

                    Debug.Log(
                        $"Auto-scaling based on transform scale. Tile size: {this.calculatedTileSize}"
                    );
                }
                else
                {
                    // No suitable parent size found, use default tile size
                    this.calculatedTileSize = this.TileSize;
                    Debug.LogWarning(
                        "Could not determine parent size for auto-scaling. Using default tile size."
                    );
                }
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
        // Block input during AI turn
        if (this.isAITurn || this.gameOver)
        {
            return;
        }

        this.HandleInput();
    }

    // Handle player input for selecting and moving pieces
    private void HandleInput()
    {
        // Check for left mouse button click
        if (!Mouse.current.leftButton.wasPressedThisFrame)
        {
            return;
        }

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

        // Check if object was clicked
        if (hit.collider == null)
        {
            return;
        }

        // Check if we hit a tile
        if (!hit.collider.TryGetComponent<ChessTile>(out var clickedTile))
        {
            return;
        }

        // If we have a piece selected and clicked on a valid move tile
        if (this.selectedPiece != null && this.highlightedTiles.Contains(clickedTile))
        {
            // Move the piece
            this.MovePiece(this.selectedPiece, clickedTile);
            this.ClearHighlights();
            this.selectedPiece = null;
            this.isWhiteTurn = !this.isWhiteTurn; // Switch turns after move
        }
        else if (clickedTile.CurrentPiece != null) // If we clicked on a piece
        {
            bool isPlayersPiece = clickedTile.CurrentPiece.IsWhite == this.isWhiteTurn;
            if (!isPlayersPiece)
            {
                return; // Can't select opponent's pieces
            }

            // Deselect current piece
            if (this.selectedPiece != null)
            {
                this.ClearHighlights();
            }

            // Select new piece
            this.selectedPiece = clickedTile.CurrentPiece;
            this.HighlightValidMoves(this.selectedPiece);
        }
        else // If we clicked on an empty tile
        {
            this.ClearHighlights();
            this.selectedPiece = null;
        }
    }

    public void GenerateBoard()
    {
        // Clear existing tiles if board is regenerated
        foreach (var tile in this.tiles.Values)
        {
            if (tile == null)
            {
                continue;
            }

            if (Application.isEditor)
            {
                DestroyImmediate(tile.gameObject);
            }
            else
            {
                Destroy(tile.gameObject);
            }
        }

        this.tiles.Clear();

        // Calculate board center offset for alignment
        float boardWidth = this.Width * this.calculatedTileSize;
        float boardHeight = this.Height * this.calculatedTileSize;
        Vector2 boardCenter = new Vector2(boardWidth / 2, boardHeight / 2);
        Vector2 startPos = new Vector2(
            -boardCenter.x + (this.calculatedTileSize / 2),
            -boardCenter.y + (this.calculatedTileSize / 2)
        );

        for (int x = 0; x < this.Width; x++)
        {
            for (int y = 0; y < this.Height; y++)
            {
                // Create chess tile game object
                GameObject tileObject = new GameObject($"Tile_{GetCoordinateFromPosition(x, y)}");
                tileObject.transform.parent = this.transform;

                // Calculate centered position
                float posX = startPos.x + (x * this.calculatedTileSize);
                float posY = startPos.y + (y * this.calculatedTileSize);
                tileObject.transform.localPosition = new Vector3(posX, posY, -1);

                // Setup collider for mouse interaction
                BoxCollider2D collider = tileObject.AddComponent<BoxCollider2D>();
                collider.size = new Vector2(this.TileSize, this.TileSize);

                // Setup tile component
                ChessTile tile = tileObject.AddComponent<ChessTile>();

                bool isWhite = (x + y) % 2 == 0;
                tile.Initialize(GetCoordinateFromPosition(x, y), isWhite);

                // Set sprite
                tile.SpriteRenderer.sprite = isWhite ? this.WhiteTileSprite : this.BlackTileSprite;

                // Set sorting order to be behind chess pieces
                tile.SpriteRenderer.sortingOrder = 2;

                // Set proper size for the tile
                tileObject.transform.localScale = new Vector3(
                    1 / this.calculatedTileSize,
                    1 / this.calculatedTileSize,
                    -1 // be in front of background but behind chess pieces
                );

                // Add to dictionary for easy lookup
                this.tiles.Add(tile.Coordinate, tile);
            }
        }
    }

    // Utility method to convert from grid position to chess notation
    public static string GetCoordinateFromPosition(int x, int y)
    {
        char file = (char)('a' + x);
        int rank = y + 1;
        return $"{file}{rank}";
    }

    // Utility method to parse chess notation into grid position
    public static void ParseCoordinate(string coordinate, out int x, out int y)
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
        if (this.tiles.TryGetValue(coordinate, out ChessTile tile))
        {
            return tile;
        }

        return null;
    }

    // Set up traditional chess piece layout
    public void SetupTraditionalPieces()
    {
        // Make sure we have a standard board size
        if (this.Width != 8 || this.Height != 8)
        {
            Debug.LogWarning("Standard chess setup requires an 8x8 board.");
            return;
        }

        // Setup white pieces
        this.SpawnPiece(ChessPieceType.Rook, true, "a1");
        this.SpawnPiece(ChessPieceType.Knight, true, "b1");
        this.SpawnPiece(ChessPieceType.Bishop, true, "c1");
        this.SpawnPiece(ChessPieceType.Queen, true, "d1");
        this.SpawnPiece(ChessPieceType.King, true, "e1");
        this.SpawnPiece(ChessPieceType.Bishop, true, "f1");
        this.SpawnPiece(ChessPieceType.Knight, true, "g1");
        this.SpawnPiece(ChessPieceType.Rook, true, "h1");

        // Setup white pawns
        for (int i = 0; i < this.Width; i++)
        {
            string coord = GetCoordinateFromPosition(i, 1);
            this.SpawnPiece(ChessPieceType.Pawn, true, coord);
        }

        // Setup black pieces
        this.SpawnPiece(ChessPieceType.Rook, false, "a8");
        this.SpawnPiece(ChessPieceType.Knight, false, "b8");
        this.SpawnPiece(ChessPieceType.Bishop, false, "c8");
        this.SpawnPiece(ChessPieceType.Queen, false, "d8");
        this.SpawnPiece(ChessPieceType.King, false, "e8");
        this.SpawnPiece(ChessPieceType.Bishop, false, "f8");
        this.SpawnPiece(ChessPieceType.Knight, false, "g8");
        this.SpawnPiece(ChessPieceType.Rook, false, "h8");

        // Setup black pawns
        for (int i = 0; i < this.Width; i++)
        {
            string coord = GetCoordinateFromPosition(i, 6);
            this.SpawnPiece(ChessPieceType.Pawn, false, coord);
        }
    }

    // Spawn a chess piece on the board
    public ChessPiece SpawnPiece(ChessPieceType type, bool isWhite, string coordinate)
    {
        ChessTile tile = this.GetTile(coordinate);
        if (tile == null)
        {
            Debug.LogError($"Cannot spawn piece: tile {coordinate} not found");
            return null;
        }

        GameObject pieceObject = new GameObject($"{(isWhite ? "White" : "Black")}_{type}");
        pieceObject.transform.parent = this.transform;

        ChessPiece piece = pieceObject.AddComponent<ChessPiece>();

        // Get the appropriate sprite based on piece type and color
        Sprite sprite = this.GetPieceSprite(type, isWhite);

        // Initialize the piece
        piece.Initialize(type, isWhite, sprite, this, this.PieceMaterial);

        // Scale to fit within the tile, centered
        float scaleX = this.calculatedTileSize * 0.8f; // Use 80% of tile width
        float scaleY = this.calculatedTileSize * 0.8f; // Use 80% of tile height
        float scale = Mathf.Min(scaleX, scaleY); // Use the smaller scale to maintain aspect ratio

        pieceObject.transform.localScale = new Vector3(scale, scale, -2); // -2 to render in front of tiles

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
                    return this.WhitePawnSprite;
                case ChessPieceType.Rook:
                    return this.WhiteRookSprite;
                case ChessPieceType.Knight:
                    return this.WhiteKnightSprite;
                case ChessPieceType.Bishop:
                    return this.WhiteBishopSprite;
                case ChessPieceType.Queen:
                    return this.WhiteQueenSprite;
                case ChessPieceType.King:
                    return this.WhiteKingSprite;
                default:
                    return this.WhitePawnSprite; // Default
            }
        }
        else
        {
            switch (type)
            {
                case ChessPieceType.Pawn:
                    return this.BlackPawnSprite;
                case ChessPieceType.Rook:
                    return this.BlackRookSprite;
                case ChessPieceType.Knight:
                    return this.BlackKnightSprite;
                case ChessPieceType.Bishop:
                    return this.BlackBishopSprite;
                case ChessPieceType.Queen:
                    return this.BlackQueenSprite;
                case ChessPieceType.King:
                    return this.BlackKingSprite;
                default:
                    return this.BlackPawnSprite; // Default
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
            this.highlightedTiles.Add(tile);
        }
    }

    // Clears all highlighted tiles
    private void ClearHighlights()
    {
        foreach (ChessTile tile in this.highlightedTiles)
        {
            tile.Highlight(false);
        }

        this.highlightedTiles.Clear();
    }

    // Move a piece to a new tile
    public void MovePiece(ChessPiece piece, ChessTile targetTile)
    {
        if (piece == null || targetTile == null)
        {
            return;
        }

        // If there's a piece on the target tile (capture)
        if (targetTile.CurrentPiece != null)
        {
            if (Application.isEditor)
            {
                DestroyImmediate(targetTile.CurrentPiece.gameObject);
            }
            else
            {
                Destroy(targetTile.CurrentPiece.gameObject);
            }
        }

        // Remove piece from current tile
        if (piece.CurrentTile != null)
        {
            piece.CurrentTile.RemovePiece();
        }

        // Place piece on new tile and ensure Z position
        targetTile.PlacePiece(piece);

        // Ensure piece is in front of tile
        piece.transform.position = new Vector3(
            targetTile.transform.position.x,
            targetTile.transform.position.y,
            -2 // render in front of tiles
        );

        // Update AI icons
        if (this.SpriteHolder != null)
        {
            this.SpriteHolder.OnPlayerMove();
        }
    }

    // Resize the board to a custom size
    public void ResizeBoard(int newWidth, int newHeight)
    {
        this.Width = newWidth;
        this.Height = newHeight;
        this.GenerateBoard();
    }

    // Create a custom piece with custom movement rules
    public ChessPiece CreateCustomPiece(
        string coordinate,
        bool isWhite,
        Sprite sprite,
        List<MoveRule> moveRules
    )
    {
        ChessPiece piece = this.SpawnPiece(ChessPieceType.Custom, isWhite, coordinate);
        if (piece == null)
        {
            return piece;
        }

        // TODO: special sprite for example mix of white/black piece maybe?
        piece.Sprite = sprite;
        piece.GetComponent<SpriteRenderer>().sprite = sprite;

        // // Apply custom material if available
        // TODO: shining material for special pieces for example
        // if (pieceMaterial != null)
        // {
        //     piece.GetComponent<SpriteRenderer>().material = pieceMaterial;
        // }

        piece.CustomizeMoveRules(moveRules);
        return piece;
    }

    // handle board resizing
    public void Resize(int newWidth, int newHeight, bool regenerate = true)
    {
        this.Width = newWidth;
        this.Height = newHeight;

        if (this.AutoScale)
        {
            this.CalculateScaling();
        }

        if (regenerate)
        {
            this.GenerateBoard();
        }
    }

    // Handler for scaling from UI
    private void OnRectTransformDimensionsChange()
    {
        if (!this.AutoScale || !this.gameObject.activeInHierarchy)
        {
            return;
        }

        this.CalculateScaling();
        this.GenerateBoard();
        this.SetupTraditionalPieces();
    }

    // Get all pieces from player
    public List<ChessPiece> GetAllPieces(bool isWhite)
    {
        List<ChessPiece> pieces = new List<ChessPiece>();
        foreach (var tile in this.tiles.Values)
        {
            if (tile.CurrentPiece != null && tile.CurrentPiece.IsWhite == isWhite)
            {
                pieces.Add(tile.CurrentPiece);
            }
        }

        return pieces;
    }

    public static List<ChessMove> GetValidMoves(ChessPiece piece)
    {
        if (piece == null)
        {
            throw new System.ArgumentNullException(nameof(piece));
        }

        List<ChessMove> moves = new List<ChessMove>();
        List<ChessTile> validTiles = piece.GetValidMoves();

        foreach (var tile in validTiles)
        {
            moves.Add(
                new ChessMove(
                    GetPositionFromCoordinate(piece.CurrentTile.Coordinate),
                    GetPositionFromCoordinate(tile.Coordinate),
                    tile.CurrentPiece != null
                )
            );
        }

        return moves;
    }

    private static Vector2Int GetPositionFromCoordinate(string coordinate)
    {
        ParseCoordinate(coordinate, out int x, out int y);
        return new Vector2Int(x, y);
    }

    public void SetAITurn(bool aiTurn)
    {
        this.isAITurn = aiTurn;
        if (this.SpriteHolder != null)
        {
            this.SpriteHolder.ShowLoading(aiTurn);
        }
    }

    public void MovePieceAI(ChessPiece piece, ChessMove move)
    {
        if (move == null)
        {
            throw new System.ArgumentNullException(nameof(move));
        }

        this.SetAITurn(true);

        string targetCoord = GetCoordinateFromPosition(
            move.TargetPosition.x,
            move.TargetPosition.y
        );
        ChessTile targetTile = this.GetTile(targetCoord);

        if (targetTile != null)
        {
            this.MovePiece(piece, targetTile);
            this.isWhiteTurn = !this.isWhiteTurn;
        }

        this.SetAITurn(false);
    }
}
