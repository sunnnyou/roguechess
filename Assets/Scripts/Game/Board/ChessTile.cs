namespace Assets.Scripts.Game.Board
{
    using UnityEngine;

    // Represents a single chess tile on the board
    public class ChessTile : MonoBehaviour
    {
        public string Coordinate;
        public Color OriginalColor;
        public bool IsWhite;
        public ChessPiece CurrentPiece;
        public SpriteRenderer SpriteRenderer;

        private Color highlightColor = new Color(0.0f, 0.4f, 0.0f); // green

        public void Awake()
        {
            this.SpriteRenderer = this.GetComponent<SpriteRenderer>();
            if (this.SpriteRenderer == null)
            {
                this.SpriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();
            }
        }

        public void Initialize(string coord, bool isWhiteTile)
        {
            this.Coordinate = coord;
            this.IsWhite = isWhiteTile;

            // Make sure we have a SpriteRenderer at this point
            if (this.SpriteRenderer == null)
            {
                this.SpriteRenderer = this.GetComponent<SpriteRenderer>();
                if (this.SpriteRenderer == null)
                {
                    this.SpriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();
                }
            }

            this.OriginalColor = this.SpriteRenderer.color;

            // Render tile behind chess pieces
            this.SpriteRenderer.sortingOrder = 2;
        }

        public void Highlight(bool highlight)
        {
            this.SpriteRenderer.color = highlight ? this.highlightColor : this.OriginalColor;
        }

        public void UpdatePiece(ChessPiece piece)
        {
            // If there's already a piece here, try to remove it
            if (this.CurrentPiece == null || this.RemovePiece(piece))
            {
                this.CurrentPiece = piece;
            }

            if (piece != null)
            {
                // Ensure Z position is in front of tiles
                piece.transform.position = new Vector3(
                    this.transform.position.x,
                    this.transform.position.y,
                    -2 // render in front of tiles
                );
                piece.CurrentTile = this;
            }
        }

        /// <summary>
        /// Tries to remove the current piece from this tile.
        /// </summary>
        /// <param name="newPiece">chess piece replacing the current piece on this tile.</param>
        /// <returns>True if piece was replaced, false otherwise.</returns>
        private bool RemovePiece(ChessPiece newPiece)
        {
            return this.CurrentPiece.FightPiece(newPiece);
        }

        // Initialize tile copy without GameObject components (for AI board copying)
        public void InitializeForCopy(string coordinate, bool isWhite)
        {
            this.Coordinate = coordinate;
            this.IsWhite = isWhite;
            this.CurrentPiece = null;

            // We don't need SpriteRenderer for AI copies
            this.SpriteRenderer = null;
        }

        // Set piece for copied board (without GameObject updates)
        public void SetPieceForCopy(ChessPiece piece)
        {
            // Remove piece from old tile if it exists
            if (piece != null && piece.CurrentTile != null && piece.CurrentTile != this)
            {
                piece.CurrentTile.CurrentPiece = null;
            }

            // Set new piece
            this.CurrentPiece = piece;
            if (piece != null)
            {
                piece.CurrentTile = this;
            }
        }

        // Get position as Vector2Int (helper for AI)
        public Vector2Int GetPosition()
        {
            ChessBoard.ParseCoordinate(this.Coordinate, out int x, out int y);
            return new Vector2Int(x, y);
        }
    }
}
