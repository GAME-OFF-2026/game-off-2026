using Godot;
using System;

public partial class PhaseClock : Control
{
	// Managers
    private GameManager _gameManager;
    private MobManager _mobManager;

	// Parameters
	private int PhaseNumber = 3;
	private int PhaseDuration = 1;
	private Color[] PhaseActiveColors = { Colors.Orange, Colors.Cyan, Colors.Blue };
	private Color[] PhaseInactiveColors = { Colors.DarkOrange, Colors.DarkCyan, Colors.DarkBlue };
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
			Elapsed = (float)_gameManager.SetupPhaseCompletion();
		}
		else if (_gameManager.GamePhase == GamePhase.Protest)
		{
			CurrentPhase = 1;
			Elapsed = 1 + (float)_gameManager.ProtestPhaseCompletion();
		}
		else if (_gameManager.GamePhase == GamePhase.Ending)
		{
			CurrentPhase = 2;
			Elapsed = 2 + (float)_gameManager.EndingPhaseCompletion();
		}

		QueueRedraw();
	}

	private int PhaseAt(float time)
	{
		float acc = 0f;
		for (int i = 0; i < PhaseNumber; i++)
		{
			acc += PhaseDuration;
			if (time < acc) return i;
		}
		return PhaseNumber - 1;
	}

	public override void _Draw()
	{
		Vector2 center = Size / 2f;
		float total = PhaseDuration * PhaseNumber;
		float start = -Mathf.Pi / 2f; 

		float radius = 40f;
		float thickness = 20f;
		for (int i = 0; i < PhaseNumber; i++)
		{
			float end = start + Mathf.Tau * PhaseDuration / total;
			Color col = i != CurrentPhase ? PhaseInactiveColors[i] : PhaseActiveColors[i];
			DrawArc(center, radius, start, end, 64, col, thickness, true);
			start = end;
		}

		float angle = -Mathf.Pi / 2f + Mathf.Tau * Elapsed / total;
		Vector2 tip = center + Vector2.FromAngle(angle) * (radius + thickness / 2f);
		DrawLine(center, tip, Colors.White, 3f, true);
		DrawCircle(center, 6f, Colors.White);
	}

	public void Reset()
	{
		Elapsed = 0f;
		CurrentPhase = 0;
	}
}