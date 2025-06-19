namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "AdolescenceBuff", menuName = "Game/Buffs/AdolescenceBuff")]
    public class AdolescenceBuff : UpdateBuff
    {
        public new string BuffName { get; set; }
        public new string Description { get; set; }
        public new Sprite Icon { get; set; }
        public new int Cost { get; set; }
        public new bool WasUsed { get; set; }

        public AdolescenceBuff()
        {
            this.UpdateFunction = this.AdolescenceBuffFnc;
        }

        public IChessObject AdolescenceBuffFnc(IChessObject chessObject, ChessBoard board)
        {
            if (chessObject is not ChessPiece piece || piece == null || board == null)
            {
                Debug.LogError("Invalid arguments for Adolescence buff.");
                return null;
            }

            // TODO:
            return null;
        }
    }
}
