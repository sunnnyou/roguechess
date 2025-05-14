using System.Collections.Generic;
using UnityEngine;

public class ChessPiece : MonoBehaviour
{
    public ChessPieceType pieceType;
    public bool isWhite;
    public Sprite sprite;
    public ChessTile currentTile;
    public List<MoveRule> moveRules = new List<MoveRule>();

    private SpriteRenderer spriteRenderer;
    private ChessBoard board;

    public void Initialize(ChessPieceType type, bool white, Sprite sprite, ChessBoard chessBoard)
    {
        pieceType = type;
        isWhite = white;
        this.sprite = sprite;
        board = chessBoard;

        // Check if we already have a SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        // Explicitly set the sorting order to be higher than tiles
        spriteRenderer.sortingLayerName = "Default";
        spriteRenderer.sortingOrder = 10;

        spriteRenderer.sprite = sprite;

        // Set default move rules based on piece type
        SetDefaultMoveRules();
    }

    public void SetDefaultMoveRules()
    {
        moveRules.Clear();

        switch (pieceType)
        {
            case ChessPieceType.Pawn:
                // Forward movement (will need special handling for first move and capture)
                moveRules.Add(
                    new MoveRule
                    {
                        xDirection = 0,
                        yDirection = isWhite ? 1 : -1,
                        maxDistance = 1,
                    }
                );
                break;

            case ChessPieceType.Rook:
                // Horizontal and vertical movement
                moveRules.Add(
                    new MoveRule
                    {
                        xDirection = 1,
                        yDirection = 0,
                        maxDistance = int.MaxValue,
                    }
                );
                moveRules.Add(
                    new MoveRule
                    {
                        xDirection = -1,
                        yDirection = 0,
                        maxDistance = int.MaxValue,
                    }
                );
                moveRules.Add(
                    new MoveRule
                    {
                        xDirection = 0,
                        yDirection = 1,
                        maxDistance = int.MaxValue,
                    }
                );
                moveRules.Add(
                    new MoveRule
                    {
                        xDirection = 0,
                        yDirection = -1,
                        maxDistance = int.MaxValue,
                    }
                );
                break;

            case ChessPieceType.Knight:
                // L-shaped movement
                moveRules.Add(
                    new MoveRule
                    {
                        xDirection = 1,
                        yDirection = 2,
                        canJumpOverPieces = true,
                    }
                );
                moveRules.Add(
                    new MoveRule
                    {
                        xDirection = 2,
                        yDirection = 1,
                        canJumpOverPieces = true,
                    }
                );
                moveRules.Add(
                    new MoveRule
                    {
                        xDirection = 2,
                        yDirection = -1,
                        canJumpOverPieces = true,
                    }
                );
                moveRules.Add(
                    new MoveRule
                    {
                        xDirection = 1,
                        yDirection = -2,
                        canJumpOverPieces = true,
                    }
                );
                moveRules.Add(
                    new MoveRule
                    {
                        xDirection = -1,
                        yDirection = -2,
                        canJumpOverPieces = true,
                    }
                );
                moveRules.Add(
                    new MoveRule
                    {
                        xDirection = -2,
                        yDirection = -1,
                        canJumpOverPieces = true,
                    }
                );
                moveRules.Add(
                    new MoveRule
                    {
                        xDirection = -2,
                        yDirection = 1,
                        canJumpOverPieces = true,
                    }
                );
                moveRules.Add(
                    new MoveRule
                    {
                        xDirection = -1,
                        yDirection = 2,
                        canJumpOverPieces = true,
                    }
                );
                break;

            case ChessPieceType.Bishop:
                // Diagonal movement
                moveRules.Add(
                    new MoveRule
                    {
                        xDirection = 1,
                        yDirection = 1,
                        maxDistance = int.MaxValue,
                    }
                );
                moveRules.Add(
                    new MoveRule
                    {
                        xDirection = 1,
                        yDirection = -1,
                        maxDistance = int.MaxValue,
                    }
                );
                moveRules.Add(
                    new MoveRule
                    {
                        xDirection = -1,
                        yDirection = 1,
                        maxDistance = int.MaxValue,
                    }
                );
                moveRules.Add(
                    new MoveRule
                    {
                        xDirection = -1,
                        yDirection = -1,
                        maxDistance = int.MaxValue,
                    }
                );
                break;

            case ChessPieceType.Queen:
                // Combination of Rook and Bishop movement
                moveRules.Add(
                    new MoveRule
                    {
                        xDirection = 1,
                        yDirection = 0,
                        maxDistance = int.MaxValue,
                    }
                );
                moveRules.Add(
                    new MoveRule
                    {
                        xDirection = -1,
                        yDirection = 0,
                        maxDistance = int.MaxValue,
                    }
                );
                moveRules.Add(
                    new MoveRule
                    {
                        xDirection = 0,
                        yDirection = 1,
                        maxDistance = int.MaxValue,
                    }
                );
                moveRules.Add(
                    new MoveRule
                    {
                        xDirection = 0,
                        yDirection = -1,
                        maxDistance = int.MaxValue,
                    }
                );
                moveRules.Add(
                    new MoveRule
                    {
                        xDirection = 1,
                        yDirection = 1,
                        maxDistance = int.MaxValue,
                    }
                );
                moveRules.Add(
                    new MoveRule
                    {
                        xDirection = 1,
                        yDirection = -1,
                        maxDistance = int.MaxValue,
                    }
                );
                moveRules.Add(
                    new MoveRule
                    {
                        xDirection = -1,
                        yDirection = 1,
                        maxDistance = int.MaxValue,
                    }
                );
                moveRules.Add(
                    new MoveRule
                    {
                        xDirection = -1,
                        yDirection = -1,
                        maxDistance = int.MaxValue,
                    }
                );
                break;

            case ChessPieceType.King:
                // One square in any direction
                moveRules.Add(new MoveRule { xDirection = 1, yDirection = 0 });
                moveRules.Add(new MoveRule { xDirection = -1, yDirection = 0 });
                moveRules.Add(new MoveRule { xDirection = 0, yDirection = 1 });
                moveRules.Add(new MoveRule { xDirection = 0, yDirection = -1 });
                moveRules.Add(new MoveRule { xDirection = 1, yDirection = 1 });
                moveRules.Add(new MoveRule { xDirection = 1, yDirection = -1 });
                moveRules.Add(new MoveRule { xDirection = -1, yDirection = 1 });
                moveRules.Add(new MoveRule { xDirection = -1, yDirection = -1 });
                break;

            case ChessPieceType.Custom:
                // Custom pieces start with no move rules
                break;
        }
    }

    public void CustomizeMoveRules(List<MoveRule> newRules)
    {
        moveRules = newRules;
    }

    public List<ChessTile> GetValidMoves()
    {
        List<ChessTile> validMoves = new List<ChessTile>();

        if (currentTile == null || board == null)
            return validMoves;

        // Parse current position
        board.ParseCoordinate(currentTile.coordinate, out int currentX, out int currentY);

        foreach (MoveRule rule in moveRules)
        {
            for (int distance = 1; distance <= rule.maxDistance; distance++)
            {
                int targetX = currentX + (rule.xDirection * distance);
                int targetY = currentY + (rule.yDirection * distance);

                // Check if target is within board bounds
                if (targetX < 0 || targetX >= board.width || targetY < 0 || targetY >= board.height)
                    break;

                string targetCoord = board.GetCoordinateFromPosition(targetX, targetY);
                ChessTile targetTile = board.GetTile(targetCoord);

                if (targetTile == null)
                    break;

                // If there's a piece on this tile
                if (targetTile.currentPiece != null)
                {
                    // If it's an enemy piece, we can capture it
                    if (targetTile.currentPiece.isWhite != isWhite)
                        validMoves.Add(targetTile);

                    // If we can't jump over pieces, stop checking this direction
                    if (!rule.canJumpOverPieces)
                        break;
                }
                else
                {
                    // Empty tile, valid move
                    validMoves.Add(targetTile);
                }
            }
        }

        // Special handling for pawns (capturing diagonally)
        if (pieceType == ChessPieceType.Pawn)
        {
            // Diagonal captures
            int[] captureDirections = { -1, 1 };
            int forwardDirection = isWhite ? 1 : -1;

            foreach (int dir in captureDirections)
            {
                int targetX = currentX + dir;
                int targetY = currentY + forwardDirection;

                if (targetX >= 0 && targetX < board.width && targetY >= 0 && targetY < board.height)
                {
                    string targetCoord = board.GetCoordinateFromPosition(targetX, targetY);
                    ChessTile targetTile = board.GetTile(targetCoord);

                    if (
                        targetTile != null
                        && targetTile.currentPiece != null
                        && targetTile.currentPiece.isWhite != isWhite
                    )
                    {
                        validMoves.Add(targetTile);
                    }
                }
            }

            // First move can be 2 squares forward
            if ((isWhite && currentY == 1) || (!isWhite && currentY == board.height - 2))
            {
                int twoForward = currentY + (forwardDirection * 2);
                if (twoForward >= 0 && twoForward < board.height)
                {
                    string oneStepCoord = board.GetCoordinateFromPosition(
                        currentX,
                        currentY + forwardDirection
                    );
                    string twoStepCoord = board.GetCoordinateFromPosition(currentX, twoForward);

                    ChessTile oneStepTile = board.GetTile(oneStepCoord);
                    ChessTile twoStepTile = board.GetTile(twoStepCoord);

                    if (
                        oneStepTile != null
                        && oneStepTile.currentPiece == null
                        && twoStepTile != null
                        && twoStepTile.currentPiece == null
                    )
                    {
                        validMoves.Add(twoStepTile);
                    }
                }
            }
        }

        return validMoves;
    }
}
