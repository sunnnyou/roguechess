namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "FlamingHotBuff", menuName = "Game/Buffs/FlamingHotBuff")]
    public class FlamingHotBuff : UpdateBuff
    {
        public string BuffName { get; set; }

        public string Description { get; set; }

        public Sprite Icon { get; set; }

        public int Cost { get; set; }

        public bool WasUsed { get; set; }

        public FlamingHotBuff()
        {
            this.UpdateFunction = this.FlamingHotFnc;
        }

        public IChessObject FlamingHotFnc(IChessObject chessObject, ChessBoard board)
        {
            if (chessObject is not ChessPiece piece || piece == null || board == null)
            {
                Debug.LogError("Invalid arguments for FlamingHot buff.");
                return null;
            }

            // TODO:
            return null;
        }
    }
}
