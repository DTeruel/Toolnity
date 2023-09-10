#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Toolnity.Games
{
	public partial class Minesweeper
	{
		private const int GRID_SIZE = 10;
		private const float MINE_PERCENTAGE = 0.15f;

		private const int CELL_SIZE = 25;
        
		private enum CellState
		{
			Default,
			Marked,
			Question
		}
        
		private struct CellInfo
		{
			public bool IsRevealed;
			public bool HasMine;
			public int MinesAround;
			public CellState State;
		}
		
		private static GUIContent blankButton;
		private static GUIContent flagButton;
		private static GUIContent questionButton;
		private static GUIContent mineButton;
		private static GUIContent[] numberButton;
        
		private static CellInfo[][] grid;
		private static int remainingCellsToReveal;
        
		private static double startTime;
		private static double totalTime;

		private static bool matchWon;

		#region FLOW
		private static void OnEnterPlayingState()
		{
			CheckCellButtons();

			GenerateGrid();
			matchWon = false;
			startTime = EditorApplication.timeSinceStartup;
		}

		private static void FinishGame(bool win)
		{
			if (win)
			{
				matchWon = true;
				MarkAllMinesAsFlags();
				GetInstance().ShowNotification(new GUIContent("You Win!"));
			}
			else
			{
				matchWon = false;
				RevealAllCells();
				GetInstance().ShowNotification(new GUIContent("You Lose!"));
			}

			SetState(GameState.Finished);
		}
		#endregion
		
		#region GRID GENERATION
		private static void GenerateGrid()
		{
			grid = new CellInfo[GRID_SIZE][];
            
			for (var i = 0; i < GRID_SIZE; i++)
			{
				grid[i] = new CellInfo[GRID_SIZE];
			}

			remainingCellsToReveal = GRID_SIZE * GRID_SIZE;
            
			AddMines();
		}

        private static void AddMines()
        {
            var numMines = (int)(GRID_SIZE * GRID_SIZE * MINE_PERCENTAGE);
            remainingCellsToReveal -= numMines;
            
            while(numMines > 0)
            {
                var i = Random.Range(0, GRID_SIZE);
                var j = Random.Range(0, GRID_SIZE);
                
                if (!grid[i][j].HasMine)
                {
                    numMines--;
                    grid[i][j].HasMine = true;

                    if (i > 0)
                    {
                        if (j > 0)
                        {
                            grid[i - 1][j - 1].MinesAround++;
                        }
                        grid[i - 1][j].MinesAround++;
                        if (j < GRID_SIZE - 1)
                        {
                            grid[i - 1][j + 1].MinesAround++;
                        }
                    }
                    
                    if (j > 0)
                    {
                        grid[i][j - 1].MinesAround++;
                    }
                    if (j < GRID_SIZE - 1)
                    {
                        grid[i][j + 1].MinesAround++;
                    }
                    
                    if (i < GRID_SIZE - 1)
                    {
                        if (j > 0)
                        {
                            grid[i + 1][j - 1].MinesAround++;
                        }
                        grid[i + 1][j].MinesAround++;
                        if (j < GRID_SIZE - 1)
                        {
                            grid[i + 1][j + 1].MinesAround++;
                        }
                    }
                }
            }
        }
		#endregion
		
		#region UPDATE
		private void UpdatePlayingState()
		{
			UpdateTimer();
			Repaint();
		}

		private static void UpdateTimer()
		{
			totalTime = EditorApplication.timeSinceStartup - startTime;
		}
		#endregion

		#region RENDER
		private static void DrawPlayingState()
		{
			DrawTopBarPlaying();
			DrawGrid();
		}

		private static void DrawTopBarPlaying()
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Time: " + TimeSpan.FromSeconds(totalTime).ToString("mm\\:ss"));
			GUILayout.EndHorizontal();
		}

		private static void DrawGrid()
		{
			for (var i = 0; i < GRID_SIZE; i++)
			{
				GUILayout.BeginHorizontal();
				for (var j = 0; j < GRID_SIZE; j++)
				{
					DrawCell(i, j);
				}
				GUILayout.EndHorizontal();
			}
		}

		private static readonly Color DisabledButtonColor = new (0.7f, 0.7f, 0.7f, 1);
		private static void DrawCell(int i, int j)
		{
			if (grid[i][j].IsRevealed)
			{
				EditorGUI.BeginDisabledGroup(grid[i][j].MinesAround == 0);
				
				var buttonContent = GetRevealedCellButton(i, j);
				var defaultBackgroundColor = GUI.backgroundColor;
				GUI.backgroundColor = DisabledButtonColor;
				
				if (GUILayout.Button(
					    buttonContent, 
					    GUILayout.Width(CELL_SIZE), 
					    GUILayout.Height(CELL_SIZE)))
				{
					if (Event.current.button == 2) // Center click
					{
						CheckCellsMarkedAndDiscoverCellsAround(i, j);
					}
				}
				GUI.backgroundColor = defaultBackgroundColor;

				EditorGUI.EndDisabledGroup();
			}
			else
			{
				var buttonStyle = GetUnrevealedCellButton(i, j);
				if (GUILayout.Button(buttonStyle, GUILayout.Width(CELL_SIZE), GUILayout.Height(CELL_SIZE)))
				{
					switch (Event.current.button)
					{
						case 0:	// Left Click
							CellPressed(i, j);
							break;
						case 1: // Right click
							SwitchCellState(i, j);
							break;
					}
				}
			}
		}
		#endregion
		
		#region CELLS
		private static void CheckCellButtons()
		{
			if (blankButton == null)
			{
				blankButton = new GUIContent("");
			}
            
			if (flagButton == null)
			{
				var texture = (Texture)Resources.Load("Flag");
				flagButton = new GUIContent("", texture);
			}
            
			if (questionButton == null)
			{
				var texture = (Texture)Resources.Load("Question");
				questionButton = new GUIContent("", texture);
			}
            
			if (mineButton == null)
			{
				var texture = (Texture)Resources.Load("Mine");
				mineButton = new GUIContent("", texture);
			}

			if (numberButton == null)
			{
				numberButton = new GUIContent[9];
				numberButton[0] = new GUIContent("");
				for (var i = 1; i < 9; i++)
				{
					numberButton[i] = new GUIContent(i.ToString());
				}
			}
		}

		private static GUIContent GetRevealedCellButton(int i, int j)
		{
			if (grid[i][j].HasMine)
			{
				return mineButton;
			}

			return numberButton[grid[i][j].MinesAround];
		}

		private static GUIContent GetUnrevealedCellButton(int i, int j)
		{
			switch(grid[i][j].State)
			{
				case CellState.Marked:
					return flagButton;
				case CellState.Question:
					return questionButton;
			}

			return blankButton;
		}

		private static void SwitchCellState(int i, int j)
		{
			switch(grid[i][j].State)
			{
				case CellState.Default:
					grid[i][j].State = CellState.Marked;
					break;
				case CellState.Marked:
					grid[i][j].State = CellState.Question;
					break;
				case CellState.Question:
					grid[i][j].State = CellState.Default;
					break;
			}
		}

        private static void CellPressed(int i, int j)
        {
	        if (grid[i][j].State != CellState.Default)
	        {
		        return;
	        }
	        
            if (gameState == GameState.MainScreen)
            {
                SetState(GameState.Playing);
            }
	            
            if (grid[i][j].HasMine)
            {
                grid[i][j].IsRevealed = true;
                FinishGame(false);
            }
            else
            {
                RevealCell(i, j);
            }
        }

        private static void CheckCellsMarkedAndDiscoverCellsAround(int i, int j)
        {
	        if (!grid[i][j].IsRevealed || grid[i][j].MinesAround == 0)
	        {
		        return;
	        }

	        var cellsMarkedAround = GetNumCellsMarkedAround(i, j);
	        if (cellsMarkedAround == grid[i][j].MinesAround)
	        {
		        RevealCellsNotMarkedAround(i, j);
	        }
        }

        private static int GetNumCellsMarkedAround(int i, int j)
        {
	        var numCellsMarkedAround = 0;
	      
	        if (i > 0)
	        {
		        if (j > 0)
		        {
			        if (grid[i - 1][j - 1].State == CellState.Marked)
			        {
				        numCellsMarkedAround++;
			        }
			        else if (grid[i - 1][j - 1].State == CellState.Question)
			        {
				        return -1;
			        }
		        }
		        if (grid[i - 1][j].State == CellState.Marked)
		        {
			        numCellsMarkedAround++;
		        }
		        else if (grid[i - 1][j].State == CellState.Question)
		        {
			        return -1;
		        }
		        if (j < GRID_SIZE - 1)
		        {
			        if (grid[i - 1][j + 1].State == CellState.Marked)
			        {
				        numCellsMarkedAround++;
			        }
			        else if (grid[i - 1][j + 1].State == CellState.Question)
			        {
				        return -1;
			        }
		        }
	        }
                
	        if (j > 0)
	        {
		        if (grid[i][j - 1].State == CellState.Marked)
		        {
			        numCellsMarkedAround++;
		        }
		        else if (grid[i][j - 1].State == CellState.Question)
		        {
			        return -1;
		        }
	        }
	        if (j < GRID_SIZE - 1)
	        {
		        if (grid[i][j + 1].State == CellState.Marked)
		        {
			        numCellsMarkedAround++;
		        }
		        else if (grid[i][j + 1].State == CellState.Question)
		        {
			        return -1;
		        }
	        }
                
	        if (i < GRID_SIZE - 1)
	        {
		        if (j > 0)
		        {
			        if (grid[i + 1][j - 1].State == CellState.Marked)
			        {
				        numCellsMarkedAround++;
			        }
			        else if (grid[i + 1][j - 1].State == CellState.Question)
			        {
				        return -1;
			        }
		        }
		        if (grid[i + 1][j].State == CellState.Marked)
		        {
			        numCellsMarkedAround++;
		        }
		        else if (grid[i + 1][j].State == CellState.Question)
		        {
			        return -1;
		        }
		        if (j < GRID_SIZE - 1)
		        {
			        if (grid[i + 1][j + 1].State == CellState.Marked)
			        {
				        numCellsMarkedAround++;
			        }
			        else if (grid[i + 1][j + 1].State == CellState.Question)
			        {
				        return -1;
			        }
		        }
	        }

	        return numCellsMarkedAround;
        }

        private static void RevealCellsNotMarkedAround(int i, int j)
        {
	        if (i > 0)
	        {
		        if (j > 0)
		        {
			        CellPressed(i - 1, j - 1);
		        }
		        CellPressed(i - 1, j);
		        if (j < GRID_SIZE - 1)
		        {
			        CellPressed(i - 1, j + 1);
		        }
	        }
                    
	        if (j > 0)
	        {
		        CellPressed(i, j - 1);
	        }
	        if (j < GRID_SIZE - 1)
	        {
		        CellPressed(i, j + 1);
	        }
                    
	        if (i < GRID_SIZE - 1)
	        {
		        if (j > 0)
		        {
			        CellPressed(i + 1, j - 1);
		        }
		        CellPressed(i + 1, j);
		        if (j < GRID_SIZE - 1)
		        {
			        CellPressed(i + 1, j + 1);
		        }
	        }
        }

        private static void RevealCell(int i, int j)
        {
            if (grid[i][j].IsRevealed)
            {
                return;
            }
            
            if (grid[i][j].MinesAround == 0)
            {
                RevealCellsWithoutMinesAroundRecursive(i, j);
            }
            else
            {
                grid[i][j].IsRevealed = true;
                grid[i][j].State = CellState.Default;
                OnCellRevealed();
            }
        }

        private static bool OnCellRevealed()
        {
            remainingCellsToReveal--;
            if (remainingCellsToReveal <= 0)
            {
                FinishGame(true);
                return true;
            }

            return false;
        }

        private static void RevealCellsWithoutMinesAroundRecursive(int i, int j)
        {
            if (grid[i][j].IsRevealed || grid[i][j].MinesAround > 0)
            {
                return;
            }
            
            grid[i][j].IsRevealed = true;
            grid[i][j].State = CellState.Default;
            if (OnCellRevealed())
            {
                return;
            }
            
            if (i > 0)
            {
                if (j > 0)
                {
                    RevealCell(i - 1, j - 1);
                }
                RevealCell(i - 1, j);
                if (j < GRID_SIZE - 1)
                {
                    RevealCell(i - 1, j + 1);
                }
            }
                    
            if (j > 0)
            {
                RevealCell(i, j - 1);
            }
            if (j < GRID_SIZE - 1)
            {
                RevealCell(i, j + 1);
            }
                    
            if (i < GRID_SIZE - 1)
            {
                if (j > 0)
                {
                    RevealCell(i + 1, j - 1);
                }
                RevealCell(i + 1, j);
                if (j < GRID_SIZE - 1)
                {
                    RevealCell(i + 1, j + 1);
                }
            }
        }

        private static void MarkAllMinesAsFlags()
        {
	        for (var i = 0; i < GRID_SIZE; i++)
	        {
		        for (var j = 0; j < GRID_SIZE; j++)
		        {
			        if (grid[i][j].HasMine && grid[i][j].State == CellState.Default)
			        {
				        grid[i][j].State = CellState.Marked;
			        }
		        }
	        }
        }

        private static void RevealAllCells()
        {
	        for (var i = 0; i < GRID_SIZE; i++)
	        {
		        for (var j = 0; j < GRID_SIZE; j++)
		        {
			        grid[i][j].IsRevealed = true;
		        }
	        }
        }
		#endregion
	}
}
#endif