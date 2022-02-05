using System.Diagnostics;

public class LetterInfo
{
    public LetterInfo()
    {
        letter = null;
        status = Status.NotUsedAsInput;
        possibleIndices = new List<byte>();
    }

    enum Status
    {
        NotUsedAsInput, // Gray     
        NotInAnswerWord,// Black
        InAnswerWord,   // Yellow
        LocationKnown,  // Green
    }
    char? letter;
    Status status;
    List<byte> possibleIndices;
}

public class WordleSolver
{
    private static readonly byte NumAlphabets = 26;
    private static readonly byte WordLength = 5;
    private static readonly byte NumTrials = 6;

    private List<LetterInfo> _letterInfos;
    private string? _answerWord; 

    public WordleSolver()
    {
        _letterInfos = new List<LetterInfo>(NumAlphabets);
        _answerWord = null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="answerWord">正解の単語</param>
    /// <returns>(bool, byte) = (正解か, 試行回数)</returns>
    public (bool, byte) Solve(string answerWord)
    {
        Debug.Assert(answerWord != null && answerWord.Count() == WordLength);
        _answerWord = answerWord;

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
        Console.WriteLine($"Answer  : {answerWord}");
        Console.WriteLine($"Trials  : {curNumTrials}");
        Console.WriteLine($"Time    : {watch.ElapsedMilliseconds} ms");

        return (answeredCorrectly, curNumTrials);
    }

    public void AddLetterInfos(string inputWord)
    {
        // todo
    }

    public string GetBestInputWord()
    {
        // todo
        return "";
    }

}