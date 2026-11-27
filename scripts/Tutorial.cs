using System.Linq;
using Godot;

[GlobalClass]
public partial class Tutorial : Panel
{
    private GameManager _gameManager;
    private bool _started;
    private int _stepIndex;
    private TutorialStep[] _steps;

    private TutorialStep CurrentStep => _stepIndex < _steps.Length ? _steps[_stepIndex] : null;

    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>("%GameManager");
        if (GlobalState.Instance.Day > 0)
        {
            _gameManager.TutorialPause = false;
            QueueFree();
            return;
        }

        _steps = [.. GetChildren().OfType<TutorialStep>()];

        UpdateSteps();
    }

    public override void _Process(double delta)
    {
        Visible = CurrentStep?.IsTriggered() ?? false;
        CurrentStep?.SetActive(Visible);
        _gameManager.TutorialPause = Visible;
    }

    private void UpdateSteps()
    {
        foreach (var step in _steps)
            step.SetActive(false);
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (@event.IsActionPressed("Click"))
        {
            _stepIndex++;
            if (_stepIndex >= _steps.Length)
            {
                _gameManager.TutorialPause = false;
                QueueFree();
                return;
            }

            UpdateSteps();
        }
    }
}