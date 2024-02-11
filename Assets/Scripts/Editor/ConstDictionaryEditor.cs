using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
    [CustomEditor(typeof(ConstDictionary))]
    public class ConstDictionaryEditor : Editor
    {
        private SerializedProperty floorTiles;
        private SerializedProperty wallTiles;
        private SerializedProperty edgeUpTiles;
        private SerializedProperty edgeLeftTiles;
        private SerializedProperty edgeRightTiles;
        private SerializedProperty edgeUpLeftTiles;
        private SerializedProperty edgeUpRightTiles;
        private SerializedProperty tileToEmptyRoom;
        private SerializedProperty nothingTiles;
        private SerializedProperty trader;
        private SerializedProperty chestClose;
        private SerializedProperty enemies;
        private SerializedProperty progressBar;
        private SerializedProperty progressBarPieces;

        private const float thumbnailSize = 64f;
        private float thumbnailsPerRow;

        private void OnEnable()
        {
            floorTiles = serializedObject.FindProperty("floorTiles");
            wallTiles = serializedObject.FindProperty("wallTiles");
            edgeUpTiles = serializedObject.FindProperty("edgeUpTiles");
            edgeLeftTiles = serializedObject.FindProperty("edgeLeftTiles");
            edgeRightTiles = serializedObject.FindProperty("edgeRightTiles");
            edgeUpLeftTiles = serializedObject.FindProperty("edgeUpLeftTiles");
            edgeUpRightTiles = serializedObject.FindProperty("edgeUpRightTiles");
            tileToEmptyRoom = serializedObject.FindProperty("tileToEmptyRoom");
            nothingTiles = serializedObject.FindProperty("nothingTiles");
            trader = serializedObject.FindProperty("trader");
            chestClose = serializedObject.FindProperty("chestClose");
            enemies = serializedObject.FindProperty("enemies");
            progressBar = serializedObject.FindProperty("progressBar");
            progressBarPieces = serializedObject.FindProperty("progressBarPieces");
        }

        private void UpdateThumbnailsPerRow()
        {
            float inspectorWidth = EditorGUIUtility.currentViewWidth;
            thumbnailsPerRow = Mathf.Floor(inspectorWidth / thumbnailSize);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            UpdateThumbnailsPerRow();

            DisplayThumbnailGrid(floorTiles);
            DisplayThumbnailGrid(wallTiles);
            DisplayThumbnailGrid(edgeUpTiles);
            DisplayThumbnailGrid(edgeLeftTiles);
            DisplayThumbnailGrid(edgeRightTiles);
            DisplayThumbnailGrid(edgeUpLeftTiles);
            DisplayThumbnailGrid(edgeUpRightTiles);
            DisplayThumbnailGrid(tileToEmptyRoom);
            DisplayThumbnailGrid(nothingTiles);
            DisplayThumbnailGrid(enemies);
            DisplayThumbnailGrid(progressBar);
            DisplayThumbnailGrid(progressBarPieces);

            EditorGUILayout.ObjectField(
                trader,
                typeof(Sprite),
                GUIContent.none,
                GUILayout.Width(thumbnailSize),
                GUILayout.Height(thumbnailSize)
                );
            EditorGUILayout.ObjectField(
                chestClose,
                typeof(Sprite),
                GUIContent.none,
                GUILayout.Width(thumbnailSize),
                GUILayout.Height(thumbnailSize)
                );

            EditorGUILayout.PropertyField(floorTiles, true);
            EditorGUILayout.PropertyField(wallTiles, true);
            EditorGUILayout.PropertyField(edgeUpTiles, true);
            EditorGUILayout.PropertyField(edgeLeftTiles, true);
            EditorGUILayout.PropertyField(edgeRightTiles, true);
            EditorGUILayout.PropertyField(edgeUpLeftTiles, true);
            EditorGUILayout.PropertyField(edgeUpRightTiles, true);
            EditorGUILayout.PropertyField(tileToEmptyRoom, true);
            EditorGUILayout.PropertyField(nothingTiles, true);
            EditorGUILayout.PropertyField(enemies, true);
            EditorGUILayout.PropertyField(progressBar, true);
            EditorGUILayout.PropertyField(progressBarPieces, true);
            serializedObject.ApplyModifiedProperties();
        }

        private void DisplayThumbnailGrid(SerializedProperty spriteArray)
        {
            int arraySize = spriteArray.arraySize;

            GUILayout.Space(10f);
            EditorGUILayout.LabelField(spriteArray.displayName, EditorStyles.boldLabel);
            GUILayout.BeginVertical();
            for (int i = 0; i < arraySize; i += (int)thumbnailsPerRow)
            {
                GUILayout.BeginHorizontal();
                for (int j = 0; j < thumbnailsPerRow && i + j < arraySize; j++)
                {
                    Texture2D thumbnail = AssetPreview.GetAssetPreview(spriteArray.GetArrayElementAtIndex(i + j).objectReferenceValue);
                    EditorGUILayout.LabelField(new GUIContent(thumbnail), GUILayout.MaxHeight(64), GUILayout.MaxWidth(64));
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
    }
}
