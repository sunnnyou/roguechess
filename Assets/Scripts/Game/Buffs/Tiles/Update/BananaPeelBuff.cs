namespace Assets.Scripts.Game.Buffs.Tiles.Update
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "BananaPeelBuff", menuName = "Game/Buffs/BananaPeelBuff")]
    public class BananaPeelBuff : UpdateBuff
    {
        private SpriteRenderer topSpriteRenderer;
        private GameObject topSpriteObject;

        private readonly int damageAmount = 2;

        public BananaPeelBuff()
        {
            this.UpdateFunction = this.BananaPeelFnc;
        }

        public IChessObject BananaPeelFnc(IChessObject chessObject)
        {
            if (chessObject is not ChessTile tile || tile == null)
            {
                Debug.LogError("Invalid arguments for EnPassantDebuff buff.");
                return null;
            }
            this.AddTopSpriteRenderer(tile);

            if (tile.CurrentPiece != null)
            {
                this.WasUsed = true;
                tile.CurrentPiece.AddReduceLives(-this.damageAmount, true);
                this.RemoveTopSpriteRenderer();
            }

            return null;
        }

        public override void RemoveBuff()
        {
            this.RemoveTopSpriteRenderer();
        }

        public void AddTopSpriteRenderer(ChessTile tile)
        {
            if (tile == null)
            {
                return;
            }

            // Remove existing top sprite if it exists
            if (this.topSpriteRenderer != null)
            {
                return;
                //this.RemoveTopSpriteRenderer();
            }

            // Create new GameObject for the top sprite
            var tileObject = new GameObject(
                $"Tile_{CoordinateHelper.XYToString(tile.Position.x, tile.Position.y)}_Overlay"
            );
            tileObject.transform.parent = tile.transform.parent;
            tileObject.transform.localPosition = tile.transform.localPosition;
            tileObject.transform.localScale = tile.transform.localScale;

            // Add SpriteRenderer component
            this.topSpriteRenderer = this.topSpriteObject.AddComponent<SpriteRenderer>();
            this.topSpriteRenderer.sprite = this.Icon;

            // Set sorting order to be 1 higher than main sprite
            this.topSpriteRenderer.sortingOrder = tile.SpriteRenderer.sortingOrder + 1;
        }

        /// <summary>
        /// Removes the top SpriteRenderer if it exists
        /// </summary>
        public void RemoveTopSpriteRenderer()
        {
            if (this.topSpriteRenderer != null)
            {
                // Destroy the GameObject containing the top sprite
                if (Application.isPlaying)
                {
                    Destroy(this.topSpriteObject);
                }
                else
                {
                    DestroyImmediate(this.topSpriteObject);
                }

                // Clear references
                this.topSpriteRenderer = null;
                this.topSpriteObject = null;
            }
        }
    }
}
