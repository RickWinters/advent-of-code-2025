// See https://aka.ms/new-console-template for more information

using AoC.Year2025;


string dayPath(int day)
{
    return $"Docments";
}
var solutionRunner = new AoCDayRunner();
for (var i = 1; i < 13; i++)
{
    await solutionRunner.Run(i, dayPath(i));
}