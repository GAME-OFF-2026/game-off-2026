using System.Linq;
using Godot;

[GlobalClass]
public partial class Tutorial : Panel
{
    private TutorialStep[] _steps;

    private int _stepIndex;

    public override void _Ready()
    {
        _steps = [.. GetChildren().OfType<TutorialStep>()];

        UpdateSteps();
    }

    private void UpdateSteps()
    {
        for (var i = 0; i < _steps.Length; i++)
            _steps[i].SetActive(i == _stepIndex);
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (@event.IsActionPressed("Click"))
        {
            _stepIndex++;
            if (_stepIndex >= _steps.Length)
            {
                QueueFree();
                return;
            }

            UpdateSteps();
        }
    }
}