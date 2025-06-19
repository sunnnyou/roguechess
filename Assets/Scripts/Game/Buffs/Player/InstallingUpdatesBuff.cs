namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(
        fileName = "InstallingUpdatesBuff",
        menuName = "Game/Buffs/InstallingUpdatesBuff"
    )]
    public class InstallingUpdatesBuff : UpdateBuff
    {
        public new string BuffName { get; set; }
        public new string Description { get; set; }
        public new Sprite Icon { get; set; }
        public new int Cost { get; set; }
        public new bool WasUsed { get; set; }

        public InstallingUpdatesBuff()
        {
            this.UpdateFunction = this.InstallingUpdatesFnc;
        }

        public IChessObject InstallingUpdatesFnc(IChessObject chessObject, ChessBoard board)
        {
            if (chessObject is not ChessPiece piece || piece == null || board == null)
            {
                Debug.LogError("Invalid arguments for InstallingUpdates buff.");
                return null;
            }

            // TODO:
            return null;
        }
    }
}
