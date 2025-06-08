using System;
using System.Collections.Generic;
using CamLib;

public class FinalRankingManager : Singleton<FinalRankingManager>
{
    public List<Ranking> Rankings = new List<Ranking>();
    
    
        
    [Serializable]
    public class Ranking
    {
        public string Rank;
        public int ScoreThreshold;
    }

    public string GetRanking(int score)
    {
        // Find the highest ranking that the score qualifies for
        string currentRank = Rankings[0].Rank; // Default to lowest rank
            
        for (int i = Rankings.Count - 1; i >= 0; i--)
        {
            if (score >= Rankings[i].ScoreThreshold)
            {
                currentRank = Rankings[i].Rank;
                break;
            }
        }

        return currentRank;
    }
}