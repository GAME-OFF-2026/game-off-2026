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
    public readonly int PhaseNumber = 3;
    private readonly Array<Array<int>> _scoreThresholds = [[5000], [10000, 15000], [16000, 25000, 30000]];
    private Array<int> _scores = [-1, -1, -1];
    public static GlobalState Instance { get; private set; }
    public FutureTimeline Timeline 
    { 
        get
        {
            return GetTimeline(_scoreThresholds, Day, TotalScore);
        } 
    }
    public int Day 
    { 
        get
        {
            return _scores.Count(elem => elem != -1);
        }
    }
    public int LatestScore 
    { 
        get
        {
            if (_scores.Any(elem => elem != -1))
            {
                return _scores.LastOrDefault(elem => elem != -1);
            }
            return -1;
        }
    }
    public int TotalScore 
    { 
        get
        {
            if (_scores.Any(elem => elem != -1))
            {
                return _scores.Where(elem => elem != -1).Sum();
            }
            return -1;
        }
    }


    public override void _Ready()
    {
        if (IsInstanceValid(Instance))
            GD.PrintErr("GlobalState instantiated twice");
        Instance = this;
    }

    public void SwitchToDialoguePhase(int finalScore)
    {
        GetTree().SetPause(false);
        _scores[Day] = finalScore;
        GetTree().ChangeSceneToFile("res://scenes/dialogue.tscn");
    }

    public void SwitchToGamePhase()
    {
        GetTree().SetPause(false);
        GetTree().ChangeSceneToFile("res://scenes/JeanMace.tscn");
    }

    private static FutureTimeline GetTimeline(Array<Array<int>> dayThresholds, int day, int score)
    {
        if (day >= dayThresholds.Count)
            return FutureTimeline.Fascist;

        var thresholds = dayThresholds[day];

        for (var i = 0; i < thresholds.Count; i++)
            if (score >= thresholds[i])
                return (FutureTimeline)i;

        return FutureTimeline.Fascist;
    }
}