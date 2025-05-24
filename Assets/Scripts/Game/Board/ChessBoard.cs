namespace Assets.Scripts.Game.Board
{
    using System.Collections.Generic;
    using Assets.Scripts.Game.AI;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class ChessBoard : MonoBehaviour
    {
        // TODO: fix bug with overlapping ai icons
        // TODO: add custom piece with custom movement rules
        // TODO: add function to receive new piece when pawn reaches the end (maybe with custom piece)
        // TODO: add other chess functions (castling, en passant, promotion, check, checkmate, stalemate)
        // TODO: buffs (undo action(zaa wardoo), multiple tile destroy, more reach, player second chance, multi-life chess pieces, freeze opponent piece, "invisible" pieces, clone pieces, more gold, more time)
        // TODO: consumables (one-time use buffs, one-time use pieces, chess pieces management (destroy pieces, clone))
        // TODO: add shop (buffs, custom pieces, consumables)

        // Board dimensions and scaling
        public int Width = 8;
        public int Height = 8;
        public float TileSize = 1.0f;
        public bool AutoScale = true;
        public float ScalePadding = 0.05f;
        private RectTransform parentRectTransform;
        private float calculatedTileSize;

        // Sprites and materials
        public SpriteHolder SpriteHolder;
        public Sprite WhiteTileSprite;
        public Sprite BlackTileSprite;
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
        public Material PieceMaterial;

        public Dictionary<string, ChessTile> Tiles = new Dictionary<string, ChessTile>();
        private ChessPiece selectedPiece;
        private List<ChessTile> highlightedTiles = new List<ChessTile>();

        // Game variables
        public bool IsWhiteTurn = true;
        public bool GameOver;
        public bool IsAITurn;
        private List<ChessMoveHistory> moveHistory = new List<ChessMoveHistory>();

        // Start is called before the first frame update
        public void Start()
        {
            this.CalculateScaling();
            this.GenerateBoard();
            this.SetupTraditionalPieces();
        }

        // Update is called once per frame
        public void Update()
        {
            // Block input during AI turn
            if (this.IsAITurn || this.GameOver)
            {
                return;
            }

            this.HandleInput();
        }

        public void GenerateBoard()
        {
            // Clear existing tiles if board is regenerated
            foreach (var tile in this.Tiles.Values)
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

            this.Tiles.Clear();

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
                    GameObject tileObject = new GameObject(
                        $"Tile_{GetCoordinateFromPosition(x, y)}"
                    );
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
                    tile.SpriteRenderer.sprite = isWhite
                        ? this.WhiteTileSprite
                        : this.BlackTileSprite;

                    // Set sorting order to be behind chess pieces
                    tile.SpriteRenderer.sortingOrder = 2;

                    // Set proper size for the tile
                    tileObject.transform.localScale = new Vector3(
                        1 / this.calculatedTileSize,
                        1 / this.calculatedTileSize,
                        -1 // be in front of background but behind chess pieces
                    );

                    // Add to dictionary for easy lookup
                    this.Tiles.Add(tile.Coordinate, tile);
                }
            }
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
        public ChessPiece SpawnPiece(
            ChessPieceType type,
            bool isWhite,
            string coordinate,
            Sprite customSprite = null,
            List<Material> customMaterials = null,
            List<MoveRule> customMoveRules = null
        )
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
            Sprite sprite =
                customSprite != null ? customSprite : this.GetPieceSprite(type, isWhite);

            // Set up the piece materials
            var pieceMaterials = new List<Material> { this.PieceMaterial };
            if (customMaterials != null && customMaterials.Count > 0)
            {
                pieceMaterials.AddRange(customMaterials);
            }

            // Initialize the piece
            piece.Initialize(type, isWhite, sprite, this, pieceMaterials, customMoveRules);

            // Scale to fit within the tile, centered
            float scaleX = this.calculatedTileSize * 0.8f; // Use 80% of tile width
            float scaleY = this.calculatedTileSize * 0.8f; // Use 80% of tile height
            float scale = Mathf.Min(scaleX, scaleY); // Use the smaller scale to maintain aspect ratio

            pieceObject.transform.localScale = new Vector3(scale, scale, -2); // -2 to render in front of tiles

            // Place the piece on the tile
            tile.UpdatePiece(piece);

            return piece;
        }

        // Get all pieces from player
        public List<ChessPiece> GetAllPieces(bool isWhite)
        {
            List<ChessPiece> pieces = new List<ChessPiece>();
            foreach (var tile in this.Tiles.Values)
            {
                if (tile.CurrentPiece != null && tile.CurrentPiece.IsWhite == isWhite)
                {
                    pieces.Add(tile.CurrentPiece);
                }
            }

            return pieces;
        }

        // Helper to get the proper sprite based on piece type and color
        public Sprite GetPieceSprite(ChessPieceType type, bool isWhite)
        {
            if (isWhite)
            {
                return type switch
                {
                    ChessPieceType.Rook => this.WhiteRookSprite,
                    ChessPieceType.Knight => this.WhiteKnightSprite,
                    ChessPieceType.Bishop => this.WhiteBishopSprite,
                    ChessPieceType.Queen => this.WhiteQueenSprite,
                    ChessPieceType.King => this.WhiteKingSprite,
                    _ => this.WhitePawnSprite, // Default
                };
            }
            else
            {
                return type switch
                {
                    ChessPieceType.Rook => this.BlackRookSprite,
                    ChessPieceType.Knight => this.BlackKnightSprite,
                    ChessPieceType.Bishop => this.BlackBishopSprite,
                    ChessPieceType.Queen => this.BlackQueenSprite,
                    ChessPieceType.King => this.BlackKingSprite,
                    _ => this.BlackPawnSprite, // Default
                };
            }
        }

        // Calculates appropriate scaling based on parent container
        public void CalculateScaling()
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
        public void OnRectTransformDimensionsChange()
        {
            if (!this.AutoScale || !this.gameObject.activeInHierarchy)
            {
                return;
            }

            this.Start();
        }

        // Handle player input for selecting and moving pieces
        public void HandleInput()
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
                this.IsWhiteTurn = !this.IsWhiteTurn; // Switch turns after move
            }
            else if (clickedTile.CurrentPiece != null) // If we clicked on a piece
            {
                bool isPlayersPiece = clickedTile.CurrentPiece.IsWhite == this.IsWhiteTurn;
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

        // Move a piece to a new tile
        public void MovePiece(ChessPiece piece, ChessTile targetTile)
        {
            if (piece == null || targetTile == null)
            {
                return;
            }

            // Record the move history before making the move
            string fromCoord = piece.CurrentTile.Coordinate;
            ChessPiece capturedPiece = targetTile.CurrentPiece;
            this.moveHistory.Add(
                new ChessMoveHistory(
                    piece,
                    fromCoord,
                    targetTile.Coordinate,
                    capturedPiece,
                    this.IsWhiteTurn
                )
            );

            // Update tiles current piece (destroy/disable old piece if any)
            targetTile.UpdatePiece(piece);

            // Update AI icons
            if (this.SpriteHolder != null)
            {
                this.SpriteHolder.OnPlayerMove();
            }
        }

        public bool CanUndo()
        {
            return this.moveHistory.Count > 0;
        }

        public void UndoLastMove()
        {
            if (!this.CanUndo())
            {
                return;
            }

            // Get the last move
            var lastMove = this.moveHistory[this.moveHistory.Count - 1];
            this.moveHistory.RemoveAt(this.moveHistory.Count - 1);

            // Get the tiles involved
            ChessTile fromTile = this.GetTile(lastMove.FromCoordinate);
            ChessTile toTile = this.GetTile(lastMove.ToCoordinate);
            ChessPiece pieceToMove = lastMove.MovedPiece;

            // Move the piece back
            fromTile.UpdatePiece(pieceToMove);

            // Restore captured piece if there was one
            if (lastMove.CapturedPiece != null)
            {
                // Reactivate the captured piece
                lastMove.CapturedPiece.gameObject.SetActive(true);
                toTile.UpdatePiece(lastMove.CapturedPiece);
            }
            else
            {
                toTile.UpdatePiece(null);
            }

            lastMove.MovedPiece.gameObject.SetActive(true);

            // Restore the turn
            this.IsWhiteTurn = lastMove.WasWhiteTurn;

            // Clear any highlights
            this.ClearHighlights();
            this.selectedPiece = null;
        }

        public void UndoMoves(int numberOfMoves)
        {
            int movesToUndo = Mathf.Min(numberOfMoves, this.moveHistory.Count);
            for (int i = 0; i < movesToUndo; i++)
            {
                this.UndoLastMove();
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
            if (this.Tiles.TryGetValue(coordinate, out ChessTile tile))
            {
                return tile;
            }

            return null;
        }

        // Highlights all valid moves for a piece
        public void HighlightValidMoves(ChessPiece piece)
        {
            if (piece == null)
            {
                return;
            }

            List<ChessTile> validTiles = piece.GetValidTiles();

            foreach (ChessTile tile in validTiles)
            {
                tile.Highlight(true);
                this.highlightedTiles.Add(tile);
            }
        }

        // Clears all highlighted tiles
        public void ClearHighlights()
        {
            foreach (ChessTile tile in this.highlightedTiles)
            {
                tile.Highlight(false);
            }

            this.highlightedTiles.Clear();
        }

        public static Vector2Int GetPositionFromCoordinate(string coordinate)
        {
            ParseCoordinate(coordinate, out int x, out int y);
            return new Vector2Int(x, y);
        }

        public void SetAITurn(bool aiTurn)
        {
            this.IsAITurn = aiTurn;
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

            string targetCoord = GetCoordinateFromPosition(
                move.TargetPosition.x,
                move.TargetPosition.y
            );
            ChessTile targetTile = this.GetTile(targetCoord);

            if (targetTile != null)
            {
                this.MovePiece(piece, targetTile);
                this.IsWhiteTurn = !this.IsWhiteTurn;
            }
        }

        // Get tile by Vector2Int position (overload for AI use)
        public ChessTile GetTile(Vector2Int position)
        {
            string coordinate = GetCoordinateFromPosition(position.x, position.y);
            return this.GetTile(coordinate);
        }

        // Check if a coordinate is valid on the board
        public bool IsValidCoordinate(string coordinate)
        {
            return this.Tiles.ContainsKey(coordinate);
        }

        // Check if a position is valid on the board
        public bool IsValidPosition(Vector2Int position)
        {
            return position.x >= 0
                && position.x < this.Width
                && position.y >= 0
                && position.y < this.Height;
        }

        // Get all pieces of a specific type and color
        public List<ChessPiece> GetPiecesOfType(ChessPieceType type, bool isWhite)
        {
            List<ChessPiece> pieces = new List<ChessPiece>();
            foreach (var tile in this.Tiles.Values)
            {
                ChessPiece piece = tile.CurrentPiece;
                if (piece != null && piece.PieceType == type && piece.IsWhite == isWhite)
                {
                    pieces.Add(piece);
                }
            }

            return pieces;
        }

        // Check if the king of a specific color is in check
        public bool IsInCheck(bool isWhiteKing)
        {
            // Find the king
            List<ChessPiece> kings = this.GetPiecesOfType(ChessPieceType.King, isWhiteKing);
            if (kings.Count == 0)
            {
                return false; // No king found
            }

            ChessPiece king = kings[0];
            Vector2Int kingPosition = GetPositionFromCoordinate(king.CurrentTile.Coordinate);

            // Check if any enemy piece can attack the king's position
            List<ChessPiece> enemyPieces = this.GetAllPieces(!isWhiteKing);
            foreach (var enemyPiece in enemyPieces)
            {
                var validMoves = enemyPiece.GetValidMoves();
                foreach (var move in validMoves)
                {
                    if (move.TargetPosition == kingPosition)
                    {
                        return true; // King is in check
                    }
                }
            }

            return false;
        }

        // Evaluate if the current position is checkmate or stalemate
        public GameState GetGameState(bool isWhiteTurn)
        {
            List<ChessPiece> currentPlayerPieces = this.GetAllPieces(isWhiteTurn);
            bool hasValidMoves = false;

            // Check if the current player has any valid moves
            foreach (var piece in currentPlayerPieces)
            {
                var validMoves = piece.GetValidMoves();
                if (validMoves.Count > 0)
                {
                    hasValidMoves = true;
                    break;
                }
            }

            if (!hasValidMoves)
            {
                // No valid moves available
                if (this.IsInCheck(isWhiteTurn))
                {
                    return isWhiteTurn ? GameState.BlackWins : GameState.WhiteWins; // Checkmate
                }
                else
                {
                    return GameState.Stalemate; // Stalemate
                }
            }

            return GameState.Ongoing; // Game continues
        }
    }
}
