namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "ZoomiesBuff", menuName = "Game/Buffs/ZoomiesBuff")]
    public class ZoomiesBuff : UpdateBuff
    {
        public new string BuffName { get; set; }
        public new string Description { get; set; }
        public new Sprite Icon { get; set; }
        public new int Cost { get; set; }
        public new bool WasUsed { get; set; }

        public ZoomiesBuff()
        {
            this.UpdateFunction = this.ZoomiesFnc;
        }

        public IChessObject ZoomiesFnc(IChessObject chessObject)
        {
            if (chessObject is not ChessPiece piece || piece == null)
            {
                Debug.LogError("Invalid arguments for Zoomies buff.");
                return null;
            }

            // TODO:
            return null;
        }
    }
}
