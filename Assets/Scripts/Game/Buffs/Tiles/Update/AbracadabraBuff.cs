namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "AbracadabraBuff", menuName = "Game/Buffs/AbracadabraBuff")]
    public class AbracadabraBuff : UpdateBuff
    {
        public new string BuffName { get; set; }

        public new string Description { get; set; }

        public new Sprite Icon { get; set; }

        public new int Cost { get; set; }

        public new bool WasUsed { get; set; }

        public AbracadabraBuff()
        {
            this.UpdateFunction = this.AbracadabraFnc;
        }

        public IChessObject AbracadabraFnc(IChessObject chessObject)
        {
            if (chessObject is not ChessPiece piece || piece == null)
            {
                Debug.LogError("Invalid arguments for Abracadabra buff.");
                return null;
            }

            // TODO:
            return null;
        }
    }
}
