namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "GoldenGooseBuff", menuName = "Game/Buffs/GoldenGooseBuff")]
    public class GoldenGooseBuff : UpdateBuff
    {
        public string BuffName { get; set; }

        public string Description { get; set; }

        public Sprite Icon { get; set; }

        public int Cost { get; set; }

        public bool WasUsed { get; set; }

        public GoldenGooseBuff()
        {
            this.UpdateFunction = this.GoldenGooseFnc;
        }

        public IChessObject GoldenGooseFnc(IChessObject chessObject, ChessBoard board)
        {
            if (chessObject is not ChessPiece piece || piece == null || board == null)
            {
                Debug.LogError("Invalid arguments for GoldenGoose buff.");
                return null;
            }

            // TODO:
            return null;
        }
    }
}
