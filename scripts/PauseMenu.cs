using Godot;

[GlobalClass]
public partial class PauseMenu : Panel
{
    public override void _Ready()
    {
        GetNode<Button>("%QuitButton").Pressed += () => { GlobalState.Instance.QuitToMenu(); };
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("Pause"))
        {
            GetTree().Paused = !GetTree().Paused;
            Visible = GetTree().Paused;
        }
    }
}