namespace Assets.Scripts.Game.Buffs.Tiles.Update
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "ExtinctionEventBuff", menuName = "Game/Buffs/ExtinctionEventBuff")]
    public class ExtinctionEventBuff : UpdateBuff
    {
        private int damageAmount = 1;

        public ExtinctionEventBuff()
        {
            this.UpdateFunction = this.ExtinctionEventFnc;
        }

        public IChessObject ExtinctionEventFnc(IChessObject chessObject)
        {
            if (chessObject is not ChessTile tile || tile == null)
            {
                Debug.LogError("Invalid arguments for UnoReversTileFnc buff.");
                return null;
            }

            var surroundingPos = CoordinateHelper.GetSurroundingCoordinatesWithBounds(
                tile.Position.x,
                tile.Position.y
            );

            foreach (var (x, y) in surroundingPos)
            {
                if (
                    ChessBoard.Instance.GetTile(
                        CoordinateHelper.XYToString(x, y),
                        out ChessTile currentTile
                    ) && currentTile.CurrentPiece.gameObject.activeSelf
                )
                {
                    currentTile.CurrentPiece.AddReduceLives(-this.damageAmount, true);
                }
            }
            tile.CurrentPiece.AddReduceLives(-this.damageAmount, true);

            this.WasUsed = true;

            return null;
        }
    }
}
