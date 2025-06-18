namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "SnapBuff", menuName = "Game/Buffs/SnapBuff")]
    public class SnapBuff : UpdateBuff
    {
        public string BuffName { get; set; }

        public string Description { get; set; }

        public Sprite Icon { get; set; }

        public int Cost { get; set; }

        public bool WasUsed { get; set; }

        public SnapBuff()
        {
            this.UpdateFunction = this.SnapFnc;
        }

        public IChessObject SnapFnc(IChessObject chessObject, ChessBoard board)
        {
            if (chessObject is not ChessPiece piece || piece == null || board == null)
            {
                Debug.LogError("Invalid arguments for Snap buff.");
                return null;
            }

            // TODO:
            return null;
        }
    }
}
