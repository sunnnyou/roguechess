namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "LoneWolfBuff", menuName = "Game/Buffs/LoneWolfBuff")]
    public class LoneWolfBuff : UpdateBuff
    {
        private readonly int addedDamageAmount = 2;

        private bool addedDamage;

        public LoneWolfBuff()
        {
            this.UpdateFunction = this.LoneWolfFnc;
        }

        public IChessObject LoneWolfFnc(IChessObject chessObject)
        {
            if (chessObject is not ChessPiece piece || piece == null)
            {
                Debug.LogError("Invalid arguments for LoneWolf buff.");
                return null;
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
                    && piece.IsWhite == tile.CurrentPiece.IsWhite
                )
                {
                    if (this.addedDamage)
                    {
                        piece.AddReduceStrength(-this.addedDamageAmount, true);
                        this.addedDamage = false;
                    }

                    return piece;
                }
            }

            if (!this.addedDamage)
            {
                piece.AddReduceStrength(this.addedDamageAmount, true);
                this.addedDamage = true;
            }

            return piece;
        }
    }
}
