namespace Assets.Scripts.Game.Buffs.Pieces.Update
{
    using System.Collections.Generic;
    using Assets.Scripts.Game.Board;
    using Assets.Scripts.UI;
    using UnityEngine;

    [CreateAssetMenu(
        fileName = "PromoteAtEndPieceBuff",
        menuName = "Game/Buffs/PromoteAtEndPieceBuff"
    )]
    public class PromoteAtEndPieceBuff : UpdateBuff
    {
        private readonly int? promoteAtX;
        private readonly int? promoteAtY;

        // Reference to the generic selection UI manager
        public static SelectionUIManager SelectionUIManager { get; private set; }

        // Promotion configuration - can be set statically or per instance
        public static List<IChessObject> PromotionPieces { get; private set; } = new();

        public static List<string> PromotionTooltips { get; private set; } = new();

        public static string PromotionTitle { get; set; } = "Royal Ascension";

        public static string PromotionDescription { get; set; } = "Choose a piece to promote to";

        public static string PromotionConfirmText { get; set; } = "Promote";

        public PromoteAtEndPieceBuff(int? promoteAtX, int? promoteAtY)
        {
            this.promoteAtX = promoteAtX;
            this.promoteAtY = promoteAtY;
            this.UpdateFunction = this.RoyalAscension;
        }

        // Initialize the promotion system with the UI manager
        public static void InitializePromotionSystem(SelectionUIManager uiManager)
        {
            SelectionUIManager = uiManager;
        }

        // Set up promotion pieces and their display information
        public static void ConfigurePromotionPieces(
            List<IChessObject> pieces,
            List<string> tooltips = null,
            string title = "Royal Ascension",
            string description = "Choose a piece to promote to:",
            string confirmText = "Promote"
        )
        {
            PromotionPieces = pieces ?? new List<IChessObject>();
            PromotionTooltips = tooltips ?? new List<string>();
            PromotionTitle = title;
            PromotionDescription = description;
            PromotionConfirmText = confirmText;
        }

        public IChessObject RoyalAscension(IChessObject chessObject)
        {
            if (chessObject is not ChessPiece piece || piece == null)
            {
                Debug.LogError("Invalid arguments for Royal Ascension buff.");
                return null;
            }

            if (
                (this.promoteAtX != null && piece.CurrentTile.Position.x != this.promoteAtX)
                || (this.promoteAtY != null && piece.CurrentTile.Position.y != this.promoteAtY)
            )
            {
                // Piece has not reached the promotion position
                return null;
            }

            // Check if piece has reached the end of the board
            if (HasReachedEndOfBoard(piece))
            {
                Debug.Log(
                    $"Piece {piece.name} reached the end of the board and is eligible for promotion."
                );

                // Trigger promotion UI using the selection manager
                RequestPromotion(piece);

                return null; // piece update and promotion will happen via callback
            }

            return null;
        }

        // Checks if the piece has reached the end of the board based on its color and position
        private static bool HasReachedEndOfBoard(ChessPiece piece)
        {
            // For white pieces, end of board is typically rank 8 (y = 7 in 0-indexed)
            // For black pieces, end of board is typically rank 1 (y = 0 in 0-indexed)
            if (piece.IsWhite)
            {
                return piece.CurrentTile.Position.y == ChessBoard.Instance.Height - 1; // Top of board for white
            }
            else
            {
                return piece.CurrentTile.Position.y == 0; // Bottom of board for black
            }
        }

        // Requests promotion using the generic selection UI
        private static void RequestPromotion(ChessPiece piece)
        {
            if (SelectionUIManager == null)
            {
                Debug.LogError(
                    "GenericSelectionUIManager not initialized! Call InitializePromotionSystem() first."
                );
                return;
            }

            if (PromotionPieces == null || PromotionPieces.Count == 0)
            {
                Debug.LogError(
                    "No promotion pieces configured! Call ConfigurePromotionPieces() first."
                );
                return;
            }

            // Show the generic selection UI for promotion
            SelectionUIManager.ShowSelectionUI(
                PromotionPieces,
                (selectedPiece) => OnPromotionSelected(piece, selectedPiece),
                PromotionTitle,
                PromotionDescription,
                PromotionTooltips.Count > 0 ? PromotionTooltips : null,
                PromotionConfirmText,
                500f, // width
                400f, // height
                null, // position (center)
                1f // scale
            );
        }

        // Handles the promotion selection callback
        private static void OnPromotionSelected(ChessPiece oldPiece, IChessObject selectedPiece)
        {
            if (selectedPiece == null)
            {
                Debug.LogError("No piece selected for promotion.");
                return;
            }

            if (selectedPiece is not ChessPiece)
            {
                Debug.LogError("Selected Piece is not of type ChessPiece");
            }

            // Complete the promotion
            CompletePromotion(oldPiece, (ChessPiece)selectedPiece);
        }

        // Completes the promotion by replacing the piece on the board
        private static void CompletePromotion(ChessPiece oldPiece, ChessPiece newPiece)
        {
            if (oldPiece == null || oldPiece.CurrentTile == null || newPiece == null)
            {
                Debug.LogError("Cannot complete promotion: invalid pieces or tile.");
                return;
            }

            var currentTile = oldPiece.CurrentTile;

            // Remove old piece from tile
            currentTile.CurrentPiece = null;

            // Set up new piece
            newPiece.CurrentTile = currentTile;
            newPiece.IsWhite = oldPiece.IsWhite;
            newPiece.transform.position = oldPiece.transform.position;

            // Place new piece on tile
            currentTile.CurrentPiece = newPiece;

            // Disable old piece, Enable new Piece
            oldPiece.gameObject.SetActive(false);
            newPiece.gameObject.SetActive(true);

            Debug.Log($"Promotion completed: {oldPiece.name} -> {newPiece.name}");
        }
    }
}
