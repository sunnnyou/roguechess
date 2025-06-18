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
        public string BuffName { get; set; }

        public string Description { get; set; }

        public Sprite Icon { get; set; }

        public int Cost { get; set; }

        public bool WasUsed { get; set; }

        public BlessingInDisguiseBuff()
        {
            this.UpdateFunction = this.BlessingInDisguiseFnc;
        }

        public IChessObject BlessingInDisguiseFnc(IChessObject chessObject, ChessBoard board)
        {
            if (chessObject is not ChessPiece piece || piece == null || board == null)
            {
                Debug.LogError("Invalid arguments for BlessingInDisguise buff.");
                return null;
            }

            // TODO:
            return null;
        }
    }
}
