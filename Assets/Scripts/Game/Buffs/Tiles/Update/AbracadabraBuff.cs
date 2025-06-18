namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "AbracadabraBuff", menuName = "Game/Buffs/AbracadabraBuff")]
    public class AbracadabraBuff : UpdateBuff
    {
        public string BuffName { get; set; }

        public string Description { get; set; }

        public Sprite Icon { get; set; }

        public int Cost { get; set; }

        public bool WasUsed { get; set; }

        public AbracadabraBuff()
        {
            this.UpdateFunction = this.AbracadabraFnc;
        }

        public IChessObject AbracadabraFnc(IChessObject chessObject, ChessBoard board)
        {
            if (chessObject is not ChessPiece piece || piece == null || board == null)
            {
                Debug.LogError("Invalid arguments for Abracadabra buff.");
                return null;
            }

            // TODO:
            return null;
        }
    }
}
