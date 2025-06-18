namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "SkillIssueBuff", menuName = "Game/Buffs/SkillIssueBuff")]
    public class SkillIssueBuff : UpdateBuff
    {
        public string BuffName { get; set; }

        public string Description { get; set; }

        public Sprite Icon { get; set; }

        public int Cost { get; set; }

        public bool WasUsed { get; set; }

        public SkillIssueBuff()
        {
            this.UpdateFunction = this.SkillIssueFnc;
        }

        public IChessObject SkillIssueFnc(IChessObject chessObject, ChessBoard board)
        {
            if (chessObject is not ChessPiece piece || piece == null || board == null)
            {
                Debug.LogError("Invalid arguments for SkillIssue buff.");
                return null;
            }

            // TODO:
            return null;
        }
    }
}
