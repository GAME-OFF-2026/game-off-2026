using Godot;

[GlobalClass]
public partial class PauseMenu : Panel
{
    private GameManager _gameManager;

    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>("%GameManager");
        GetNode<Button>("%QuitButton").Pressed += () => { GlobalState.Instance.QuitToMenu(); };
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("Pause"))
        {
            Visible = !Visible;
            _gameManager.PlayerPause = Visible;
        }
    }
}