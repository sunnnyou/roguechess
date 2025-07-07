namespace Assets.Scripts.Game.Buffs.Player.Update
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "SkillIssueBuff", menuName = "Game/Buffs/SkillIssueBuff")]
    public class SkillIssueBuff : UpdateBuff
    {
        private readonly int minStrength = 1;
        private readonly int strengthReduction = 1;

        public SkillIssueBuff()
        {
            this.UpdateFunction = this.SkillIssueFnc;
        }

        public IChessObject SkillIssueFnc(IChessObject chessObject)
        {
            var allEnemyPieces = ChessBoard.Instance.GetAllPieces(false);

            foreach (ChessPiece piece in allEnemyPieces)
            {
                if (piece.Strength > this.minStrength)
                {
                    piece.AddReduceStrength(-this.strengthReduction, true);
                }
            }
            return null;
        }
    }
}
