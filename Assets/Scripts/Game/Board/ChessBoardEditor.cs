// Editor for the chess board to allow for easy setup in Unity Inspector

namespace Assets.Scripts.Game.Board
{
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(ChessBoard))]
    public class ChessBoardEditor : Editor
    {
        // Size and scale properties
        public SerializedProperty WidthProperty;
        public SerializedProperty HeightProperty;
        public SerializedProperty TileSizeProperty;
        public SerializedProperty AutoScaleProperty;
        public SerializedProperty ScalePaddingProperty;

        // Tile sprite properties
        public SerializedProperty WhiteTileSpriteProperty;
        public SerializedProperty BlackTileSpriteProperty;

        // Piece sprite properties
        public SerializedProperty WhitePawnSpriteProperty;
        public SerializedProperty WhiteRookSpriteProperty;
        public SerializedProperty WhiteKnightSpriteProperty;
        public SerializedProperty WhiteBishopSpriteProperty;
        public SerializedProperty WhiteQueenSpriteProperty;
        public SerializedProperty WhiteKingSpriteProperty;
        public SerializedProperty BlackPawnSpriteProperty;
        public SerializedProperty BlackRookSpriteProperty;
        public SerializedProperty BlackKnightSpriteProperty;
        public SerializedProperty BlackBishopSpriteProperty;
        public SerializedProperty BlackQueenSpriteProperty;
        public SerializedProperty BlackKingSpriteProperty;

        // Material properties
        public SerializedProperty PieceMaterialProperty;

        // Misc
        public SerializedProperty EnemySpriteManager;
        public SerializedProperty MainFont;

        private void OnEnable()
        {
            this.WidthProperty = this.serializedObject.FindProperty("Width");
            this.HeightProperty = this.serializedObject.FindProperty("Height");
            this.TileSizeProperty = this.serializedObject.FindProperty("TileSize");
            this.AutoScaleProperty = this.serializedObject.FindProperty("AutoScale");
            this.ScalePaddingProperty = this.serializedObject.FindProperty("ScalePadding");
            this.WhiteTileSpriteProperty = this.serializedObject.FindProperty("WhiteTileSprite");
            this.BlackTileSpriteProperty = this.serializedObject.FindProperty("BlackTileSprite");

            // Piece sprite properties
            this.WhitePawnSpriteProperty = this.serializedObject.FindProperty("WhitePawnSprite");
            this.WhiteRookSpriteProperty = this.serializedObject.FindProperty("WhiteRookSprite");
            this.WhiteKnightSpriteProperty = this.serializedObject.FindProperty(
                "WhiteKnightSprite"
            );
            this.WhiteBishopSpriteProperty = this.serializedObject.FindProperty(
                "WhiteBishopSprite"
            );
            this.WhiteQueenSpriteProperty = this.serializedObject.FindProperty("WhiteQueenSprite");
            this.WhiteKingSpriteProperty = this.serializedObject.FindProperty("WhiteKingSprite");
            this.BlackPawnSpriteProperty = this.serializedObject.FindProperty("BlackPawnSprite");
            this.BlackRookSpriteProperty = this.serializedObject.FindProperty("BlackRookSprite");
            this.BlackKnightSpriteProperty = this.serializedObject.FindProperty(
                "BlackKnightSprite"
            );
            this.BlackBishopSpriteProperty = this.serializedObject.FindProperty(
                "BlackBishopSprite"
            );
            this.BlackQueenSpriteProperty = this.serializedObject.FindProperty("BlackQueenSprite");
            this.BlackKingSpriteProperty = this.serializedObject.FindProperty("BlackKingSprite");

            // Piece material properties
            this.PieceMaterialProperty = this.serializedObject.FindProperty("PieceMaterial");

            // Misc
            this.EnemySpriteManager = this.serializedObject.FindProperty("EnemySpriteManager");
            this.MainFont = this.serializedObject.FindProperty("MainFont");
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();

            EditorGUILayout.LabelField("Board Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(this.WidthProperty);
            EditorGUILayout.PropertyField(this.HeightProperty);
            EditorGUILayout.PropertyField(this.TileSizeProperty);
            EditorGUILayout.PropertyField(this.AutoScaleProperty);
            EditorGUILayout.PropertyField(this.ScalePaddingProperty);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Tile Sprites", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(this.WhiteTileSpriteProperty);
            EditorGUILayout.PropertyField(this.BlackTileSpriteProperty);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("White Piece Sprites", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(this.WhitePawnSpriteProperty);
            EditorGUILayout.PropertyField(this.WhiteRookSpriteProperty);
            EditorGUILayout.PropertyField(this.WhiteKnightSpriteProperty);
            EditorGUILayout.PropertyField(this.WhiteBishopSpriteProperty);
            EditorGUILayout.PropertyField(this.WhiteQueenSpriteProperty);
            EditorGUILayout.PropertyField(this.WhiteKingSpriteProperty);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Black Piece Sprites", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(this.BlackPawnSpriteProperty);
            EditorGUILayout.PropertyField(this.BlackRookSpriteProperty);
            EditorGUILayout.PropertyField(this.BlackKnightSpriteProperty);
            EditorGUILayout.PropertyField(this.BlackBishopSpriteProperty);
            EditorGUILayout.PropertyField(this.BlackQueenSpriteProperty);
            EditorGUILayout.PropertyField(this.BlackKingSpriteProperty);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Piece Material", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(this.PieceMaterialProperty);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Sprite Manger for Enemy", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(this.EnemySpriteManager);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Main Font", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(this.MainFont);

            this.serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            ChessBoard board = (ChessBoard)this.target;

            if (GUILayout.Button("Generate Board"))
            {
                board.GenerateBoard();
            }

            if (GUILayout.Button("Setup Traditional Pieces"))
            {
                board.SetupTraditionalPieces();
            }
        }
    }
#endif
}
