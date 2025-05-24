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

        // Tries to remove the current piece from this tile.
        private bool RemovePiece(ChessPiece newPiece)
        {
            return this.CurrentPiece.FightPiece(newPiece);
        }
    }
}
