namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "ZombieBuff", menuName = "Game/Buffs/ZombieBuff")]
    public class ZombieBuff : UpdateBuff
    {
        public new string BuffName { get; set; }
        public new string Description { get; set; }
        public new Sprite Icon { get; set; }
        public new int Cost { get; set; }
        public new bool WasUsed { get; set; }

        public ZombieBuff()
        {
            this.UpdateFunction = this.ZombieFnc;
        }

        public IChessObject ZombieFnc(IChessObject chessObject, ChessBoard board)
        {
            if (chessObject is not ChessPiece piece || piece == null || board == null)
            {
                Debug.LogError("Invalid arguments for Zombie buff.");
                return null;
            }

            // TODO:
            return null;
        }
    }
}
