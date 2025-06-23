namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "SnapBuff", menuName = "Game/Buffs/SnapBuff")]
    public class SnapBuff : UpdateBuff
    {
        public new string BuffName { get; set; }
        public new string Description { get; set; }
        public new Sprite Icon { get; set; }
        public new int Cost { get; set; }
        public new bool WasUsed { get; set; }

        public SnapBuff()
        {
            this.UpdateFunction = this.SnapFnc;
        }

        public IChessObject SnapFnc(IChessObject chessObject)
        {
            if (chessObject is not ChessPiece piece || piece == null)
            {
                Debug.LogError("Invalid arguments for Snap buff.");
                return null;
            }

            // TODO:
            return null;
        }
    }
}
