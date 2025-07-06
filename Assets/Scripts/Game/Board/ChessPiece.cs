namespace Assets.Scripts.Game.Board
{
    using System.Collections.Generic;
    using System.Linq;
    using Assets.Scripts.Game.Buffs;
    using Assets.Scripts.Game.Buffs.Pieces.Move;
    using Assets.Scripts.Game.Buffs.Pieces.Update;
    using Assets.Scripts.Game.MoveRules;
    using UnityEngine;

    public class ChessPiece : MonoBehaviour, IChessObject
    {
        [SerializeField]
        private ChessPieceData pieceData;

        // Runtime properties
        public List<BuffBase> Buffs { get; } = new();
        public SpriteRenderer SpriteRenderer { get; set; }
        public ChessPieceType PieceType { get; private set; }
        public bool IsWhite { get; set; }
        public ChessTile CurrentTile { get; set; }
        public List<MoveRule> MoveRules { get; private set; } = new();
        public int Strength { get; set; } = 1;
        public int Lives { get; set; } = 1;
        public bool HasMoved;

        // Initialize from ScriptableObject data
        public void Initialize(ChessPieceData data)
        {
            if (data != null)
            {
                this.pieceData = data;
                this.Initialize(
                    data.PieceType,
                    data.IsWhite,
                    data.Sprite,
                    data.Materials,
                    data.CustomMoveRules,
                    data.InitialBuffs
                );
                this.Strength = data.Strength;
                this.Lives = data.Lives;
            }
            else
            {
                Debug.LogWarning("ChessPieceData is null!");
            }
        }

        public void Initialize(
            ChessPieceType type,
            bool white,
            Sprite sprite,
            List<Material> materials,
            List<MoveRule> customRules = null,
            List<BuffBase> buffs = null
        )
        {
            this.PieceType = type;
            this.IsWhite = white;

            // Check if we already have a SpriteRenderer
            this.SpriteRenderer = this.GetComponent<SpriteRenderer>();
            if (this.SpriteRenderer == null)
            {
                this.SpriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();
            }

            if (materials != null)
            {
                int baseRenderQueue = 3000; // default render queue value for sprites
                for (int i = 0; i < materials.Count; i++)
                {
                    var pieceMaterial = new Material(materials[i])
                    {
                        renderQueue = baseRenderQueue - (i + 1), // lower value so that its rendered behind sprite
                    };
                    this.SpriteRenderer.materials = this
                        .SpriteRenderer.materials.Append(pieceMaterial)
                        .ToArray();
                }
            }

            this.SpriteRenderer.sortingOrder =
                this.pieceData != null ? this.pieceData.SortingOrder : 10; // render above tiles
            this.SpriteRenderer.sprite = sprite;

            // Set default move rules based on piece type
            this.SetDefaultMoveRules(customRules);

            // Add buffs if provided
            if (buffs != null)
            {
                this.Buffs.AddRange(buffs);
            }

            if (type == ChessPieceType.Pawn)
            {
                var enPassantBuff = new EnPassantPieceBuff();
                this.Buffs.Add(enPassantBuff);
                var extraReachBuff = new ExtraReachPieceBuff();
                this.Buffs.Add(extraReachBuff);
                var promoteAtEndPieceBuff = new PromoteAtEndPieceBuff(
                    null,
                    white ? ChessBoard.Instance.Height - 1 : 0
                );
                this.Buffs.Add(promoteAtEndPieceBuff);
            }
        }

        public void SetDefaultMoveRules(List<MoveRule> customRules = null)
        {
            this.MoveRules.Clear();

            switch (this.PieceType)
            {
                case ChessPieceType.Pawn:
                    this.MoveRules = PawnMove.GetMoveRules(this.IsWhite);
                    break;
                case ChessPieceType.Rook:
                    this.MoveRules = RookMove.GetMoveRules();
                    break;
                case ChessPieceType.Knight:
                    this.MoveRules = KnightMove.GetMoveRules();
                    break;
                case ChessPieceType.Bishop:
                    this.MoveRules = BishopMove.GetMoveRules();
                    break;
                case ChessPieceType.Queen:
                    this.MoveRules = QueenMove.GetMoveRules();
                    break;
                case ChessPieceType.King:
                    this.MoveRules = KingMove.GetMoveRules();
                    break;
                case ChessPieceType.Custom:
                    this.MoveRules = customRules ?? PawnMove.GetMoveRules(this.IsWhite);
                    break;
            }
        }

        public List<ChessTile> GetValidTiles()
        {
            var validMoves = new List<ChessTile>();

            if (this.CurrentTile == null)
            {
                return validMoves;
            }

            validMoves = MoveRule.GetValidTiles(
                this.MoveRules,
                this.CurrentTile.Position,
                this.IsWhite
            );

            // Get valid tiles from buffs
            foreach (var buff in this.Buffs)
            {
                if (buff == null || !buff.IsActive || buff is not MoveBuff)
                {
                    continue;
                }

                if (buff.ApplyBuff(this) is List<ChessTile> buffedTiles && buffedTiles.Count > 0)
                {
                    validMoves.AddRange(buffedTiles);
                }
            }

            return validMoves;
        }

        public List<ChessMove> GetValidMoves()
        {
            var moves = new List<ChessMove>();
            var validTiles = this.GetValidTiles();

            foreach (var tile in validTiles)
            {
                moves.Add(
                    new ChessMove(
                        this.CurrentTile.Position,
                        tile.Position,
                        tile.CurrentPiece != null
                    )
                );
            }

            return moves;
        }

        public static bool FightPiece(ChessPiece currentPiece, ChessPiece enemyPiece)
        {
            if (currentPiece == null && enemyPiece != null)
            {
                return true;
            }
            else if (currentPiece != null && enemyPiece == null)
            {
                currentPiece.DestroyPiece();
                return true;
            }
            else if (currentPiece != null && enemyPiece != null)
            {
                // TODO: fight function (use buffs)

                currentPiece.Lives -= enemyPiece.Strength;
                if (currentPiece.Lives <= 0)
                {
                    // Enemy won and killed this piece
                    currentPiece.DestroyPiece();
                    return true;
                }
            }

            return false;
        }

        private void DestroyPiece()
        {
            // TODO: add capture animation and add to captured pieces list for ui
            int key = this.gameObject.GetInstanceID();
            while (ChessBoard.Instance.DownedPieces.ContainsKey(key))
            {
                key++;
            }

            ChessBoard.Instance.DownedPieces.Add(key, this.CurrentTile.Position);

            this.gameObject.SetActive(false);
        }

        public void RevivePiece(int lives = 1)
        {
            this.gameObject.SetActive(true);
            this.Lives = lives;
            ChessBoard.Instance.DownedPieces.Remove(this.gameObject.GetInstanceID());
        }

        public void UseUpdateBuffs()
        {
            foreach (var buff in this.Buffs)
            {
                if (
                    buff != null
                    && buff.IsActive
                    && buff is UpdateBuff
                    && buff.ApplyBuff(this) is ChessPiece updatedPiece
                    && updatedPiece != null
                )
                {
                    this.UpdateFromBuff(updatedPiece);
                }
            }
        }

        public void UpdateFromBuff(ChessPiece updatedPiece)
        {
            if (updatedPiece == null)
            {
                Debug.LogError("Updated piece is null, cannot apply buff.");
                return;
            }

            // Update properties from the buffed piece
            this.PieceType = updatedPiece.PieceType;
            this.IsWhite = updatedPiece.IsWhite;
            this.Lives = updatedPiece.Lives;

            // Update move rules
            this.MoveRules = new List<MoveRule>(updatedPiece.MoveRules);

            // Update buffs
            this.Buffs.Clear();
            this.Buffs.AddRange(updatedPiece.Buffs);

            // Update sprite renderer
            // TODO: Maybe add function to check if white or black
            if (this.SpriteRenderer != null)
            {
                this.SpriteRenderer.sprite = updatedPiece.SpriteRenderer.sprite;
            }

            if (this.Lives <= 0)
            {
                this.DestroyPiece();
            }

            // TODO: maybe add function for updating tile if position has not changed
        }
    }
}
