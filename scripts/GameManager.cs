using System.Linq;
using Godot;

[GlobalClass]
public partial class GameManager : Node
{
    [Signal]
    public delegate void SetupPhaseEndEventHandler();

    private MobManager _mobManager;
    private RichTextLabel _scoreText;

    public GamePhase GamePhase { get; private set; } = GamePhase.Setup;

    public double Score { get; private set; }

    public override void _Ready()
    {
        _mobManager = GetNode<MobManager>("%MobManager");

        _scoreText = GetNode<RichTextLabel>("%ScoreText");
    }

    public override void _Process(double delta)
    {
        if (GamePhase == GamePhase.Setup)
            return;

        Score += delta * _mobManager.CurrentMobs.Sum(mob => mob.IsProtesting ? mob.Motivation : 0);
        _scoreText.Text = $"Score: {Score:N0}";
    }

    public void EndSetupPhase()
    {
        if (GamePhase != GamePhase.Setup)
        {
            GD.PrintErr("GameManager: EndSetupPhase called in phase: " + GamePhase);
            return;
        }

        GamePhase = GamePhase.Protest;
        EmitSignalSetupPhaseEnd();
    }
}