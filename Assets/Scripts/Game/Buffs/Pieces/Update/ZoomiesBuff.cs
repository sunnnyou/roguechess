namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "ZoomiesBuff", menuName = "Game/Buffs/ZoomiesBuff")]
    public class ZoomiesBuff : UpdateBuff
    {
        public string BuffName { get; set; }

        public string Description { get; set; }

        public Sprite Icon { get; set; }

        public int Cost { get; set; }

        public bool WasUsed { get; set; }

        public ZoomiesBuff()
        {
            this.UpdateFunction = this.ZoomiesFnc;
        }

        public IChessObject ZoomiesFnc(IChessObject chessObject, ChessBoard board)
        {
            if (chessObject is not ChessPiece piece || piece == null || board == null)
            {
                Debug.LogError("Invalid arguments for Zoomies buff.");
                return null;
            }

            // TODO:
            return null;
        }
    }
}
