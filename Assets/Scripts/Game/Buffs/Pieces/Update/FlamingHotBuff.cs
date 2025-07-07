namespace Assets.Scripts.Game.Buffs.Player
{
    using System.Threading.Tasks;
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "FlamingHotBuff", menuName = "Game/Buffs/FlamingHotBuff")]
    public class FlamingHotBuff : UpdateBuff
    {
        private readonly int damageAmount = 1;

        public FlamingHotBuff()
        {
            this.UpdateFunction = this.FlamingHotFnc;
        }

        // Damage every surrounding enemy piece and reduce current pieces strength to zero
        public IChessObject FlamingHotFnc(IChessObject chessObject)
        {
            if (chessObject is not ChessPiece piece || piece == null)
            {
                Debug.LogError("Invalid arguments for FlamingHotFnc buff.");
                return null;
            }

            var surroundingPos = CoordinateHelper.GetSurroundingCoordinatesWithBounds(
                piece.CurrentTile.Position.x,
                piece.CurrentTile.Position.y
            );

            piece.AddReduceStrength(-piece.Strength, true);

            foreach (var (x, y) in surroundingPos)
            {
                if (
                    ChessBoard.Instance.GetTile(
                        CoordinateHelper.XYToString(x, y),
                        out ChessTile tile
                    )
                    && piece.IsWhite != tile.CurrentPiece.IsWhite
                )
                {
                    tile.CurrentPiece.AddReduceLives(-this.damageAmount, true);
                }
            }

            return piece;
        }
    }
}
