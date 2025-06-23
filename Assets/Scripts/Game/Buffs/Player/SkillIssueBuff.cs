namespace Assets.Scripts.Game.Buffs.Player
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "SkillIssueBuff", menuName = "Game/Buffs/SkillIssueBuff")]
    public class SkillIssueBuff : UpdateBuff
    {
        public new string BuffName { get; set; }
        public new string Description { get; set; }
        public new Sprite Icon { get; set; }
        public new int Cost { get; set; }
        public new bool WasUsed { get; set; }

        public SkillIssueBuff()
        {
            this.UpdateFunction = this.SkillIssueFnc;
        }

        public IChessObject SkillIssueFnc(IChessObject chessObject)
        {
            if (chessObject is not ChessPiece piece || piece == null)
            {
                Debug.LogError("Invalid arguments for SkillIssue buff.");
                return null;
            }

            // TODO:
            return null;
        }
    }
}
