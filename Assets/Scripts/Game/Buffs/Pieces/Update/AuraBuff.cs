namespace Assets.Scripts.Game.Buffs.Player
{
    using System.Collections.Generic;
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "AuraBuff", menuName = "Game/Buffs/AuraBuff")]
    public class AuraBuff : UpdateBuff
    {
        public new string BuffName { get; set; }
        public new string Description { get; set; }
        public new Sprite Icon { get; set; }
        public new int Cost { get; set; }
        public new bool WasUsed { get; set; }
        private readonly int healAmount = 1;

        public AuraBuff()
        {
            this.UpdateFunction = this.AuraBuffFnc;
        }

        public IChessObject AuraBuffFnc(IChessObject chessObject)
        {
            if (chessObject is not ChessPiece piece || piece == null)
            {
                Debug.LogError("Invalid arguments for Aura buff.");
                return chessObject;
            }

            var surroundingPos = CoordinateHelper.GetSurroundingCoordinatesWithBounds(
                piece.CurrentTile.Position.x,
                piece.CurrentTile.Position.y,
                0,
                0,
                ChessBoard.Instance.Height - 1,
                ChessBoard.Instance.Height - 1
            );

            foreach (var (x, y) in surroundingPos)
            {
                if (
                    ChessBoard.Instance.GetTile(
                        CoordinateHelper.XYToString(x, y),
                        out ChessTile tile
                    )
                    && tile.CurrentPiece.gameObject.activeSelf
                    && piece.IsWhite == tile.CurrentPiece.IsWhite
                )
                {
                    tile.CurrentPiece.Lives += this.healAmount;
                }
            }
            piece.Lives += this.healAmount;
            return piece;
        }
    }
}
