// Editor for the chess board to allow for easy setup in Unity Inspector
using Unity;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ChessBoard))]
public class ChessBoardEditor : Editor
{
    SerializedProperty widthProperty;
    SerializedProperty heightProperty;
    SerializedProperty tileSizeProperty;
    SerializedProperty whiteTileSpriteProperty;
    SerializedProperty blackTileSpriteProperty;

    // Piece sprite properties
    SerializedProperty whitePawnSpriteProperty;
    SerializedProperty whiteRookSpriteProperty;
    SerializedProperty whiteKnightSpriteProperty;
    SerializedProperty whiteBishopSpriteProperty;
    SerializedProperty whiteQueenSpriteProperty;
    SerializedProperty whiteKingSpriteProperty;
    SerializedProperty blackPawnSpriteProperty;
    SerializedProperty blackRookSpriteProperty;
    SerializedProperty blackKnightSpriteProperty;
    SerializedProperty blackBishopSpriteProperty;
    SerializedProperty blackQueenSpriteProperty;
    SerializedProperty blackKingSpriteProperty;

    private void OnEnable()
    {
        widthProperty = serializedObject.FindProperty("width");
        heightProperty = serializedObject.FindProperty("height");
        tileSizeProperty = serializedObject.FindProperty("tileSize");
        whiteTileSpriteProperty = serializedObject.FindProperty("whiteTileSprite");
        blackTileSpriteProperty = serializedObject.FindProperty("blackTileSprite");

        // Piece sprite properties
        whitePawnSpriteProperty = serializedObject.FindProperty("whitePawnSprite");
        whiteRookSpriteProperty = serializedObject.FindProperty("whiteRookSprite");
        whiteKnightSpriteProperty = serializedObject.FindProperty("whiteKnightSprite");
        whiteBishopSpriteProperty = serializedObject.FindProperty("whiteBishopSprite");
        whiteQueenSpriteProperty = serializedObject.FindProperty("whiteQueenSprite");
        whiteKingSpriteProperty = serializedObject.FindProperty("whiteKingSprite");
        blackPawnSpriteProperty = serializedObject.FindProperty("blackPawnSprite");
        blackRookSpriteProperty = serializedObject.FindProperty("blackRookSprite");
        blackKnightSpriteProperty = serializedObject.FindProperty("blackKnightSprite");
        blackBishopSpriteProperty = serializedObject.FindProperty("blackBishopSprite");
        blackQueenSpriteProperty = serializedObject.FindProperty("blackQueenSprite");
        blackKingSpriteProperty = serializedObject.FindProperty("blackKingSprite");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Board Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(widthProperty);
        EditorGUILayout.PropertyField(heightProperty);
        EditorGUILayout.PropertyField(tileSizeProperty);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Tile Sprites", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(whiteTileSpriteProperty);
        EditorGUILayout.PropertyField(blackTileSpriteProperty);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("White Piece Sprites", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(whitePawnSpriteProperty);
        EditorGUILayout.PropertyField(whiteRookSpriteProperty);
        EditorGUILayout.PropertyField(whiteKnightSpriteProperty);
        EditorGUILayout.PropertyField(whiteBishopSpriteProperty);
        EditorGUILayout.PropertyField(whiteQueenSpriteProperty);
        EditorGUILayout.PropertyField(whiteKingSpriteProperty);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Black Piece Sprites", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(blackPawnSpriteProperty);
        EditorGUILayout.PropertyField(blackRookSpriteProperty);
        EditorGUILayout.PropertyField(blackKnightSpriteProperty);
        EditorGUILayout.PropertyField(blackBishopSpriteProperty);
        EditorGUILayout.PropertyField(blackQueenSpriteProperty);
        EditorGUILayout.PropertyField(blackKingSpriteProperty);

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space();
        ChessBoard board = (ChessBoard)target;

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
