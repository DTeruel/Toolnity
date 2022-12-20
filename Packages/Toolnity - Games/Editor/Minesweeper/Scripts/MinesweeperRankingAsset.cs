using System.Collections.Generic;
using UnityEngine;
namespace Toolnity.Games
{
	public class MinesweeperRankingAsset : ScriptableObject
	{
		[SerializeField] public List<string> Nicknames;
		[SerializeField] public List<double> Scores;
	}
}
