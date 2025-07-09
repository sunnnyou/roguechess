namespace Assets.Scripts.Game.Board
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
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
        private readonly List<BuffBase> buffs = new();
        public SpriteRenderer SpriteRenderer { get; set; }
        public ChessPieceType PieceType { get; private set; }
        public bool IsWhite { get; set; }
        public ChessTile CurrentTile { get; set; }
        public List<MoveRule> MoveRules { get; private set; } = new();
        public int Strength { get; private set; } = 1;
        public int Lives { get; private set; } = 1;

        List<BuffBase> IChessObject.Buffs => this.buffs;
        public bool HasMoved;
        private Color originalColor;
        private readonly float flashDurationSeconds = 0.2f;

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

            // Sprite material
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

            // Sprite other
            this.SpriteRenderer.sortingOrder =
                this.pieceData != null ? this.pieceData.SortingOrder : 10; // render above tiles
            this.SpriteRenderer.sprite = sprite;
            this.originalColor = this.SpriteRenderer.color;

            // Default move rules
            this.SetDefaultMoveRules(customRules);

            // Custom buffs
            if (buffs != null)
            {
                this.buffs.AddRange(buffs);
            }

            // Default buffs
            if (type == ChessPieceType.Pawn)
            {
                var enPassantBuff = new EnPassantPieceBuff();
                this.buffs.Add(enPassantBuff);
                var extraReachBuff = new ExtraReachPieceBuff();
                this.buffs.Add(extraReachBuff);
                var promoteAtEndPieceBuff = new PromoteAtEndPieceBuff(
                    null,
                    white ? ChessBoard.Instance.Height - 1 : 0
                );
                this.buffs.Add(promoteAtEndPieceBuff);
            }

            // Tooltip
            if (this.CurrentTile != null)
            {
                this.CurrentTile.Tooltip = this.gameObject.GetComponent<WorldTooltip>();
                if (this.CurrentTile.Tooltip == null)
                {
                    this.CurrentTile.Tooltip = this.gameObject.AddComponent<WorldTooltip>();
                }
                this.CurrentTile.Tooltip.SetTooltipText(this.GenerateTooltip());
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
            foreach (var buff in this.buffs)
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

        public static bool FightPiece(
            ChessPiece currentPiece,
            ChessPiece enemyPiece,
            bool flashColor
        )
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

                currentPiece.AddReduceLives(-enemyPiece.Strength, flashColor);
                if (currentPiece.Lives <= 0)
                {
                    // Enemy won and killed this piece
                    return true;
                }
            }

            return false;
        }

        private void DestroyPiece()
        {
            int key = this.gameObject.GetInstanceID();
            if (!ChessBoard.Instance.DownedPieces.ContainsKey(key))
            {
                ChessBoard.Instance.DownedPieces.Add(key, this);
            }

            if (this.CurrentTile != null)
            {
                Destroy(this.CurrentTile.Tooltip);
            }

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
            foreach (var buff in this.buffs)
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
            this.AddReduceLives(updatedPiece.Lives - this.Lives, true);
            this.AddReduceLives(updatedPiece.Strength - this.Strength, true);

            // Update move rules
            this.MoveRules = new List<MoveRule>(updatedPiece.MoveRules);

            // Update buffs
            this.buffs.AddRange(updatedPiece.buffs);

            // Update sprite renderer
            // TODO: Maybe add function to check if white or black
            if (this.SpriteRenderer != null && updatedPiece.SpriteRenderer != null)
            {
                this.SpriteRenderer.sprite = updatedPiece.SpriteRenderer.sprite;
            }

            // TODO: maybe add function for updating tile if position has not changed
        }

        public void AddReduceLives(int changedAmount, bool flashColor)
        {
            if (this.Lives <= 0 && changedAmount > 0)
            {
                this.RevivePiece(changedAmount);
                if (flashColor)
                {
                    this.FlashSprite(Color.green).ConfigureAwait(true);
                }
                return;
            }

            if (flashColor)
            {
                if (changedAmount >= 0)
                {
                    this.FlashSprite(Color.green).ConfigureAwait(true);
                }
                else
                {
                    this.FlashSprite(Color.red).ConfigureAwait(true);
                }
            }

            this.Lives += changedAmount;

            if (this.Lives <= 0)
            {
                this.DestroyPiece();
            }

            this.UpdateTooltip();
        }

        public void AddReduceStrength(int changedAmount, bool flashColor)
        {
            if (flashColor)
            {
                if (changedAmount >= 0)
                {
                    this.FlashSprite(Color.yellow).ConfigureAwait(true);
                }
                else
                {
                    this.FlashSprite(Color.purple).ConfigureAwait(true);
                }
            }

            this.Strength += changedAmount;

            this.UpdateTooltip();
        }

        public async Task FlashSprite(Color flashColor)
        {
            await this.FlashCoroutineAsync(flashColor).ConfigureAwait(true);
        }

        private async Task FlashCoroutineAsync(Color flashColor)
        {
            // Change to flash color
            this.SpriteRenderer.color = flashColor;

            // Wait for the flash duration
            await Task.Delay((int)(this.flashDurationSeconds * 1000)).ConfigureAwait(true);

            // Return to original color
            this.SpriteRenderer.color = this.originalColor;
        }

        public void SetPieceType(ChessPieceType newType)
        {
            this.PieceType = newType;
        }

        public string GenerateTooltip()
        {
            string buffString = string.Empty;

            if (this.buffs.Count > 0)
            {
                var buff = this.buffs.ElementAt(0);
                if (buff != null && buff.IsActive)
                {
                    buffString = buff.BuffName ?? buff.GetType().Name;
                }

                if (this.buffs.Count > 1)
                {
                    int i = 1;
                    do
                    {
                        buff = this.buffs.ElementAt(i);
                        if (buff != null && buff.IsActive)
                        {
                            buffString = buffString + "\n" + (buff.BuffName ?? buff.GetType().Name);
                        }
                        i++;
                    } while (i < this.buffs.Count);
                }
            }

            if (string.IsNullOrEmpty(buffString))
            {
                return $"HP: {this.Lives}\nStrength: {this.Strength}";
            }
            else
            {
                return $"HP: {this.Lives}\nStrength: {this.Strength}\nBuffs:\n{buffString}";
            }
        }

        internal void AddBuff(BuffBase newBuff)
        {
            this.buffs.Add(newBuff);

            this.UpdateTooltip();
        }

        public void UpdateTooltip()
        {
            if (this.CurrentTile == null)
            {
                return;
            }

            if (this.Lives > 0)
            {
                this.CurrentTile.Tooltip = this.CurrentTile.gameObject.GetComponent<WorldTooltip>();
                if (this.CurrentTile.Tooltip == null)
                {
                    this.CurrentTile.Tooltip =
                        this.CurrentTile.gameObject.AddComponent<WorldTooltip>();
                }
                this.CurrentTile.Tooltip.SetTooltipText(this.GenerateTooltip());
            }
            else
            {
                Destroy(this.CurrentTile.Tooltip);
            }
        }
    }
}
