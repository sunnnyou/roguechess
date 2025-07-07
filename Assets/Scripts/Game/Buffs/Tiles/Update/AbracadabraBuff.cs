namespace Assets.Scripts.Game.Buffs.Tiles.Update
{
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "AbracadabraBuff", menuName = "Game/Buffs/AbracadabraBuff")]
    public class AbracadabraBuff : UpdateBuff
    {
        public AbracadabraBuff()
        {
            this.UpdateFunction = AbracadabraFnc;
        }

        public static IChessObject AbracadabraFnc(IChessObject chessObject)
        {
            if (chessObject is not ChessPiece piece || piece == null)
            {
                Debug.LogError("Invalid arguments for EnPassantDebuff buff.");
                return null;
            }

            if (piece.Lives == 0)
            {
                return null;
            }

            piece.AddReduceLives(-piece.Lives, true);
            return null;
        }
    }
}
