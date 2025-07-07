namespace Assets.Scripts.Game.Buffs.Player.Update
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(
        fileName = "InstallingUpdatesBuff",
        menuName = "Game/Buffs/InstallingUpdatesBuff"
    )]
    public class InstallingUpdatesBuff : UpdateBuff
    {
        public InstallingUpdatesBuff()
        {
            this.UpdateFunction = this.InstallingUpdatesFnc;
        }

        public IChessObject InstallingUpdatesFnc(IChessObject chessObject)
        {
            ChessBoard.Instance.SkipNextRound();
            return null;
        }
    }
}
