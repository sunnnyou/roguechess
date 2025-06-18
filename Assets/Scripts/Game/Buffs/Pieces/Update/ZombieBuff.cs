namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "ZombieBuff", menuName = "Game/Buffs/ZombieBuff")]
    public class ZombieBuff : UpdateBuff
    {
        public string BuffName { get; set; }

        public string Description { get; set; }

        public Sprite Icon { get; set; }

        public int Cost { get; set; }

        public bool WasUsed { get; set; }

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
