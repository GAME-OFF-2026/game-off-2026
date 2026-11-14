using System.Linq;
using Godot;

[GlobalClass]
public partial class GameManager : Node
{
    [Export] private uint _phaseProtestDuration = 10;
    [Export] private uint _phaseEndDuration = 10;

    [Signal] public delegate void SetupPhaseEndEventHandler();
    [Signal] public delegate void ProtestPhaseEndEventHandler();

    private MobManager _mobManager;
    private RichTextLabel _scoreText;
    private double _phaseCurrentTime;
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

        _phaseCurrentTime += delta;
		if (GamePhase == GamePhase.Protest)
        {
            if (_phaseCurrentTime > _phaseProtestDuration)
            {
                _phaseCurrentTime = 0;
                EndProtestPhase();
            }
        }
        else if (GamePhase == GamePhase.Ending)
        {
            if (_phaseCurrentTime > _phaseEndDuration)
            {
                _phaseCurrentTime = 0;
                EndEndingPhase();
            }
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

        GamePhase = GamePhase.Ending;
        EmitSignalProtestPhaseEnd();
    }

    
    public void EndEndingPhase()
    {
        if (GamePhase != GamePhase.Ending)
        {
            GD.PrintErr("GameManager: EndEndingPhase called in phase: " + GamePhase);
            return;
        }
        
        // TODO 
    }

    public double SetupPhaseCompletion()
    {
        return _mobManager.PlacedPackRate;
    }

    
    public double ProtestPhaseCompletion()
    {
        if (GamePhase == GamePhase.Setup)
            return 0;
        else if (GamePhase == GamePhase.Ending)
            return 1;

        if (_phaseProtestDuration == 0)
            return 1;

        return _phaseCurrentTime / _phaseProtestDuration;
    }   

    public double EndingPhaseCompletion()
    {
        if (GamePhase == GamePhase.Setup || GamePhase == GamePhase.Protest)
            return 0;

        if (_phaseEndDuration == 0)
            return 1;

        return _phaseCurrentTime / _phaseEndDuration;
    }
}