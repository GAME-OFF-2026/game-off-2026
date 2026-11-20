using System.Linq;
using Godot;

[GlobalClass]
public partial class GameManager : Node
{
    [Signal]
    public delegate void PhaseChangedEventHandler(GamePhase phase);

    [Export] private uint _protestPhaseDuration = 120;
    [Export] private uint _eveningPhaseDuration = 120;

    private MobManager _mobManager;
    private RichTextLabel _scoreText;
    private double _phaseCurrentTime;

    public uint SetupPhaseSpent = 0;

    public GamePhase GamePhase { get; private set; }

    public double Score { get; private set; }
    private int ScoreFloored => Mathf.FloorToInt(Score);


    public uint SetupPhaseBudget => 8;
    public double SetupPhaseCompletion => SetupPhaseSpent / (float)SetupPhaseBudget;

    public void SetGamePhase(GamePhase newGamePhase)
    {
        _phaseCurrentTime = 0;

        GamePhase = newGamePhase;
        EmitSignalPhaseChanged(GamePhase);
    }

    public override void _Ready()
    {
        _mobManager = GetNode<MobManager>("%MobManager");

        _scoreText = GetNode<RichTextLabel>("%ScoreText");

        GetNode<Button>("%BackToTheFutureButton").Pressed += () => { GlobalState.Instance.SwitchToDialoguePhase(ScoreFloored); };

        PhaseChanged += phase =>
        {
            if (phase == GamePhase.Protest)
            {
                GetNode<NavigationRegion2D>("%ProtesterNavmesh").BakeNavigationPolygon();
                GetNode<NavigationRegion2D>("%NonProtesterNavmesh").BakeNavigationPolygon();
            }

            if (phase == GamePhase.End)
            {
                GetTree().SetPause(true);

                GetNode<RichTextLabel>("%FinalScoreLabel").Text = $"Final Score: {ScoreFloored}";
                GetNode<Control>("%EndScreen").Visible = true;
            }
        };

        SetGamePhase(GamePhase.Setup);
    }

    public override void _Process(double delta)
    {
        if (GamePhase == GamePhase.Setup)
        {
            if (SetupPhaseSpent >= SetupPhaseBudget)
                SetGamePhase(GamePhase.Protest);
            return;
        }

        Score += delta * _mobManager.CurrentMobs.Sum(mob => mob.IsProtesting ? mob.Motivation : 0);
        _scoreText.Text = $"Score: {ScoreFloored}";

        _phaseCurrentTime += delta;
        if (GamePhase == GamePhase.Protest)
        {
            if (_phaseCurrentTime > _protestPhaseDuration)
                SetGamePhase(GamePhase.Evening);
        }
        else if (GamePhase == GamePhase.Evening)
        {
            if (!_mobManager.CurrentMobs.Any(mob => mob.IsProtesting))
                SetGamePhase(GamePhase.End);
        }
    }


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