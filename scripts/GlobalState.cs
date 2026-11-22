using System.Linq;
using Godot;
using Godot.Collections;

public enum FutureTimeline
{
    Fascist = 0,
    Capitalist = 1,
    Liberal = 2,
    Progressive = 3
}

public partial class GlobalState : Node
{
    private readonly Array<Array<int>> _scoreThresholds = [[5000], [10000, 15000], [16000, 25000, 30000]];
    private readonly Array<int> _scores = [];

    public static GlobalState Instance { get; private set; }

    public FutureTimeline Timeline => GetTimeline(_scoreThresholds, Day, TotalScore);

    public int Day => _scores.Count;

    private int LatestScore =>
        _scores.Count > 0
            ? _scores[^1]
            : 0;

    public int TotalScore => _scores.Sum();

    public override void _Ready()
    {
        if (IsInstanceValid(Instance))
            GD.PrintErr("GlobalState instantiated twice");
        Instance = this;
    }

    public void StartNewGame()
    {
        GetTree().SetPause(false);
        _scores.Clear();

        GetTree().ChangeSceneToFile("res://scenes/dialogue.tscn");
    }

    public void SwitchToDialoguePhase(int finalScore)
    {
        GetTree().SetPause(false);
        _scores.Add(finalScore);
        GetTree().ChangeSceneToFile("res://scenes/dialogue.tscn");
    }

    public void SwitchToGamePhase()
    {
        GetTree().SetPause(false);
        GetTree().ChangeSceneToFile("res://scenes/JeanMace.tscn");
    }

    private static FutureTimeline GetTimeline(Array<Array<int>> dayThresholds, int day, int score)
    {
        if (day < 1 || day > dayThresholds.Count)
            return FutureTimeline.Fascist;

        var thresholds = dayThresholds[day - 1];

        for (var i = 0; i < thresholds.Count; i++)
            if (score < thresholds[i])
                return (FutureTimeline)i;

        return (FutureTimeline)thresholds.Count;
    }
}