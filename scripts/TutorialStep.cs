using Godot;
using Godot.Collections;

internal enum TutorialStepTrigger
{
    None,
    After10Seconds,
    OnUpgrade
}

[GlobalClass]
public partial class TutorialStep : RichTextLabel
{
    [Export] private TutorialStepTrigger _trigger = TutorialStepTrigger.None;
    [Export] private Array<CanvasItem> _highlightNodes;

    private GameManager _gameManager;
    private PlayerActionsManager _playerActionsManager;

    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>("%GameManager");
        _playerActionsManager = GetNode<PlayerActionsManager>("%PlayerActionsManager");
    }

    public void SetActive(bool active)
    {
        Visible = active;
        if (_highlightNodes != null)
            foreach (var node in _highlightNodes)
                node.ZIndex = active ? 1 : 0;
    }

    public bool IsTriggered() =>
        _trigger switch
        {
            TutorialStepTrigger.After10Seconds => _gameManager.PhaseCurrentTime > 10,
            TutorialStepTrigger.OnUpgrade => _gameManager.PhaseCurrentTime > _playerActionsManager.BonusPeriod,
            _ => true
        };
}