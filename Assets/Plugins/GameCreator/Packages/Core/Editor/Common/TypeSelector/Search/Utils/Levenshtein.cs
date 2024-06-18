using System;

namespace GameCreator.Editor.Search
{
    internal static class Levenshtein
    {
        public const int MAX_EDITS = 3;
        
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public static int Get(string source, string target)
        {
            int sourceLength = source.Length;
            int targetLength = target.Length;
            
            int threshold = sourceLength > 1 
                ? Math.Clamp(sourceLength - 1, 1, MAX_EDITS) 
                : 0;
            
            if (targetLength > sourceLength)
            {
                target = target[..sourceLength];
                targetLength = sourceLength;
            }

            if (sourceLength - targetLength > threshold)
            {
                return int.MaxValue;
            }
            
            int[,] distances = new int[sourceLength + 1, targetLength + 1];
            
            for (int i = 0; i <= sourceLength; ++i)
            {
                distances[i, 0] = i;
            }
            
            for (int j = 0; j <= targetLength; ++j)
            {
                distances[0, j] = j;
            }
            
            for (int j = 1; j <= targetLength; ++j)
            {
                for (int i = 1; i <= sourceLength; ++i)
                {
                    int cost = source[i - 1] == target[j - 1] ? 0 : 1;
                    int insertion = distances[i, j - 1] + 1;
                    int deletion = distances[i - 1, j] + 1;
                    int substitution = distances[i - 1, j - 1] + cost;
                    
                    distances[i, j] = Math.Min(
                        Math.Min(insertion, deletion),
                        substitution
                    );
            
                    if (i > 1 && j > 1 && source[i - 1] == target[j - 2] && source[i - 2] == target[j - 1])
                    {
                        distances[i, j] = Math.Min(
                            distances[i, j],
                            distances[i - 2, j - 2] + cost
                        );
                    }
                }
            
                if (distances[j, j] > threshold)
                {
                    return int.MaxValue;
                }
            }
            
            return distances[sourceLength, targetLength];
        }
    }
}