namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "GoldenGooseBuff", menuName = "Game/Buffs/GoldenGooseBuff")]
    public class GoldenGooseBuff : UpdateBuff
    {
        public new string BuffName { get; set; }
        public new string Description { get; set; }
        public new Sprite Icon { get; set; }
        public new int Cost { get; set; }
        public new bool WasUsed { get; set; }

        public GoldenGooseBuff()
        {
            this.UpdateFunction = this.GoldenGooseFnc;
        }

        public IChessObject GoldenGooseFnc(IChessObject chessObject)
        {
            if (chessObject is not ChessPiece piece || piece == null)
            {
                Debug.LogError("Invalid arguments for GoldenGoose buff.");
                return null;
            }

            // TODO:
            return null;
        }
    }
}
