#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Toolnity.Games
{
    public partial class Minesweeper : EditorWindow
    {
        private enum GameState
        {
            MainScreen,
            Playing,
            Finished,
            Ranking
        }
        
        private static Minesweeper instance;
        private static GameState gameState;

        [MenuItem("Tools/Toolnity/Games/Minesweeper", priority = 3500)]
        public static void ShowWindow()
        {
            instance = GetWindow<Minesweeper>("Minesweeper");
            var texture = (Texture)Resources.Load("Mine Icon");
            instance.titleContent = new GUIContent("Minesweeper", texture);
            instance.Show();
            
            SetState(GameState.MainScreen);
        }

        private static Minesweeper GetInstance()
        {
            if (instance == null)
            {
                instance = GetWindow<Minesweeper>("Minesweeper");
            }

            return instance;
        }

        private static void SetState(GameState newState)
        {
            gameState = newState;

            switch (gameState)
            {
                case GameState.MainScreen:
                    OnEnterMainScreenState();
                    break;
                case GameState.Playing:
                    OnEnterPlayingState();
                    break;
                case GameState.Finished:
                    break;
                case GameState.Ranking:
                    OnEnterRankingState();
                    break;
            }
        }
        
        private void Update()
        {
            switch (gameState)
            {
                case GameState.MainScreen:
                    break;
                case GameState.Playing:
                    UpdatePlayingState();
                    break;
                case GameState.Finished:
                    break;
                case GameState.Ranking:
                    break;
            }
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            switch (gameState)
            {
                case GameState.MainScreen:
                    DrawMainScreenState();
                    break;
                case GameState.Playing:
                    DrawPlayingState();
                    break;
                case GameState.Finished:
                    DrawFinishedState();
                    break;
                case GameState.Ranking:
                    DrawRankingState();
                    break;
            }
            
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}
#endif