namespace Assets.Scripts.Game.Buffs.Player
{
    using System.Collections.Generic;
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "AuraBuff", menuName = "Game/Buffs/AuraBuff")]
    public class AuraBuff : UpdateBuff
    {
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
                piece.CurrentTile.Position.y
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
                    tile.CurrentPiece.AddReduceLives(this.healAmount, true);
                }
            }

            piece.AddReduceLives(this.healAmount, true);
            return piece;
        }
    }
}
