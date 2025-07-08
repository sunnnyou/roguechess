namespace Assets.Scripts.Game.Board
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Assets.Scripts.Game.Buffs;
    using Assets.Scripts.Game.Buffs.Pieces.Update;
    using Assets.Scripts.Game.Enemy;
    using Assets.Scripts.Game.MoveRules;
    using Assets.Scripts.Game.Player;
    using Assets.Scripts.UI;
    using TMPro;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class ChessBoard : BaseManager
    {
        // TODO: add tile positions to border of chessboard
        // TODO: add castling
        // TODO: fix check highlighting in ChessTile
        // TODO: buffs (undo action(zaa wardoo), multiple tile destroy, more reach, player second chance, multi-life chess pieces, freeze opponent piece, "invisible" pieces, clone pieces, more gold, more time)
        // TODO: add buying piece pack for selection of pieces and consumable to destroy or change buffs
        // TODO: add function to save and load game state (including board, pieces, move history, etc.)
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
        public Dictionary<int, ChessPiece> DownedPieces { get; internal set; } = new();

        private NotificationManager notificationManager;
        private EnemySpriteManager enemySpriteManager;
        private RoundEndUIManager roundEndManager;
        private ChessEngine chessEngine;
        private TextMeshProUGUI roundNumber;
        private TextMeshProUGUI roundText;
        private bool gameEnd;
        private EnemyRound enemyRoundData;
        private bool skipEnemy;

        public List<BuffBase> Buffs { get; private set; } = new();

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
            if (this.IsInitialized)
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
        }

        public void Update()
        {
            if (this.gameEnd)
            {
                return;
            }

            try
            {
                if (this.IsWhiteTurn)
                {
                    this.HandleInput();
                }
                else if (this.chessEngine == null || this.skipEnemy)
                {
                    Debug.Log(
                        $"Skipping next enemy turn. ChessEngine is null: {this.chessEngine == null}"
                    );
                    this.skipEnemy = false;
                    this.IsWhiteTurn = !this.IsWhiteTurn;
                }
                // Make AI move for black pieces
                else if (this.chessEngine.MakeBestMove(false))
                {
                    // no more valid moves for enemy
                    this.EndGame(false);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                this.IsWhiteTurn = !this.IsWhiteTurn;
            }
        }

        public void GenerateBoard()
        {
            Debug.Log("Generating Board");

            // Clear existing tiles
            foreach (var tile in this.tiles.Values)
            {
                if (tile == null)
                {
                    continue;
                }
                tile.Destroy();
            }

            foreach (var downed in this.DownedPieces.Values)
            {
                if (downed == null)
                {
                    continue;
                }

                if (Application.isEditor)
                {
                    DestroyImmediate(downed);
                }
                else
                {
                    Destroy(downed);
                }
            }

            this.tiles.Clear();
            this.MoveHistory.Clear();
            this.DownedPieces.Clear();
            this.CurrentRound = 0;
            this.CurrentTurn = 0;
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
                MusicManager.Instance.PlayPickUpSound();
                // Move the piece
                this.MovePiece(this.selectedPiece, clickedTile);
            }
            else if (clickedTile.CurrentPiece != null) // If we clicked on a piece
            {
                bool isPlayersPiece = clickedTile.CurrentPiece.IsWhite == this.IsWhiteTurn;
                if (!isPlayersPiece)
                {
                    this.ClearHighlights();
                    return; // Can't select opponent's pieces
                }

                // Deselect current piece
                if (this.selectedPiece != null)
                {
                    this.ClearHighlights();
                }

                // Select new piece
                this.selectedPiece = clickedTile.CurrentPiece;
                var validMoves = this.HighlightValidMoves(this.selectedPiece);
                if (validMoves.Count > 0)
                {
                    MusicManager.Instance.PlayClickSound();
                }
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
            if (this.selectedPiece == null)
            {
                this.selectedPiece = piece;
            }
            this.selectedPiece.UseUpdateBuffs(); // Apply buffs after move
            this.selectedPiece = null;

            this.CheckGameState(!this.IsWhiteTurn); // Check game state for enemy

            this.IsWhiteTurn = !this.IsWhiteTurn; // Switch turns after move
        }

        public void AddMove(ChessMoveHistory moveToAdd)
        {
            if (moveToAdd == null)
            {
                return;
            }

            this.MoveHistory.Add(moveToAdd);

            if (moveToAdd.FromPosition != null && moveToAdd.ToPosition != null)
            {
                var notification =
                    CoordinateHelper.VectorToString((Vector2Int)moveToAdd.FromPosition)
                    + " to "
                    + CoordinateHelper.VectorToString((Vector2Int)moveToAdd.ToPosition);
                this.notificationManager.CreateSlideUpUI(notification);
            }
        }

        public bool CanUndo()
        {
            return this.MoveHistory.Count > 0;
        }

        public void UndoLastMove(bool ignoreTurn = false)
        {
            if (!this.CanUndo())
            {
                return;
            }

            // Get the last move
            var lastMove = this.MoveHistory[this.MoveHistory.Count - 1];
            this.MoveHistory.RemoveAt(this.MoveHistory.Count - 1);

            this.UndoMove(lastMove);

            // Restore the turn
            if (!ignoreTurn)
            {
                this.IsWhiteTurn = lastMove.WasWhiteTurn;
            }

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

        public void UndoMove(ChessMoveHistory moveToUndo)
        {
            // ChessPiece was updated. Deactivate update piece and activate previous piece
            // TODO: add chess piece properties to ChessMoveHistory and restore them if they were changed, instead of this
            // TODO: reset buffs of pieces in move history
            // TODO: add change to previous piece on updatebuff (e.g. PromoteAtEndPieceBuff)

            if (moveToUndo == null)
            {
                return;
            }

            // Get the tiles involved
            if (
                moveToUndo.FromPosition is not Vector2Int fromPos
                || !this.GetTile(fromPos, out ChessTile fromTile)
            )
            {
                return;
            }

            if (
                moveToUndo.ToPosition is not Vector2Int toPos
                || !this.GetTile(toPos, out ChessTile toTile)
            )
            {
                return;
            }

            ChessPiece pieceToMove = moveToUndo.MovedPiece;
            var toTilePiece = toTile.CurrentPiece;
            if (
                toTilePiece != null
                && toTilePiece.gameObject != null
                && toTilePiece.gameObject.GetInstanceID() != pieceToMove.gameObject.GetInstanceID()
                && toTilePiece.IsWhite == pieceToMove.IsWhite
            )
            {
                // TODO: is this needed?
                toTilePiece.gameObject.SetActive(false);
            }

            // Move the piece back
            fromTile.UpdatePiece(pieceToMove, true, true);

            // Restore additional captured pieces if there was one
            if (
                moveToUndo.AdditionalCapturedPieces != null
                && moveToUndo.AdditionalCapturedPieces.Count > 0
            )
            {
                foreach (ChessPiece capturePiece in moveToUndo.AdditionalCapturedPieces)
                {
                    if (capturePiece == null)
                    {
                        continue;
                    }

                    // Reactivate the captured piece
                    capturePiece.RevivePiece(pieceToMove.Strength);
                }
            }

            if (moveToUndo.MainCapturedPiece != null)
            {
                // Reactivate the captured piece
                moveToUndo.MainCapturedPiece.RevivePiece(pieceToMove.Strength);
                toTile.UpdatePiece(moveToUndo.MainCapturedPiece, true, true);
            }
            else
            {
                toTile.UpdatePiece(null, true, true);
            }

            pieceToMove.gameObject.SetActive(true);
        }

        // Highlights all valid moves for a piece
        public List<ChessTile> HighlightValidMoves(ChessPiece piece)
        {
            if (piece == null)
            {
                return new List<ChessTile>();
            }

            List<ChessTile> validTiles = piece.GetValidTiles();

            foreach (ChessTile tile in validTiles)
            {
                tile.Highlight(true);
                this.highlightedTiles.Add(tile);
            }

            return validTiles;
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

        public bool GetTile(string position, out ChessTile tile)
        {
            tile = null;
            return position != null && this.tiles.TryGetValue(position, out tile);
        }

        public bool GetTile(Vector2Int vector, out ChessTile tile)
        {
            return this.GetTile(CoordinateHelper.VectorToString(vector), out tile);
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
            if (king == null)
            {
                Debug.LogWarning("king is null in IsInCheck");
                return false;
            }

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

            // Return default color for tiles
            foreach (ChessTile tile in this.tiles.Values)
            {
                tile.InCheck(false);
            }

            if (!hasValidMoves)
            {
                this.EndGame(isWhiteTurn); // Checkmate
                return;
            }

            // Find the king
            List<ChessPiece> kingsWhite = this.GetPiecesOfType(ChessPieceType.King, isWhiteTurn);
            if (kingsWhite.Count == 0)
            {
                this.EndGame(isWhiteTurn); // No king found
                return;
            }

            // Check for check
            bool checkAnimation = false;
            foreach (ChessPiece king in kingsWhite)
            {
                bool inCheck = this.IsInCheck(isWhiteTurn, king);
                king.CurrentTile.InCheck(inCheck);
                if (inCheck && !isWhiteTurn && !checkAnimation)
                {
                    // TODO: add animation for enemy (sweating or blinking exclamation mark)
                    checkAnimation = true;
                }
            }

            // Find the king
            List<ChessPiece> kingsBlack = this.GetPiecesOfType(ChessPieceType.King, isWhiteTurn);
            if (kingsBlack.Count == 0)
            {
                this.EndGame(!isWhiteTurn); // No king found
                return;
            }

            // Check for check
            checkAnimation = false;
            foreach (ChessPiece king in kingsBlack)
            {
                bool inCheck = this.IsInCheck(!isWhiteTurn, king);
                king.CurrentTile.InCheck(inCheck);
                if (inCheck && isWhiteTurn && !checkAnimation)
                {
                    // TODO: add animation for player
                    checkAnimation = true;
                }
            }
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
            this.gameEnd = true;
            int countPiecesWhite = this.GetAllPieces(true).Count;
            if (PromoteAtEndPieceBuff.SelectionUIManager != null)
            {
                PromoteAtEndPieceBuff.SelectionUIManager.HideSelectionUI();
            }

            if (isWhiteTurn)
            {
                Debug.Log("Round lost");
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
                Instance.DownedPieces.Clear();
                Instance.MoveHistory.Clear();
                Instance.ClearHighlights();

                // Remove Enemy pieces
                if (Instance.enemyRoundData != null)
                {
                    foreach (EnemyPiece enemyPiece in Instance.enemyRoundData.Pieces)
                    {
                        if (Instance.GetTile(enemyPiece.Position, out ChessTile tile))
                        {
                            RemoveChessPiece(tile.CurrentPiece);
                        }
                    }
                }

                Instance.CurrentRound++;
                Instance.CurrentTurn = 0;
                // TODO: update buffs that end after x rounds
            }
            Instance.IsWhiteTurn = true;
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
                this.IsWhiteTurn = true;
                this.gameEnd = false;
                SetActiveAll(true);
                this.enemySpriteManager = FindFirstObjectByType<EnemySpriteManager>();
                this.notificationManager = FindFirstObjectByType<NotificationManager>();
                this.roundEndManager = FindFirstObjectByType<RoundEndUIManager>();
                this.chessEngine = FindFirstObjectByType<ChessEngine>();
                this.roundNumber = GameObject
                    .Find("RoundNumberText")
                    .GetComponent<TextMeshProUGUI>();
                this.roundText = GameObject
                    .Find("RoundNameLabelText")
                    .GetComponent<TextMeshProUGUI>();

                this.SetupPromotion();
                this.GetEnemyRoundData();
            }
            else
            {
                this.IsWhiteTurn = true;
                SetActiveAll(false);
            }
        }

        internal IEnumerable<ChessTile> GetAllTiles()
        {
            return this.tiles.Values;
        }

        private void GetEnemyRoundData()
        {
            // Add enemy pieces
            this.enemyRoundData = EnemyRoundDatabase.GetRound(this.CurrentRound + 1);
            if (this.enemyRoundData == null)
            {
                return;
            }

            if (this.roundNumber != null)
            {
                this.roundNumber.text = (this.CurrentRound + 1).ToString();
            }

            if (this.roundText != null)
            {
                this.roundText.text = this.enemyRoundData.Name;
            }

            foreach (EnemyPiece enemyPiece in this.enemyRoundData.Pieces)
            {
                if (this.GetTile(enemyPiece.Position, out ChessTile tile))
                {
                    this.SpawnPiece(enemyPiece.PieceType, false, enemyPiece.Position, tile);
                }
            }
        }

        public void NewTurn()
        {
            this.CurrentTurn++;
            foreach (BuffBase buff in this.Buffs)
            {
                buff.UpdateDuration();
            }
        }

        public int? GetTotalEnemyPieceCount()
        {
            if (this.enemyRoundData != null && this.enemyRoundData.Pieces != null)
            {
                return this.enemyRoundData.Pieces.Length;
            }
            else
            {
                return null;
            }
        }

        public void SkipNextRound()
        {
            this.skipEnemy = true;
        }
    }
}
