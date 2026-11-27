using Godot;

[GlobalClass]
public partial class ScoreBar : Control
{
    private GameManager _gameManager;
    private Control _marker;

    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>("%GameManager");
        _marker = GetNode<Control>("Marker");

        var container = GetNode<Control>("%VBoxContainer");

        foreach (var unused in GlobalState.Instance.CurrentScoreThresholds)
        {
            container.AddChild(GetNode<Control>("%Spacer").Duplicate());
            container.AddChild(GetNode<Control>("%Mark").Duplicate());
        }
    }

    public override void _Process(double delta)
    {
        _marker.Position =
            new Vector2(0, Mathf.Lerp(
                Size.Y - 5,
                0f,
                (float)GlobalState.Instance.GetDayProgress(_gameManager.ScoreFloored)
            ));
    }
}