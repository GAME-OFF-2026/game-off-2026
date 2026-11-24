using Godot;

[GlobalClass]
public partial class PhaseClock : Control
{
    // Managers
    private GameManager _gameManager;
    private MobManager _mobManager;

    // Parameters
    private int PhaseNumber = 3;
    private int PhaseDuration = 1;
    private Color[] PhaseActiveColors = { Colors.Orange, Colors.Cyan, Colors.Blue };
    private float Elapsed;
    private int CurrentPhase;

    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>("%GameManager");
    }

    public override void _Process(double delta)
    {
        if (_gameManager.GamePhase == GamePhase.Setup)
        {
            CurrentPhase = 0;
            Elapsed = (float)_gameManager.SetupPhaseCompletion;
        }
        else if (_gameManager.GamePhase == GamePhase.Protest)
        {
            CurrentPhase = 1;
            Elapsed = 1 + (float)_gameManager.ProtestPhaseCompletion();
        }
        else if (_gameManager.GamePhase == GamePhase.Evening)
        {
            CurrentPhase = 2;
            Elapsed = 2 + (float)_gameManager.EveningPhaseCompletion();
        }

        // QueueRedraw();
    }

    private int PhaseAt(float time)
    {
        var acc = 0f;
        for (var i = 0; i < PhaseNumber; i++)
        {
            acc += PhaseDuration;
            if (time < acc)
                return i;
        }

        return PhaseNumber - 1;
    }

    public override void _Draw()
    {
        var center = Size / 2f;
        float total = PhaseDuration * PhaseNumber;
        var start = -Mathf.Pi / 2f;

        var radius = 40f;
        var thickness = 20f;
        for (var i = 0; i < PhaseNumber; i++)
        {
            var end = start + Mathf.Tau * PhaseDuration / total;
            var col = PhaseActiveColors[i].Lerp(new Color(0, 0, 0, 0), i == CurrentPhase ? 0 : 0.5f);
            DrawArc(center, radius, start, end, 64, col, thickness, true);
            start = end;
        }

        var angle = -Mathf.Pi / 2f + Mathf.Tau * Elapsed / total;
        var tip = center + Vector2.FromAngle(angle) * (radius + thickness / 2f);
        DrawLine(center, tip, Colors.White, 3f, true);
        DrawCircle(center, 6f, Colors.White);
    }

    public void Reset()
    {
        Elapsed = 0f;
        CurrentPhase = 0;
    }
}