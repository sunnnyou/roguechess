namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "AdolescenceBuff", menuName = "Game/Buffs/AdolescenceBuff")]
    public class AdolescenceBuff : UpdateBuff
    {
        public string BuffName { get; set; }

        public string Description { get; set; }

        public Sprite Icon { get; set; }

        public int Cost { get; set; }

        public bool WasUsed { get; set; }

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
