using System.Diagnostics;

static class Extensions
{
    public static int ToAlphabeticalIndex(this char c)
    {
        return char.ToLower(c) - 97;
    }

    public static char ToAlphabet(this int i)
    {
        return (char)(i + 97);
    }

    public static List<int> AllIndicesOf(this string s, char c)
    {
        var foundIndices = new List<int>();
        for (int indexOfC = s.IndexOf(c); indexOfC > -1; indexOfC = s.IndexOf(c, indexOfC + 1 /* startIndex */))
        {
            foundIndices.Add(indexOfC);
        }
        if(foundIndices.Count == 0)
        {
            foundIndices.Add(-1);
        }
        return foundIndices;
    }
}

public class WordleSolver
{
    private class LetterInfo
    {
        public LetterInfo()
        {
            Status = StatusFlag.NotUsedAsInput;
            WrongIndices = new HashSet<int>();
            CorrectIndices = new HashSet<int>();
        }

        public void Reset()
        {
            Status = StatusFlag.NotUsedAsInput;
            WrongIndices.Clear();
            CorrectIndices.Clear();
        }

        public enum StatusFlag
        {
            NotUsedAsInput, // Gray     
            NotInAnswerWord,// Black
            InAnswerWord,   // Yellow
            LocationKnown,  // Green
        }
        public StatusFlag Status;
        public HashSet<int> WrongIndices;
        public HashSet<int> CorrectIndices;
    }

    private static readonly byte NumAlphabets = 26;
    private static readonly byte WordLength = 5;
    private static readonly byte NumTrials = 6;

    // 
    private string[] _wordPool;
    public string[] WordPool
    {
        //get { return _wordPool; }
        set
        {
            _wordPool = value;
        }
    }
    private List<LetterInfo> _letterInfos;
    private string? _answerWord;

    public WordleSolver(string[] wordPool)
    {
        _wordPool = wordPool;
        // todo: yellow/greenのアルファベット数まで削減し随時Addに変更
        _letterInfos = new List<LetterInfo>(NumAlphabets)
        {
            new LetterInfo(), new LetterInfo(), new LetterInfo(),
            new LetterInfo(), new LetterInfo(), new LetterInfo(),
            new LetterInfo(), new LetterInfo(), new LetterInfo(),
            new LetterInfo(), new LetterInfo(), new LetterInfo(),
            new LetterInfo(), new LetterInfo(), new LetterInfo(),
            new LetterInfo(), new LetterInfo(), new LetterInfo(),
            new LetterInfo(), new LetterInfo(), 
            new LetterInfo(), new LetterInfo(), 
            new LetterInfo(), new LetterInfo(), 
            new LetterInfo(), new LetterInfo(), 
        };
        _answerWord = null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="answerWord">正解の単語</param>
    /// <returns>(bool, byte) = (正解か, 試行回数)</returns>
    public (bool, byte) SolveHardMode()
    {
        _answerWord = GetRandomWord();

        // time performance
        Stopwatch watch = Stopwatch.StartNew();

        // solve
        bool answeredCorrectly = false;
        byte curNumTrials = 0;
        while (curNumTrials < NumTrials)
        {
            curNumTrials++;
            var inputWord = GetBestInputWord();
            Console.WriteLine($"Input {curNumTrials} : {inputWord}");
            if (inputWord == _answerWord)
            {
                answeredCorrectly = true;
                break;
            }
            else
            {
                AddLetterInfos(inputWord);
            }
        }

        watch.Stop();
        // Print the result
        Console.WriteLine($"Answer  : {_answerWord}");
        Console.WriteLine($"Trials  : {curNumTrials}");
        Console.WriteLine($"Time    : {watch.ElapsedMilliseconds} ms");
        // reset for the next one
        Reset();
        return (answeredCorrectly, curNumTrials);
    }

    private void Reset()
    {
        foreach (var letterInfo in _letterInfos)
        {
            letterInfo.Reset();
        }
    }

    private string GetRandomWord()
    {
        Random r = new Random();
        int randomIdx = r.Next(0, _wordPool.Count());
        return _wordPool[randomIdx];
    }

    private void AddLetterInfos(string inputWord)
    {
        for(int i = 0; i < WordLength; i++)
        {
            var letterInfo = _letterInfos[inputWord[i].ToAlphabeticalIndex()];
            var answerCharIndices = _answerWord?.AllIndicesOf(inputWord[i]);
            // ここら辺の処理整理したい
            foreach (var answerCharIndex in answerCharIndices)
            {
                if (answerCharIndex == i)
                {
                    letterInfo.Status = LetterInfo.StatusFlag.LocationKnown;
                    letterInfo.CorrectIndices.Add(i);
                    if (letterInfo.WrongIndices.Contains(i))
                    {
                        letterInfo.WrongIndices.Remove(i);
                    }
                }
                else if (answerCharIndex == -1)
                {
                    letterInfo.Status = LetterInfo.StatusFlag.NotInAnswerWord;
                }
                else
                {
                    if (letterInfo.Status != LetterInfo.StatusFlag.LocationKnown)
                    {
                        letterInfo.Status = LetterInfo.StatusFlag.InAnswerWord;
                    }
                    if (!letterInfo.CorrectIndices.Contains(i))
                    {
                        letterInfo.WrongIndices.Add(i);
                    }
                    // WordLength - 1箇所間違っている場所がわかっているなら、残り1か所が正解の場所と確定する.
                    if (letterInfo.WrongIndices.Count() == WordLength - 1)
                    {
                        letterInfo.Status = LetterInfo.StatusFlag.LocationKnown;

                        var sumFromOneToWordLength = (WordLength-1) * (WordLength) / 2;
                        var sumWrongIndices = 0;
                        foreach (var wrongIndex in letterInfo.WrongIndices)
                        {
                            sumWrongIndices += wrongIndex;
                        }
                        var correctIdx = sumFromOneToWordLength - sumWrongIndices;
                        letterInfo.CorrectIndices.Add(correctIdx);
                    }

                }
            }
        }
    }

    private string GetBestInputWord()
    {
        List<string> candidateWords = new List<string>();
        foreach (string word in _wordPool)
        {
            bool isCandidate = true;
            for (int i = 0; i < _letterInfos.Count(); i++)
            {
                var letterInfo = _letterInfos[i];
                switch (letterInfo.Status)
                {
                    case LetterInfo.StatusFlag.NotUsedAsInput:
                    break;
                    case LetterInfo.StatusFlag.NotInAnswerWord:
                    {
                        // AnswerWordに含まれていない黒のアルファベットが含まれている場合は候補から外れる。
                        if (word.Contains(i.ToAlphabet()))
                        {
                            isCandidate = false;
                        }
                    }
                    break;
                    case LetterInfo.StatusFlag.InAnswerWord:
                    {
                        if (!word.Contains(i.ToAlphabet()))
                        {
                            isCandidate = false;
                            break;
                        }
                        foreach (var wrongIdx in letterInfo.WrongIndices)
                        {
                            if (word[wrongIdx] == i.ToAlphabet())
                            {
                                isCandidate = false;
                                break;
                            }
                        }
                    }
                    break;
                    case LetterInfo.StatusFlag.LocationKnown:
                    {
                        if (!word.Contains(i.ToAlphabet()))
                        {
                            isCandidate = false;
                            break;
                        }
                        foreach (var correctIdx in letterInfo.CorrectIndices)
                        {
                            if (word[correctIdx] != i.ToAlphabet())
                            {
                                isCandidate = false;
                                break;
                            }
                        }
                    }
                    break;
                    default:
                    break;
                }
                if (!isCandidate)
                {
                    break;
                }
            }
            if (isCandidate)
            {
                candidateWords.Add(word);
            }
        }
        string returnWord = GetBestInputWordFromCandidates(candidateWords.ToArray());
        Debug.Assert(returnWord.Count() == WordLength);
        return returnWord;
    }

    private string GetBestInputWordFromCandidates(string[] candidateWords)
    {
        GetAlphabetFrequencies(candidateWords.ToArray(), out Dictionary<char, uint> alphbetFreqs);
        return GetWordWithMostFrequentLetters(candidateWords.ToArray(), alphbetFreqs);
    }

    private void GetAlphabetFrequencies(string[] words, out Dictionary<char, uint> alphbetFreqs)
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
    private string GetWordWithMostFrequentLetters(string[] words, Dictionary<char, uint> alphbetFreqs)
    {
        string returnWord = "";
        uint curMaxScore = 0;
        foreach (string word in words)
        {
            uint score = 0;
            List<char> lettersAppeared = new List<char>();
            foreach (char letter in word)
            {
                if (!lettersAppeared.Contains(letter))
                {
                    lettersAppeared.Add(letter);
                    score += alphbetFreqs[letter];
                }
            }
            if (score > curMaxScore)
            {
                curMaxScore = score;
                returnWord = word;
            }
        }
        return returnWord;
    }

}