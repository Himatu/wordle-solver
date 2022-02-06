using System.Diagnostics;

const int N = 10000;
string[] words = System.IO.File.ReadAllLines(@"D:\Personal Projects\wordle-solver\WordleSolver\WordleSolver\sgb-words.txt");
WordleSolver solver = new WordleSolver(words);
var numSolved = 0;
var totalNumTrialsOfSolvedQuestions = 0;
for (int i = 0; i < N; i++)
{
    // todo: パフォーマンスや構造改善、NormalMode対応、他のアルゴリズム探す
    (var solved, var numTrials) = solver.SolveHardMode();
    if (solved)
    {
        numSolved++;
        totalNumTrialsOfSolvedQuestions += numTrials;
    }
}
Console.WriteLine($"Success Rate : {(float)numSolved/N*100:0.00}%");
Console.WriteLine($"Avg. Num Trials : {(float)totalNumTrialsOfSolvedQuestions/N}");
