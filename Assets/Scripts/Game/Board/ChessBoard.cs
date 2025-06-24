namespace Assets.Scripts.Game.Board
{
    using System.Collections.Generic;
    using Assets.Scripts.Game.Buffs.Pieces.Update;
    using Assets.Scripts.Game.Enemy;
    using Assets.Scripts.Game.MoveRules;
    using Assets.Scripts.Game.Player;
    using Assets.Scripts.UI;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class ChessBoard : BaseManager
    {
        // TODO: add tile positions to border of chessboard
        // TODO: add custom piece with custom movement rules
        // TODO: add other chess functions (castling, check, checkmate, stalemate)
        // TODO: add buff for checkmate (can be used on an enemy piece and is always used on the king)
        // TODO: buffs (undo action(zaa wardoo), multiple tile destroy, more reach, player second chance, multi-life chess pieces, freeze opponent piece, "invisible" pieces, clone pieces, more gold, more time)
        // TODO: consumables (one-time use buffs, one-time use pieces, chess pieces management (destroy pieces, clone))
        // TODO: add shop (buffs, custom pieces, consumables)
        // TODO: add function to save and load game state (including board, pieces, move history, etc.)
        // TODO: add function to never have negative position values, when adding new tiles. The left bottom tile should always be (0, 0) and the top right tile should be (board.Width, board.Height)
        // TODO: add function to display piece buffs, hp, strength and other info when hovering over a piece
        // TODO: add function to add material to pieces or tiles when specific buffs are applied
        public static ChessBoard Instance { get; private set; }
        public ChessBoardData BoardData;

        [Header("Board dimensions and scaling")]
        public int Width = 8;
        public int Height = 8;
        public float TileSize = 1.0f;
        public bool AutoScale = true;
        public float ScalePadding = 0.05f;
        private RectTransform parentRectTransform;
        private float calculatedTileSize;

        [Header("Sprites, fonts and materials")]
        public Font MainFont;

        [Header("Sprites for Tiles and Pieces")]
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

        private readonly Dictionary<string, ChessTile> tiles = new();
        private ChessPiece selectedPiece;
        private readonly List<ChessTile> highlightedTiles = new();

        [Header("Game variables")]
        public bool IsWhiteTurn = true;
        public List<ChessMoveHistory> MoveHistory = new();

        public int CurrentTurn { get; internal set; }
        public int CurrentRound { get; internal set; }

        private NotificationManager notificationManager;
        private EnemySpriteManager enemySpriteManager;
        private RoundEndUIManager roundEndManager;

        protected override void Awake()
        {
            // Singleton pattern for ChessBoard
            if (Instance == null)
            {
                Instance = this;
                base.Awake(); // Call parent Awake
            }
            else if (Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
        }

        public override void Initialize()
        {
            if (IsInitialized)
            {
                return; // Prevent multiple initialization
            }

            this.StartGame();
            SetActiveAll(false);
            // LoadGameState();

            base.Initialize(); // Mark as initialized
            Debug.Log("ChessBoard initialized");
        }

        public void StartGame()
        {
            if (this.BoardData == null)
            {
                Debug.LogError("Board setup is null!");
                return;
            }

            this.CalculateScaling();
            this.GenerateBoard();
            this.SetupPromotion();
        }

        public void Update()
        {
            this.HandleInput();
        }

        public void GenerateBoard()
        {
            // Clear existing tiles
            foreach (var tile in this.tiles.Values)
            {
                if (tile == null)
                {
                    continue;
                }

                tile.Destroy();
            }
            this.tiles.Clear();
            this.MoveHistory.Clear();
            this.ClearHighlights();

            // Use board dimensions from setup
            int boardWidth = this.BoardData.BoardWidth;
            int boardHeight = this.BoardData.BoardHeight;

            // Calculate board positioning
            float boardWidthWorld = boardWidth * this.calculatedTileSize;
            float boardHeightWorld = boardHeight * this.calculatedTileSize;
            var boardCenter = new Vector2(boardWidthWorld / 2, boardHeightWorld / 2);
            var startPos = new Vector2(
                -boardCenter.x + (this.calculatedTileSize / 2),
                -boardCenter.y + (this.calculatedTileSize / 2)
            );

            // Generate tiles using setup data
            for (int x = 0; x < boardWidth; x++)
            {
                for (int y = 0; y < boardHeight; y++)
                {
                    ChessTileData tileData = this.BoardData.GetTileDataAt(x, y);

                    this.CreateTileFromData(x, y, tileData, startPos);
                }
            }
        }

        // Helper method to create a tile from tile data
        private void CreateTileFromData(int x, int y, ChessTileData tileData, Vector2 startPos)
        {
            // Create chess tile game object
            var tileObject = new GameObject($"Tile_{CoordinateHelper.XYToString(x, y)}");
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
            bool isWhite = IsWhite(x, y);

            // Setup sprite
            if (tileData.Sprite == null)
            {
                tileData.Sprite = isWhite ? this.WhiteTileSprite : this.BlackTileSprite;
            }

            // Initialize with ScriptableObject data
            tile.Initialize(tileData, new Vector2Int(x, y), isWhite);

            // Set proper size for the tile
            tileObject.transform.localScale = new Vector3(
                1 / this.calculatedTileSize,
                1 / this.calculatedTileSize,
                -1
            );

            // Add to dictionary for easy lookup
            this.tiles.Add(CoordinateHelper.XYToString(x, y), tile);
        }

        // Spawn a chess piece on the board
        public ChessPiece SpawnPiece(
            ChessPieceType type,
            bool isWhite,
            string position,
            ChessTile tile = null,
            Sprite customSprite = null,
            List<Material> customMaterials = null,
            List<MoveRule> customMoveRules = null
        )
        {
            var pieceObject = new GameObject($"{(isWhite ? "White" : "Black")}_{type}");
            pieceObject.transform.parent = this.transform;

            var piece = pieceObject.AddComponent<ChessPiece>();

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
            piece.Initialize(type, isWhite, sprite, pieceMaterials, customMoveRules);

            // Scale to fit within the tile, centered
            float scaleX = this.calculatedTileSize * 0.8f; // Use 80% of tile width
            float scaleY = this.calculatedTileSize * 0.8f; // Use 80% of tile height
            float scale = Mathf.Min(scaleX, scaleY); // Use the smaller scale to maintain aspect ratio

            pieceObject.transform.localScale = new Vector3(scale, scale, -2); // -2 to render in front of tiles

            if (tile == null)
            {
                Debug.Log(
                    $"Tile {position} not found. Spawning piece without tile setting to inactive."
                );
                piece.gameObject.SetActive(false);
            }
            else
            {
                // Place the piece on the tile
                tile.UpdatePiece(piece, true, true);
            }

            return piece;
        }

        // Keep existing methods for backward compatibility
        public void SetupPromotion()
        {
            PromoteAtEndPieceBuff.InitializePromotionSystem(SelectionUIManager.Instance);
            PromoteAtEndPieceBuff.ConfigurePromotionPieces(
                pieces: new List<IChessObject>
                {
                    this.SpawnPiece(ChessPieceType.Queen, true, null),
                    this.SpawnPiece(ChessPieceType.Rook, true, null),
                    this.SpawnPiece(ChessPieceType.Bishop, true, null),
                    this.SpawnPiece(ChessPieceType.Knight, true, null),
                },
                tooltips: new List<string> { "Queen", "Rook", "Bishop", "Knight" }
            );
        }

        // Get all pieces from player
        public List<ChessPiece> GetAllPieces(bool isWhite)
        {
            List<ChessPiece> pieces = new();
            foreach (var tile in this.tiles.Values)
            {
                if (tile.CurrentPiece != null && tile.CurrentPiece.IsWhite == isWhite)
                {
                    pieces.Add(tile.CurrentPiece);
                }
            }
            return pieces;
        }

        public ChessPiece[] GetAllPiecesArray(bool isWhite)
        {
            var pieces = new ChessPiece[this.Width * this.Height];

            foreach (var tile in this.tiles.Values)
            {
                if (tile.CurrentPiece != null && tile.CurrentPiece.IsWhite == isWhite)
                {
                    int index = (tile.Position.y * this.Width) + tile.Position.x;
                    pieces[index] = tile.CurrentPiece;
                }
            }
            return pieces;
        }

        // Helper to get the proper sprite based on piece type and color (legacy support)
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
                    _ => this.WhitePawnSprite,
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
                    _ => this.BlackPawnSprite,
                };
            }
        }

        public static void DestroyObjectsByType<T>(
            FindObjectsSortMode sortMode = FindObjectsSortMode.None
        )
            where T : UnityEngine.Object
        {
            T[] objects = UnityEngine.Object.FindObjectsByType<T>(sortMode);
            foreach (T obj in objects)
            {
                if (Application.isPlaying)
                {
                    UnityEngine.Object.Destroy(obj);
                }
                else
                {
                    UnityEngine.Object.DestroyImmediate(obj);
                }
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

                return;
            }

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
                return;
            }

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

            // Update tiles current piece (destroy/disable old piece if any)
            targetTile.UpdatePiece(piece, false, false);

            // Update Enemy icons
            if (this.enemySpriteManager != null)
            {
                this.enemySpriteManager.OnPlayerMove();
            }

            this.ClearHighlights();
            this.selectedPiece.UseUpdateBuffs(); // Apply buffs after move
            this.selectedPiece = null;

            this.CheckGameState(!this.IsWhiteTurn); // Check game state for enemy

            this.IsWhiteTurn = !this.IsWhiteTurn; // Switch turns after move
        }

        public void AddMove(
            ChessPiece piece,
            Vector2Int? currentPos,
            Vector2Int? targetPos,
            ChessPiece mainCapturedPiece,
            List<ChessPiece> additionalCapturedPieces,
            bool? isWhiteTurn = null
        )
        {
            this.MoveHistory.Add(
                new ChessMoveHistory(
                    piece,
                    currentPos,
                    targetPos,
                    mainCapturedPiece,
                    additionalCapturedPieces,
                    isWhiteTurn ?? this.IsWhiteTurn
                )
            );

            if (currentPos != null && targetPos != null)
            {
                var notification =
                    CoordinateHelper.VectorToString((Vector2Int)currentPos)
                    + " to "
                    + CoordinateHelper.VectorToString((Vector2Int)targetPos);
                this.notificationManager.CreateSlideUpUI(notification);
            }
        }

        public bool CanUndo()
        {
            return this.MoveHistory.Count > 0;
        }

        public void UndoLastMove()
        {
            // TODO: reset buffs of pieces in move history
            // TODO: add change to previous piece on updatebuff (e.g. PromoteAtEndPieceBuff)
            if (!this.CanUndo())
            {
                return;
            }

            // Get the last move
            var lastMove = this.MoveHistory[this.MoveHistory.Count - 1];
            this.MoveHistory.RemoveAt(this.MoveHistory.Count - 1);

            // Get the tiles involved
            if (
                lastMove.FromPosition is not Vector2Int fromPos
                || !this.GetTile(fromPos, out ChessTile fromTile)
            )
            {
                return;
            }

            if (
                lastMove.ToPosition is not Vector2Int toPos
                || !this.GetTile(toPos, out ChessTile toTile)
            )
            {
                return;
            }

            ChessPiece pieceToMove = lastMove.MovedPiece;

            // ChessPiece was updated. Deactivate update piece and activate previous piece
            // TODO: add chess piece properties to ChessMoveHistory and restore them if they were changed, instead of this
            var toTilePiece = toTile.CurrentPiece;
            if (
                toTilePiece != null
                && toTilePiece.gameObject != null
                && toTilePiece.gameObject.GetInstanceID() != pieceToMove.gameObject.GetInstanceID()
                && toTilePiece.IsWhite == pieceToMove.IsWhite
            )
            {
                toTilePiece.gameObject.SetActive(false);
            }

            // Move the piece back
            fromTile.UpdatePiece(pieceToMove, true, true);

            // Restore captured pieces if there was one
            if (
                lastMove.AdditionalCapturedPieces != null
                && lastMove.AdditionalCapturedPieces.Count > 0
            )
            {
                foreach (ChessPiece capturePiece in lastMove.AdditionalCapturedPieces)
                {
                    if (capturePiece == null)
                    {
                        continue;
                    }

                    // Reactivate the captured piece
                    capturePiece.gameObject.SetActive(true);
                }
            }

            if (lastMove.MainCapturedPiece != null)
            {
                // Reactivate the captured piece
                lastMove.MainCapturedPiece.gameObject.SetActive(true);
                toTile.UpdatePiece(lastMove.MainCapturedPiece, true, true);
            }
            else
            {
                toTile.UpdatePiece(null, true, true);
            }

            pieceToMove.gameObject.SetActive(true);

            // Restore the turn
            this.IsWhiteTurn = lastMove.WasWhiteTurn;

            // Clear any highlights
            this.ClearHighlights();
            this.selectedPiece = null;

            this.notificationManager.PopNewestUI();
        }

        public void UndoMoves(int numberOfMoves)
        {
            int movesToUndo = Mathf.Min(numberOfMoves, this.MoveHistory.Count);
            for (int i = 0; i < movesToUndo; i++)
            {
                this.UndoLastMove();
            }
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

        public void SetEnemyTurn(bool enemyTurn)
        {
            if (this.enemySpriteManager != null)
            {
                this.enemySpriteManager.ShowLoading(enemyTurn);
            }
        }

        public void MovePieceEnemy(ChessPiece piece, ChessMove move)
        {
            if (move == null)
            {
                Debug.LogError("Enemy Move cannot be null");
                return;
            }

            string targetCoord = CoordinateHelper.VectorToString(move.TargetPosition);

            if (this.GetTile(targetCoord, out ChessTile targetTile))
            {
                // Use the same MovePiece method so it gets recorded
                this.MovePiece(piece, targetTile);
            }
        }

        public bool GetTile(string position, out ChessTile tile)
        {
            tile = null;
            return position != null && this.tiles.TryGetValue(position, out tile);
        }

        public bool GetTile(Vector2Int vector, out ChessTile tile)
        {
            return this.GetTile(CoordinateHelper.VectorToString(vector), out tile);
        }

        public bool IsValidCoordinate(string coordinate)
        {
            return this.tiles.ContainsKey(coordinate);
        }

        public bool IsValidCoordinate(Vector2Int vector)
        {
            return this.tiles.ContainsKey(CoordinateHelper.VectorToString(vector));
        }

        // Get all pieces of a specific type and color
        public List<ChessPiece> GetPiecesOfType(ChessPieceType type, bool isWhite)
        {
            var pieces = new List<ChessPiece>();
            foreach (var tile in this.tiles.Values)
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
        public bool IsInCheck(bool isWhiteKing, ChessPiece king)
        {
            // Check if any enemy piece can attack the king's position
            List<ChessPiece> enemyPieces = this.GetAllPieces(!isWhiteKing);
            foreach (var enemyPiece in enemyPieces)
            {
                var validMoves = enemyPiece.GetValidMoves();
                foreach (var move in validMoves)
                {
                    if (move.TargetPosition == king.CurrentTile.Position)
                    {
                        return true; // King is in check
                    }
                }
            }

            return false;
        }

        // Evaluate if the current position is checkmate or stalemate
        public void CheckGameState(bool isWhiteTurn)
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

            // TODO: add event for check (Highlight king, show check icon, etc.)
            // TODO: instead of returning GameState, add a GameOver event that can be handled by the UI
            if (!hasValidMoves)
            {
                this.EndGame(isWhiteTurn); // Checkmate
                return;
            }

            // Find the king
            List<ChessPiece> kings = this.GetPiecesOfType(ChessPieceType.King, isWhiteTurn);
            if (kings.Count == 0)
            {
                this.EndGame(isWhiteTurn); // No king found
                return;
            }

            // Check for check
            bool checkAnimation = false;
            foreach (ChessPiece king in kings)
            {
                bool inCheck = this.IsInCheck(isWhiteTurn, king);
                king.CurrentTile.InCheck(inCheck);
                if (inCheck && !isWhiteTurn && !checkAnimation)
                {
                    // TODO: add animation for enemy (sweating or blinking exclamation mark)
                    checkAnimation = true;
                }
            }

            // Game continues
        }

        public static bool IsWhite(int x, int y)
        {
            return (x + y) % 2 == 1;
        }

        public bool AddChessPiece(ChessPiece piece, int x, int y, bool replaceExisting)
        {
            if (piece == null)
            {
                Debug.LogWarning("ChessPiece is null!");
                return false;
            }

            string position = CoordinateHelper.XYToString(x, y);

            if (!this.GetTile(position, out ChessTile tile))
            {
                Debug.LogWarning($"ChessTile for position {position} does not exist!");
                return false;
            }

            if (tile.CurrentPiece != null || !replaceExisting)
            {
                Debug.LogWarning(
                    "Current piece of chessTile is not null, but replaceExisting is set to false!"
                );
                return false;
            }

            this.SpawnPiece(
                piece.PieceType,
                piece.IsWhite,
                position,
                tile,
                piece.SpriteRenderer.sprite,
                new List<Material>(piece.SpriteRenderer.materials),
                piece.MoveRules
            );
            return true;
        }

        public static bool RemoveChessPiece(ChessPiece piece)
        {
            if (piece == null)
            {
                Debug.LogWarning("ChessPiece is null!");
                return false;
            }

            if (piece.CurrentTile == null)
            {
                Debug.LogWarning("Current tile of ChessPiece is null!");
                return false;
            }

            piece.CurrentTile.UpdatePiece(null, true, false);
            return true;
        }

        public void SwitchChessPieces(string fromPos, string toPos)
        {
            if (
                this.GetTile(fromPos, out ChessTile fromTile)
                && this.GetTile(toPos, out ChessTile toTile)
            )
            {
                var fromPiece = fromTile.CurrentPiece;
                fromTile.UpdatePiece(toTile.CurrentPiece, true, true);
                toTile.UpdatePiece(fromPiece, true, true);
            }
        }

        private void EndGame(bool isWhiteTurn)
        {
            int countPiecesWhite = this.GetAllPieces(true).Count;
            if (PromoteAtEndPieceBuff.SelectionUIManager != null) {
                PromoteAtEndPieceBuff.SelectionUIManager.HideSelectionUI();
            }
            if (isWhiteTurn)
            {
                Debug.Log("Round lost");
                RefreshBoard(true);
                this.roundEndManager.ShowRoundEndPanel(
                    false,
                    countPiecesWhite,
                    InventoryManager.Instance.Income,
                    InventoryManager.Instance.PlayerBuffs
                );
            }
            else
            {
                RefreshBoard(false);
                Debug.Log("Round won");
                this.roundEndManager.ShowRoundEndPanel(
                    true,
                    countPiecesWhite,
                    InventoryManager.Instance.Income,
                    InventoryManager.Instance.PlayerBuffs
                );
            }
        }

        private static void RefreshBoard(bool fullRefresh)
        {
            if (fullRefresh)
            {
                Instance.GenerateBoard();
            }
            else
            {
                while (Instance.CanUndo())
                {
                    Instance.UndoLastMove();
                }
                Instance.MoveHistory.Clear();
                Instance.ClearHighlights();
                // TODO: update buffs that end after x rounds
            }
            Instance.IsWhiteTurn = true;
            // TODO: fix selection manager being triggered on end
        }

        public static void SetActiveAll(bool active)
        {
            foreach (ChessTile tile in Instance.tiles.Values)
            {
                if (tile.CurrentPiece != null)
                {
                    tile.CurrentPiece.gameObject.SetActive(active);
                }
                tile.gameObject.SetActive(active);
            }
        }

        public override void OnSceneChanged(string sceneName)
        {
            if (sceneName == "Game")
            {
                SetActiveAll(true);
                this.enemySpriteManager = FindFirstObjectByType<EnemySpriteManager>();
                this.notificationManager = FindFirstObjectByType<NotificationManager>();
                this.roundEndManager = FindFirstObjectByType<RoundEndUIManager>();
                Instance.IsWhiteTurn = true;
            }
            else
            {
                SetActiveAll(false);
            }
        }
    }
}
