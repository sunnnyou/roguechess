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
        public new string BuffName { get; set; }
        public new string Description { get; set; }
        public new Sprite Icon { get; set; }
        public new int Cost { get; set; }
        public new bool WasUsed { get; set; }

        private readonly int healAmount = 1;

        public BlessingInDisguiseBuff()
        {
            this.UpdateFunction = this.BlessingInDisguiseFnc;
        }

        public IChessObject BlessingInDisguiseFnc(IChessObject chessObject)
        {
            if (chessObject is not ChessPiece piece || piece == null)
            {
                Debug.LogError("Invalid arguments for BlessingInDisguise buff.");
                return null;
            }

            var surroundingPos = CoordinateHelper.GetSurroundingCoordinatesWithBounds(
                piece.CurrentTile.Position.x,
                piece.CurrentTile.Position.y,
                0,
                0,
                ChessBoard.Instance.Height - 1,
                ChessBoard.Instance.Height - 1
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
                    tile.CurrentPiece.Lives += this.healAmount;
                    healthHealed += this.healAmount;
                }
            }
            piece.Lives -= healthHealed;

            return piece;
        }
    }
}
