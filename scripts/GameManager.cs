using System.Linq;
using Godot;

[GlobalClass]
public partial class GameManager : Node
{
    [Signal]
    public delegate void SetupPhaseEndEventHandler();

    [Signal]
    public delegate void ProtestPhaseEndEventHandler();

    [Export] private uint _protestPhaseDuration = 120;
    [Export] private uint _eveningPhaseDuration = 120;

    private MobManager _mobManager;
    private RichTextLabel _scoreText;
    private double _phaseCurrentTime;
    public GamePhase GamePhase { get; private set; } = GamePhase.Setup;

    public double Score { get; private set; }
    private int ScoreFloored => Mathf.FloorToInt(Score);

    public override void _Ready()
    {
        _mobManager = GetNode<MobManager>("%MobManager");

        _scoreText = GetNode<RichTextLabel>("%ScoreText");

        GetNode<Button>("%BackToTheFutureButton").Pressed += () => { GlobalState.Instance.SwitchToDialoguePhase(ScoreFloored); };
    }

    public override void _Process(double delta)
    {
        if (GamePhase == GamePhase.Setup)
            return;

        Score += delta * _mobManager.CurrentMobs.Sum(mob => mob.IsProtesting ? mob.Motivation : 0);
        _scoreText.Text = $"Score: {ScoreFloored}";

        _phaseCurrentTime += delta;
        if (GamePhase == GamePhase.Protest)
        {
            if (_phaseCurrentTime > _protestPhaseDuration)
            {
                _phaseCurrentTime = 0;
                EndProtestPhase();
            }
        }
        else if (GamePhase == GamePhase.Evening)
        {
            if (!_mobManager.CurrentMobs.Any(mob => mob.IsProtesting))
                EndEveningPhase();
        }
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

    public void EndProtestPhase()
    {
        if (GamePhase != GamePhase.Protest)
        {
            GD.PrintErr("GameManager: EndProtestPhase called in phase: " + GamePhase);
            return;
        }

        GamePhase = GamePhase.Evening;
        EmitSignalProtestPhaseEnd();
    }


    public void EndEveningPhase()
    {
        if (GamePhase != GamePhase.Evening)
        {
            GD.PrintErr("GameManager: EndEveningPhase called in phase: " + GamePhase);
            return;
        }

        GamePhase = GamePhase.End;
        GetTree().SetPause(true);

        GetNode<RichTextLabel>("%FinalScoreLabel").Text = $"Final Score: {ScoreFloored}";
        GetNode<Control>("%EndScreen").Visible = true;
    }

    public double SetupPhaseCompletion() => _mobManager.PlacedPackRate;

    public double ProtestPhaseCompletion()
    {
        if (GamePhase == GamePhase.Setup)
            return 0;
        if (GamePhase == GamePhase.Evening)
            return 1;

        if (_protestPhaseDuration == 0)
            return 1;

        return Mathf.Clamp(_phaseCurrentTime / _protestPhaseDuration, 0, 1);
    }

    public double EveningPhaseCompletion()
    {
        if (GamePhase is GamePhase.Setup or GamePhase.Protest)
            return 0;

        if (_eveningPhaseDuration == 0)
            return 1;

        return Mathf.Clamp(_phaseCurrentTime / _eveningPhaseDuration, 0, 1);
    }
}