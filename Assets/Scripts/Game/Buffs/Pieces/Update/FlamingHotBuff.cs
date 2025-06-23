namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "FlamingHotBuff", menuName = "Game/Buffs/FlamingHotBuff")]
    public class FlamingHotBuff : UpdateBuff
    {
        public new string BuffName { get; set; }
        public new string Description { get; set; }
        public new Sprite Icon { get; set; }
        public new int Cost { get; set; }
        public new bool WasUsed { get; set; }

        public FlamingHotBuff()
        {
            this.UpdateFunction = this.FlamingHotFnc;
        }

        public IChessObject FlamingHotFnc(IChessObject chessObject)
        {
            if (chessObject is not ChessPiece piece || piece == null)
            {
                Debug.LogError("Invalid arguments for FlamingHot buff.");
                return null;
            }

            // TODO:
            return null;
        }
    }
}
