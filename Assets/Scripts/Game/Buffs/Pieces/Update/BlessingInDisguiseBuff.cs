namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(
        fileName = "BlessingInDisguiseBuff",
        menuName = "Game/Buffs/BlessingInDisguiseBuff"
    )]
    public class BlessingInDisguiseBuff : UpdateBuff
    {
        private readonly int healAmount = 1;

        public BlessingInDisguiseBuff()
        {
            this.UpdateFunction = this.BlessingInDisguiseFnc;
        }

        // Heal every surrounding piece and damage current piece by healed amount
        public IChessObject BlessingInDisguiseFnc(IChessObject chessObject)
        {
            if (chessObject is not ChessPiece piece || piece == null)
            {
                Debug.LogError("Invalid arguments for BlessingInDisguise buff.");
                return null;
            }

            var surroundingPos = CoordinateHelper.GetSurroundingCoordinatesWithBounds(
                piece.CurrentTile.Position.x,
                piece.CurrentTile.Position.y
            );

            var healthHealed = 0;

            foreach (var (x, y) in surroundingPos)
            {
                if (
                    ChessBoard.Instance.GetTile(
                        CoordinateHelper.XYToString(x, y),
                        out ChessTile tile
                    )
                    && piece.IsWhite == tile.CurrentPiece.IsWhite
                )
                {
                    tile.CurrentPiece.AddReduceLives(this.healAmount, true);
                    healthHealed += this.healAmount;
                }
            }
            piece.AddReduceLives(-healthHealed, true);

            return piece;
        }
    }
}
