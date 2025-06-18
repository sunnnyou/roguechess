namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "AuraBuff", menuName = "Game/Buffs/AuraBuff")]
    public class AuraBuff : UpdateBuff
    {
        public string BuffName { get; set; }

        public string Description { get; set; }

        public Sprite Icon { get; set; }

        public int Cost { get; set; }

        public bool WasUsed { get; set; }

        public AuraBuff()
        {
            this.UpdateFunction = this.AuraBuffFnc;
        }

        public IChessObject AuraBuffFnc(IChessObject chessObject, ChessBoard board)
        {
            if (chessObject is not ChessPiece piece || piece == null || board == null)
            {
                Debug.LogError("Invalid arguments for Aura buff.");
                return null;
            }

            // TODO:
            return null;
        }
    }
}
