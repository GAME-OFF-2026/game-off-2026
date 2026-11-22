using Godot;
using Godot.Collections;

public enum FutureTimeline
{
    Fascist,
    Capitalist,
    Liberal,
    Progressive
}

public partial class GlobalState : Node
{
    private readonly Array<Array<int>> _scoreThresholds = [[3000]];
    public static GlobalState Instance { get; private set; }

    public FutureTimeline Timeline { get; private set; }
    public int Day { get; private set; }
    public int LatestScore { get; private set; }

    public override void _Ready()
    {
        if (IsInstanceValid(Instance))
            GD.PrintErr("GlobalState instantiated twice");
        Instance = this;
    }

    public void SwitchToDialoguePhase(int finalScore)
    {
        GetTree().SetPause(false);
        Day++;
        LatestScore = finalScore;
        Timeline = GetNewTimeline(_scoreThresholds, Day, finalScore);
        GetTree().ChangeSceneToFile("res://scenes/dialogue.tscn");
    }

    public void SwitchToGamePhase()
    {
        GetTree().SetPause(false);
        GetTree().ChangeSceneToFile("res://scenes/JeanMace.tscn");
    }

    private static FutureTimeline GetNewTimeline(Array<Array<int>> dayThresholds, int day, int score)
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