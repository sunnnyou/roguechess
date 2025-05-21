using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChessPiece : MonoBehaviour
{
    public ChessPieceType PieceType;
    public bool IsWhite;
    public Sprite Sprite;
    public ChessTile CurrentTile;
    public List<MoveRule> MoveRules = new List<MoveRule>();
    public Material PieceMaterial;

    private SpriteRenderer spriteRenderer;
    private ChessBoard board;

    public void Initialize(
        ChessPieceType type,
        bool white,
        Sprite sprite,
        ChessBoard chessBoard,
        Material material = null
    )
    {
        this.PieceType = type;
        this.IsWhite = white;
        this.Sprite = sprite;
        this.board = chessBoard;

        // Check if we already have a SpriteRenderer
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
        if (this.spriteRenderer == null)
        {
            this.spriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();
        }

        if (material != null)
        {
            this.PieceMaterial = new Material(material); // Get copy of material so that material texture is that of this sprite
            this.PieceMaterial.renderQueue = 2999; // lower value so that its rendered behind sprite
            this.spriteRenderer.materials = this
                .spriteRenderer.materials.Append(this.PieceMaterial)
                .ToArray();
        }

        this.spriteRenderer.sortingOrder = 10; // render above tiles
        this.spriteRenderer.sprite = sprite;

        // Set default move rules based on piece type
        this.SetDefaultMoveRules();
    }

    public void SetDefaultMoveRules()
    {
        // TODO: make this a dictionary or special class for each piece type
        this.MoveRules.Clear();

        switch (this.PieceType)
        {
            case ChessPieceType.Pawn:
                // Forward movement (will need special handling for first move and capture)
                this.MoveRules.Add(
                    new MoveRule
                    {
                        XDirection = 0,
                        YDirection = this.IsWhite ? 1 : -1,
                        MaxDistance = 1,
                    }
                );
                break;

            case ChessPieceType.Rook:
                // Horizontal and vertical movement
                this.MoveRules.Add(
                    new MoveRule
                    {
                        XDirection = 1,
                        YDirection = 0,
                        MaxDistance = int.MaxValue,
                    }
                );
                this.MoveRules.Add(
                    new MoveRule
                    {
                        XDirection = -1,
                        YDirection = 0,
                        MaxDistance = int.MaxValue,
                    }
                );
                this.MoveRules.Add(
                    new MoveRule
                    {
                        XDirection = 0,
                        YDirection = 1,
                        MaxDistance = int.MaxValue,
                    }
                );
                this.MoveRules.Add(
                    new MoveRule
                    {
                        XDirection = 0,
                        YDirection = -1,
                        MaxDistance = int.MaxValue,
                    }
                );
                break;

            case ChessPieceType.Knight:
                // L-shaped movement
                this.MoveRules.Add(
                    new MoveRule
                    {
                        XDirection = 1,
                        YDirection = 2,
                        CanJumpOverPieces = true,
                    }
                );
                this.MoveRules.Add(
                    new MoveRule
                    {
                        XDirection = 2,
                        YDirection = 1,
                        CanJumpOverPieces = true,
                    }
                );
                this.MoveRules.Add(
                    new MoveRule
                    {
                        XDirection = 2,
                        YDirection = -1,
                        CanJumpOverPieces = true,
                    }
                );
                this.MoveRules.Add(
                    new MoveRule
                    {
                        XDirection = 1,
                        YDirection = -2,
                        CanJumpOverPieces = true,
                    }
                );
                this.MoveRules.Add(
                    new MoveRule
                    {
                        XDirection = -1,
                        YDirection = -2,
                        CanJumpOverPieces = true,
                    }
                );
                this.MoveRules.Add(
                    new MoveRule
                    {
                        XDirection = -2,
                        YDirection = -1,
                        CanJumpOverPieces = true,
                    }
                );
                this.MoveRules.Add(
                    new MoveRule
                    {
                        XDirection = -2,
                        YDirection = 1,
                        CanJumpOverPieces = true,
                    }
                );
                this.MoveRules.Add(
                    new MoveRule
                    {
                        XDirection = -1,
                        YDirection = 2,
                        CanJumpOverPieces = true,
                    }
                );
                break;

            case ChessPieceType.Bishop:
                // Diagonal movement
                this.MoveRules.Add(
                    new MoveRule
                    {
                        XDirection = 1,
                        YDirection = 1,
                        MaxDistance = int.MaxValue,
                    }
                );
                this.MoveRules.Add(
                    new MoveRule
                    {
                        XDirection = 1,
                        YDirection = -1,
                        MaxDistance = int.MaxValue,
                    }
                );
                this.MoveRules.Add(
                    new MoveRule
                    {
                        XDirection = -1,
                        YDirection = 1,
                        MaxDistance = int.MaxValue,
                    }
                );
                this.MoveRules.Add(
                    new MoveRule
                    {
                        XDirection = -1,
                        YDirection = -1,
                        MaxDistance = int.MaxValue,
                    }
                );
                break;

            case ChessPieceType.Queen:
                // Combination of Rook and Bishop movement
                this.MoveRules.Add(
                    new MoveRule
                    {
                        XDirection = 1,
                        YDirection = 0,
                        MaxDistance = int.MaxValue,
                    }
                );
                this.MoveRules.Add(
                    new MoveRule
                    {
                        XDirection = -1,
                        YDirection = 0,
                        MaxDistance = int.MaxValue,
                    }
                );
                this.MoveRules.Add(
                    new MoveRule
                    {
                        XDirection = 0,
                        YDirection = 1,
                        MaxDistance = int.MaxValue,
                    }
                );
                this.MoveRules.Add(
                    new MoveRule
                    {
                        XDirection = 0,
                        YDirection = -1,
                        MaxDistance = int.MaxValue,
                    }
                );
                this.MoveRules.Add(
                    new MoveRule
                    {
                        XDirection = 1,
                        YDirection = 1,
                        MaxDistance = int.MaxValue,
                    }
                );
                this.MoveRules.Add(
                    new MoveRule
                    {
                        XDirection = 1,
                        YDirection = -1,
                        MaxDistance = int.MaxValue,
                    }
                );
                this.MoveRules.Add(
                    new MoveRule
                    {
                        XDirection = -1,
                        YDirection = 1,
                        MaxDistance = int.MaxValue,
                    }
                );
                this.MoveRules.Add(
                    new MoveRule
                    {
                        XDirection = -1,
                        YDirection = -1,
                        MaxDistance = int.MaxValue,
                    }
                );
                break;

            case ChessPieceType.King:
                // One square in any direction
                this.MoveRules.Add(new MoveRule { XDirection = 1, YDirection = 0 });
                this.MoveRules.Add(new MoveRule { XDirection = -1, YDirection = 0 });
                this.MoveRules.Add(new MoveRule { XDirection = 0, YDirection = 1 });
                this.MoveRules.Add(new MoveRule { XDirection = 0, YDirection = -1 });
                this.MoveRules.Add(new MoveRule { XDirection = 1, YDirection = 1 });
                this.MoveRules.Add(new MoveRule { XDirection = 1, YDirection = -1 });
                this.MoveRules.Add(new MoveRule { XDirection = -1, YDirection = 1 });
                this.MoveRules.Add(new MoveRule { XDirection = -1, YDirection = -1 });
                break;

            case ChessPieceType.Custom:
                // Custom pieces start with no move rules
                break;
        }
    }

    public void CustomizeMoveRules(List<MoveRule> newRules)
    {
        this.MoveRules = newRules;
    }

    public List<ChessTile> GetValidMoves()
    {
        List<ChessTile> validMoves = new List<ChessTile>();

        if (this.CurrentTile == null || this.board == null)
        {
            return validMoves;
        }

        // Parse current position
        ChessBoard.ParseCoordinate(this.CurrentTile.Coordinate, out int currentX, out int currentY);

        foreach (MoveRule rule in this.MoveRules)
        {
            for (int distance = 1; distance <= rule.MaxDistance; distance++)
            {
                int targetX = currentX + (rule.XDirection * distance);
                int targetY = currentY + (rule.YDirection * distance);

                // Check if target is within board bounds
                if (
                    targetX < 0
                    || targetX >= this.board.Width
                    || targetY < 0
                    || targetY >= this.board.Height
                )
                {
                    break;
                }

                string targetCoord = ChessBoard.GetCoordinateFromPosition(targetX, targetY);
                ChessTile targetTile = this.board.GetTile(targetCoord);

                if (targetTile == null)
                {
                    break;
                }

                // If there's a piece on this tile
                if (targetTile.CurrentPiece != null)
                {
                    // If it's an enemy piece, we can capture it
                    if (targetTile.CurrentPiece.IsWhite != this.IsWhite)
                    {
                        validMoves.Add(targetTile);
                    }

                    // If we can't jump over pieces, stop checking this direction
                    if (!rule.CanJumpOverPieces)
                    {
                        break;
                    }
                }
                else
                {
                    // Empty tile, valid move
                    validMoves.Add(targetTile);
                }
            }
        }

        // Special handling for pawns (capturing diagonally)
        // TODO: fix only diagonal capture for pawns not straight
        if (this.PieceType != ChessPieceType.Pawn)
        {
            return validMoves;
        }

        // Diagonal captures
        int[] captureDirections = { -1, 1 };
        int forwardDirection = this.IsWhite ? 1 : -1;

        foreach (int dir in captureDirections)
        {
            int targetX = currentX + dir;
            int targetY = currentY + forwardDirection;

            if (
                targetX < 0
                || targetX >= this.board.Width
                || targetY < 0
                || targetY >= this.board.Height
            )
            {
                continue;
            }

            string targetCoord = ChessBoard.GetCoordinateFromPosition(targetX, targetY);
            ChessTile targetTile = this.board.GetTile(targetCoord);

            if (
                targetTile != null
                && targetTile.CurrentPiece != null
                && targetTile.CurrentPiece.IsWhite != this.IsWhite
            )
            {
                validMoves.Add(targetTile);
            }
        }

        // First move can be 2 squares forward
        if ((!this.IsWhite || currentY != 1) && (this.IsWhite || currentY != this.board.Height - 2))
        {
            return validMoves;
        }

        int twoForward = currentY + (forwardDirection * 2);
        if (twoForward < 0 || twoForward >= this.board.Height)
        {
            return validMoves;
        }

        string oneStepCoord = ChessBoard.GetCoordinateFromPosition(
            currentX,
            currentY + forwardDirection
        );
        string twoStepCoord = ChessBoard.GetCoordinateFromPosition(currentX, twoForward);

        ChessTile oneStepTile = this.board.GetTile(oneStepCoord);
        ChessTile twoStepTile = this.board.GetTile(twoStepCoord);

        if (
            oneStepTile != null
            && oneStepTile.CurrentPiece == null
            && twoStepTile != null
            && twoStepTile.CurrentPiece == null
        )
        {
            validMoves.Add(twoStepTile);
        }

        return validMoves;
    }
}
