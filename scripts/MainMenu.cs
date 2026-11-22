using Godot;

[GlobalClass]
public partial class MainMenu : Control
{
    public override void _Ready()
    {
        GetNode<Button>("%NewGameButton").Pressed += () => GlobalState.Instance.StartNewGame();
        GetNode<Button>("%QuitButton").Pressed += () => GetTree().Quit();
    }
}