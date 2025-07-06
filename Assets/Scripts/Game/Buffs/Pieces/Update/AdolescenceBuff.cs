namespace Assets.Scripts.Game.Buffs.Player
{
    using System.Collections.Generic;
    using System.Linq;
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "AdolescenceBuff", menuName = "Game/Buffs/AdolescenceBuff")]
    public class AdolescenceBuff : UpdateBuff
    {
        public new string BuffName { get; set; }
        public new string Description { get; set; }
        public new Sprite Icon { get; set; }
        public new int Cost { get; set; }
        public new bool WasUsed { get; set; }

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

            var queen = new ChessPiece();
            queen.Initialize(
                ChessPieceType.Queen,
                piece.IsWhite,
                queenSprite,
                new List<Material>(piece.SpriteRenderer.materials),
                piece.MoveRules,
                piece.Buffs
            );

            return queen;
        }
    }
}
