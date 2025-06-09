namespace Assets.Scripts.Game.Board
{
    using System.Collections.Generic;
    using Assets.Scripts.Game.Buffs.Pieces.Update;
    using Assets.Scripts.Game.Enemy;
    using Assets.Scripts.Game.MoveRules;
    using Assets.Scripts.UI;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class ChessBoard : MonoBehaviour
    {
        // TODO: fix bug with overlapping enemy icons
        // TODO: add custom piece with custom movement rules
        // TODO: add function to receive new piece when pawn reaches the end (maybe with custom piece)
        // TODO: add other chess functions (castling, en passant, promotion, check, checkmate, stalemate)
        // TODO: add buff for checkmate (can be used on an enemy piece and is always used on the king)
        // TODO: buffs (undo action(zaa wardoo), multiple tile destroy, more reach, player second chance, multi-life chess pieces, freeze opponent piece, "invisible" pieces, clone pieces, more gold, more time)
        // TODO: consumables (one-time use buffs, one-time use pieces, chess pieces management (destroy pieces, clone))
        // TODO: add shop (buffs, custom pieces, consumables)
        // TODO: add function to save and load game state (including board, pieces, move history, etc.)
        // TODO: add function to never have negative position values, when adding new tiles. The left bottom tile should always be (0, 0) and the top right tile should be (board.Width, board.Height)
        // TODO: add function to display piece buffs when hovering over a piece
        // TODO: add function to add material to pieces or tiles when specific buffs are applied

        // Board dimensions and scaling
        public int Width = 8;
        public int Height = 8;
        public float TileSize = 1.0f;
        public bool AutoScale = true;
        public float ScalePadding = 0.05f;
        private RectTransform parentRectTransform;
        private float calculatedTileSize;

        // Sprites, fonts and materials
        public SelectionUIManager SelectionManager;
        public EnemySpriteManager EnemySpriteManager;
        public Font MainFont;
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
        private List<ChessTile> highlightedTiles = new List<ChessTile>();
        private RectTransform parentRectTransform;
        private float calculatedTileSize;

        // Game variables
        public bool IsWhiteTurn = true;
        public bool GameOver;
        public bool IsEnemyTurn;
        public List<ChessMoveHistory> MoveHistory = new();

        public int CurrentTurn { get; internal set; }

        public int CurrentRound { get; internal set; }

        // Start is called before the first frame update
        public void Start()
        {
            this.CalculateScaling();
            this.GenerateBoard();
            this.SetupTraditionalPieces();
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
            }
        }

        // Update is called once per frame
        public void Update()
        {
            // Block input during AI turn
            if (this.IsAITurn || this.gameOver)
            {
                return;
            }

            this.HandleInput();
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
                this.selectedPiece.UseUpdateBuffs(); // Apply buffs after move
                this.selectedPiece = null;

                this.CheckGameState(!this.IsWhiteTurn); // Check game state for enemy

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
            ChessTile currentTile = piece.CurrentTile;
            ChessPiece capturedPiece = targetTile.CurrentPiece;

            // Update tiles current piece (destroy/disable old piece if any)
            targetTile.UpdatePiece(piece, false, false);
            currentTile.CurrentPiece = null;

            // Update Enemy icons
            if (this.EnemySpriteManager != null)
            {
                this.EnemySpriteManager.OnPlayerMove();
            }
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

            // Initialize the piece
            piece.Initialize(type, isWhite, sprite, this, this.PieceMaterial);

            // Scale to fit within the tile, centered
            float scaleX = this.calculatedTileSize * 0.8f; // Use 80% of tile width
            float scaleY = this.calculatedTileSize * 0.8f; // Use 80% of tile height
            float scale = Mathf.Min(scaleX, scaleY); // Use the smaller scale to maintain aspect ratio

            pieceObject.transform.localScale = new Vector3(scale, scale, -2); // -2 to render in front of tiles

            // Place the piece on the tile
            tile.UpdatePiece(piece);

            return piece;
        }

        // Helper to get the proper sprite based on piece type and color
        public Sprite GetPieceSprite(ChessPieceType type, bool isWhite)
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
        public void HighlightValidMoves(ChessPiece piece)
        {
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

        // Move a piece to a new tile
        public void MovePiece(ChessPiece piece, ChessTile targetTile)
        {
            if (piece == null || targetTile == null)
            {
                return;
            }

            // Update tiles current piece (destroy old piece if any)
            targetTile.UpdatePiece(piece);

            // Update AI icons
            if (this.SpriteHolder != null)
            {
                this.EnemySpriteManager.ShowLoading(enemyTurn);
            }
        }

        // Undo the last move
        public bool UndoLastMove()
        {
            if (this.moveHistory.Count == 0)
            {
                Debug.LogWarning("No moves to undo!");
                return false;
            }

            MoveRecord lastMove = this.moveHistory.Pop();

            // Restore the moved piece to its original position
            lastMove.FromTile.CurrentPiece = lastMove.MovedPiece;
            lastMove.MovedPiece.CurrentTile = lastMove.FromTile;

            // Update piece position visually
            if (lastMove.MovedPiece.transform != null)
            {
                lastMove.MovedPiece.transform.position = new Vector3(
                    lastMove.FromTile.transform.position.x,
                    lastMove.FromTile.transform.position.y,
                    -2
                );
            }

            // Restore captured piece if there was one
            if (lastMove.CapturedPiece != null)
            {
                lastMove.ToTile.CurrentPiece = lastMove.CapturedPiece;
                lastMove.CapturedPiece.CurrentTile = lastMove.ToTile;

                // Make captured piece visible again
                if (lastMove.CapturedPiece.gameObject != null)
                {
                    lastMove.CapturedPiece.gameObject.SetActive(true);
                }
            }
            else
            {
                // Clear the target tile if no piece was captured
                lastMove.ToTile.CurrentPiece = null;
            }

            // Restore piece's first move status
            if (lastMove.WasFirstMove)
            {
                lastMove.MovedPiece.HasMoved = false;
            }

            // Restore turn
            this.isWhiteTurn = lastMove.WasWhiteTurn;

            // Clear any highlights
            this.ClearHighlights();
            this.selectedPiece = null;

            Debug.Log(
                $"Undid move: {lastMove.MovedPiece.PieceType} from {lastMove.FromTile.Coordinate} to {lastMove.ToTile.Coordinate}"
            );
            return true;
        }

        // Undo multiple moves
        public bool UndoMoves(int numberOfMoves)
        {
            if (numberOfMoves <= 0)
            {
                return false;
            }

            if (this.moveHistory.Count < numberOfMoves)
            {
                Debug.LogWarning(
                    $"Cannot undo {numberOfMoves} moves. Only {this.moveHistory.Count} moves in history."
                );
                return false;
            }

            for (int i = 0; i < numberOfMoves; i++)
            {
                if (!this.UndoLastMove())
                {
                    return false;
                }
            }

            return true;
        }

        // Get the number of moves that can be undone
        public int GetUndoableMovesCount()
        {
            return this.moveHistory.Count;
        }

        // Clear move history (useful when starting a new game)
        public void ClearMoveHistory()
        {
            this.moveHistory.Clear();
        }

        // Peek at the last move without undoing it
        public MoveRecord GetLastMove()
        {
            if (this.moveHistory.Count == 0)
            {
                return null;
            }

            return this.moveHistory.Peek();
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
                this.MovePiece(piece, targetTile);
                this.IsWhiteTurn = !this.IsWhiteTurn;
            }
        }

        public bool GetTile(string position, out ChessTile tile)
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
                this.isWhiteTurn = !this.isWhiteTurn;
            }
        }

        // Get tile by Vector2Int position (overload for AI use)
        public ChessTile GetTile(Vector2Int position)
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
        public bool? IsInCheck(bool isWhiteKing)
        {
            // Find the king
            List<ChessPiece> kings = this.GetPiecesOfType(ChessPieceType.King, isWhiteKing);
            if (kings.Count == 0)
            {
                return null; // No king found
            }

            ChessPiece king = kings[0];
            Vector2Int kingPosition = king.CurrentTile.Position;

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
        public GameState CheckGameState(bool isWhiteTurn)
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

            if (!hasValidMoves)
            {
                // TODO: instead of returning GameState, add a GameOver event that can be handled by the UI

                // No valid moves available
                if (this.IsInCheck(isWhiteTurn) is true or null)
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

        public static void HandleGameEnd(GameState state)
        {
            // Handle game end logic here
            switch (state)
            {
                case GameState.WhiteWins:
                    Debug.Log("White wins!");
                    break;
                case GameState.BlackWins:
                    Debug.Log("Black wins!");
                    break;
                case GameState.Stalemate:
                    Debug.Log("Stalemate!");
                    break;
                default:
                    Debug.Log("Game continues.");
                    break;
            }

            // TODO
        }
    }
}
