using Godot;

public partial class GlobalState : Node
{
    public static GlobalState Instance { get; private set; }

    public int Day { get; private set; }
    public int LatestScore { get; private set; }

    public override void _Ready()
    {
        if (IsInstanceValid(Instance))
            GD.PrintErr("GlobalState instantiated twice");
        Instance = this;
    }

    public void SwitchToDialoguePhase(int FinalScore)
    {
        GetTree().SetPause(false);
        Day++;
        LatestScore = FinalScore;
        GetTree().ChangeSceneToFile("res://scenes/dialogue.tscn");
    }

    public void SwitchToGamePhase()
    {
        GetTree().SetPause(false);
        GetTree().ChangeSceneToFile("res://scenes/JeanMace.tscn");
    }
}