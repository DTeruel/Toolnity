#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Toolnity.Games
{
    public partial class Minesweeper
    {
        private static MinesweeperRankingAsset rankingAsset;
        private static bool newBestRecord;
        private static Vector2 rankingScrollPos;
        private static GUIStyle centeredLabelStyle;
        private static GUIStyle leftLabelStyle;
        private static GUIStyle rightLabelStyle;

        private static void CheckRankingAsset()
        {
            if (rankingAsset != null)
            {
                return;
            }
            
            LoadOrCreateAsset();
        }
        
        private static void LoadOrCreateAsset()
        {
            var allAssets = Resources.LoadAll<MinesweeperRankingAsset>($"");
            if (allAssets.Length > 0)
            {
                rankingAsset = allAssets[0];
                return;
            }

            rankingAsset = CreateInstance<MinesweeperRankingAsset>();
            rankingAsset.Nicknames = new List<string>();
            rankingAsset.Scores = new List<double>();
			
            const string pathFolder = "Assets/Resources/";
            const string assetName = "Minesweeper.asset";
            if (!Directory.Exists("Assets/Resources"))
            {
                Directory.CreateDirectory("Assets/Resources");
            }
            AssetDatabase.CreateAsset(rankingAsset, pathFolder + assetName);
            AssetDatabase.SaveAssets();
        }

        private static void CheckStyles()
        {
            if (centeredLabelStyle == null)
            {
                centeredLabelStyle = new GUIStyle(GUI.skin.GetStyle("Label"))
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontStyle = FontStyle.Bold
                };
            }
            
            if (leftLabelStyle == null)
            {
                leftLabelStyle = new GUIStyle(GUI.skin.GetStyle("Label"))
                {
                    alignment = TextAnchor.MiddleLeft,
                    fontStyle = FontStyle.Normal
                };
            }
            
            if (rightLabelStyle == null)
            {
                rightLabelStyle = new GUIStyle(GUI.skin.GetStyle("Label"))
                {
                    alignment = TextAnchor.MiddleRight,
                    fontStyle = FontStyle.Normal
                };
            }
        }

        private static void OnEnterRankingState()
        {
            CheckStyles();
            CheckRankingAsset();
            
            newBestRecord = false;
            rankingScrollPos = Vector2.zero;
            
            if (matchWon)
            {
                SaveCurrentScore();
            }
        }

        private static void SaveCurrentScore()
        {
            newBestRecord = false;
            var userAlreadyInRanking = false;
            for (var i = 0; i < rankingAsset.Nicknames.Count; i++)
            {
                if (rankingAsset.Nicknames[i] != nickName)
                {
                    continue;
                }
                
                userAlreadyInRanking = true;
                if (rankingAsset.Scores[i] > totalTime)
                {
                    newBestRecord = true;
                    rankingAsset.Scores[i] = totalTime;
                }
            }

            if (!userAlreadyInRanking)
            {
                newBestRecord = true;
                rankingAsset.Nicknames.Add(nickName);
                rankingAsset.Scores.Add(totalTime);
            }

            ReorderRanking();
        }

        private static void ReorderRanking()
        {
            for (var i = 0; i < rankingAsset.Nicknames.Count; i++)
            {
                if (i == rankingAsset.Nicknames.Count - 1)
                {
                    continue;
                }

                if (rankingAsset.Scores[i] > rankingAsset.Scores[i + 1])
                {
                    var scoreToMove = rankingAsset.Scores[i];
                    rankingAsset.Scores.RemoveAt(i);
                    rankingAsset.Scores.Insert(i + 1, scoreToMove);
                    
                    var nickNameToMove = rankingAsset.Nicknames[i];
                    rankingAsset.Nicknames.RemoveAt(i);
                    rankingAsset.Nicknames.Insert(i + 1, nickNameToMove);

                    i = -1;
                }

            }
        }

        private static void DrawRankingState()
        {
            if (rankingAsset.Nicknames == null || rankingAsset.Nicknames.Count == 0)
            {
                GUILayout.Label("No scores yet. Keep playing!", centeredLabelStyle);
            }
            else
            {
                DrawWallOfFame();
            }
            
            DrawExitButton();
        }

        private static void DrawWallOfFame()
        {
            GUILayout.Label("W A L L   O F   F A M E", centeredLabelStyle);
            GUILayout.Space(15f);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            rankingScrollPos = EditorGUILayout.BeginScrollView(rankingScrollPos, 
                    GUILayout.Width(GRID_SIZE * CELL_SIZE), 
                    GUILayout.Height((GRID_SIZE - 2) * CELL_SIZE));
            DrawNamesAndScores();
            EditorGUILayout.EndScrollView();
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        private static void DrawNamesAndScores()
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            for (var i = 0; i < rankingAsset.Nicknames.Count; i++)
            {
                var finalText = "";
                if (rankingAsset.Nicknames[i] == nickName)
                {
                    finalText = "--> ";
                }
                finalText += (i + 1) + ". ";
                GUILayout.Label(finalText, rightLabelStyle);
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            for (var i = 0; i < rankingAsset.Nicknames.Count; i++)
            {
                GUILayout.Label(rankingAsset.Nicknames[i], leftLabelStyle);
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            for (var i = 0; i < rankingAsset.Nicknames.Count; i++)
            {
                var finalText = TimeSpan.FromSeconds(rankingAsset.Scores[i]).ToString("mm\\:ss");
                GUILayout.Label(finalText, leftLabelStyle);
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            for (var i = 0; i < rankingAsset.Nicknames.Count; i++)
            {
                var finalText = " ";
                if (rankingAsset.Nicknames[i] == nickName)
                {
                    finalText += "<-- ";
                    if (newBestRecord)
                    {
                        finalText += "NEW!";
                    }
                }
                GUILayout.Label(finalText, leftLabelStyle);
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private static void DrawExitButton()
        {
            GUILayout.Space(15f);
            
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("CONTINUE", GUILayout.Width(200f), GUILayout.Height(25f)))
            {
                SetState(GameState.MainScreen);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}
#endif