namespace Assets.Scripts.Game.Buffs.Player
{
    using System.Collections.Generic;
    using System.Linq;
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "AdolescenceBuff", menuName = "Game/Buffs/AdolescenceBuff")]
    public class AdolescenceBuff : UpdateBuff
    {
        public AdolescenceBuff()
        {
            this.UpdateFunction = AdolescenceBuffFnc;
        }

        public static IChessObject AdolescenceBuffFnc(IChessObject chessObject)
        {
            if (chessObject is not ChessPiece piece || piece == null)
            {
                Debug.LogError("Invalid arguments for Adolescence buff.");
                return chessObject;
            }

            var queenSprite = piece.IsWhite
                ? ChessBoard.Instance.WhiteQueenSprite
                : ChessBoard.Instance.BlackQueenSprite;

            piece.SetPieceType(ChessPieceType.Queen);
            piece.SpriteRenderer.sprite = queenSprite;
            piece.MoveRules.Clear();
            piece.MoveRules.AddRange(MoveRules.QueenMove.GetMoveRules());

            return piece;
        }
    }
}
