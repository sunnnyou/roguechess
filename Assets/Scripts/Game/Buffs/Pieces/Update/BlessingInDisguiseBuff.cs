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
