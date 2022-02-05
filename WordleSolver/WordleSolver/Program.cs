using System.Diagnostics;

#region main
// これもsolverに持たせたほうがいいかも
string[] words = System.IO.File.ReadAllLines(@"D:\Personal Projects\wordle-solver\WordleSolver\WordleSolver\sgb-words.txt");
PlayWordle(words);
#endregion main

void PlayWordle(string[] words)
{
    // 1. choose a random word, which is the answer
    Random r = new Random();
    int randomIdx = r.Next(0, words.Count());
    string answerWord = words[randomIdx];
    
    // 2. Find the answer
    WordleSolver solver = new WordleSolver();
    (var solved, var numTrials) = solver.Solve(answerWord);
}

List<LetterInfo> GetLetterInfos(string input, string answer)
{
    // todo
    return new List<LetterInfo>();
}

void GetAlphabetFrequencies(string[] words, out Dictionary<char, uint> alphbetFreqs)
{
    alphbetFreqs = new Dictionary<char, uint>(0);
    {
        alphbetFreqs.Add('a', 0);
        alphbetFreqs.Add('b', 0);
        alphbetFreqs.Add('c', 0);
        alphbetFreqs.Add('d', 0);
        alphbetFreqs.Add('e', 0);
        alphbetFreqs.Add('f', 0);
        alphbetFreqs.Add('g', 0);
        alphbetFreqs.Add('h', 0);
        alphbetFreqs.Add('i', 0);
        alphbetFreqs.Add('j', 0);
        alphbetFreqs.Add('k', 0);
        alphbetFreqs.Add('l', 0);
        alphbetFreqs.Add('m', 0);
        alphbetFreqs.Add('n', 0);
        alphbetFreqs.Add('o', 0);
        alphbetFreqs.Add('p', 0);
        alphbetFreqs.Add('q', 0);
        alphbetFreqs.Add('r', 0);
        alphbetFreqs.Add('s', 0);
        alphbetFreqs.Add('t', 0);
        alphbetFreqs.Add('u', 0);
        alphbetFreqs.Add('v', 0);
        alphbetFreqs.Add('w', 0);
        alphbetFreqs.Add('x', 0);
        alphbetFreqs.Add('y', 0);
        alphbetFreqs.Add('z', 0);
    }
    foreach (string word in words)
    {
        foreach (char letter in word)
        {
            alphbetFreqs[letter]++;
        }
    }
}

/// <summary>
/// Get a word wihch has the highest total of consisting letters' frequencies and occurs first.
/// </summary>
string GetWordWithMostFrequentLetters(string[] words, Dictionary<char, uint> alphbetFreqs)
{
    string returnWord = "";
    uint curMaxScore = 0;
    foreach (string word in words)
    {
        uint score = 0;
        foreach (char letter in word)
        {
            score += alphbetFreqs[letter];
        }
        if (score > curMaxScore)
        {
            curMaxScore = score;
            returnWord = word;
        }
    }
    return returnWord;
}
