using System.Linq;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class PlayerActionsManager : Control
{
    [Export] private Array<PlayerActionData> _actions;
    [Export] private double _bonusPeriod = 29;

    private PackedScene _playerActionButtonScene = GD.Load<PackedScene>("res://ui/player_action_button.tscn");
    private PackedScene _bonusChoiceScene = GD.Load<PackedScene>("res://ui/bonus_choice.tscn");

    private Container _buttonsContainer;
    private Container _bonusChoicesContainer;
    private Label _mouseLabel;

    private PlayerActionItem _itemBeingPlaced;

    private MobManager _mobManager;

    private double _timeSinceLastBonus;

    private GameManager _gameManager;
    public double BonusPeriod => _bonusPeriod;

    public override void _Ready()
    {
        _actions = [.. _actions.Select(a => a.DuplicateDeep() as PlayerActionData)];

        _buttonsContainer = GetNode<Container>("ButtonsContainer");
        _bonusChoicesContainer = GetNode<Container>("ChoicesContainer");
        _mouseLabel = GetNode<Label>("%MouseLabel");

        _mobManager = GetNode<MobManager>("%MobManager");
        _gameManager = GetNode<GameManager>("%GameManager");

        _gameManager.PhaseChanged += phase =>
        {
            // Clear actions
            foreach (var button in _buttonsContainer.GetChildren())
                button.QueueFree();

            // Instantiate all actions for new phase
            foreach (var action in _actions)
            {
                if (phase == GamePhase.Setup && !action.IsAvailableAtSetup)
                    continue;

                // Clear leftover setup actions at protest start
                if (phase == GamePhase.Protest && action.IsAvailableAtSetup)
                    action.Uses = 0;

                var newButton = _playerActionButtonScene.Instantiate<PlayerActionButton>();
                _buttonsContainer.AddChild(newButton);
                newButton.SetAction(action, false);

                newButton.Pressed += () =>
                {
                    if (IsInstanceValid(_itemBeingPlaced))
                        _itemBeingPlaced.QueueFree();

                    _itemBeingPlaced = action.Item.Instantiate<PlayerActionItem>();
                    _itemBeingPlaced.Action = action;
                    GetNode("/root/Main/Terrain").AddChild(_itemBeingPlaced);
                };
            }
        };
    }

    public override void _Process(double delta)
    {
        if (IsInstanceValid(_itemBeingPlaced))
        {
            Visible = false;
            _mouseLabel.Visible = true;
            _mouseLabel.GlobalPosition = _itemBeingPlaced.GetGlobalMousePosition() + new Vector2(0, 50);
            _itemBeingPlaced.GlobalPosition = _itemBeingPlaced.GetGlobalMousePosition();
            if (Input.IsActionJustPressed("RightClick"))
            {
                _itemBeingPlaced.QueueFree();
                _itemBeingPlaced = null;
            }
        }
        else
        {
            Visible = true;
            _mouseLabel.Visible = false;
        }

        if (_gameManager.GamePhase == GamePhase.Protest)
        {
            _timeSinceLastBonus += delta;
            if (_timeSinceLastBonus >= _bonusPeriod)
            {
                _timeSinceLastBonus = 0;
                var index1 = Utils.Rng.RandiRange(0, _actions.Count - 1);
                var index2 = Utils.Rng.RandiRange(0, _actions.Count - 1);
                while (index1 == index2)
                    index2 = Utils.Rng.RandiRange(0, _actions.Count - 1);

                var newChoice = _bonusChoiceScene.Instantiate<Control>();
                _bonusChoicesContainer.AddChild(newChoice);

                var buttonsContainer = newChoice.GetNode<Container>("ButtonsContainer");

                foreach (var action in (PlayerActionData[])[_actions[index1], _actions[index2]])
                {
                    var newButton = _playerActionButtonScene.Instantiate<PlayerActionButton>();
                    buttonsContainer.AddChild(newButton);
                    newButton.SetAction(action, true);
                    newButton.GetNode<RichTextLabel>("%UsesLabel").Visible = false;
                    newButton.Pressed += () =>
                    {
                        action.Uses++;
                        newChoice.QueueFree();
                    };
                }
            }
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("Click"))
            if (IsInstanceValid(_itemBeingPlaced))
            {
                _itemBeingPlaced.Action.Uses--;
                _itemBeingPlaced.IsPlaced = true;
                _mobManager.PlacedItems.Add(_itemBeingPlaced);

                _itemBeingPlaced = null;

                GetNode<GameManager>("%GameManager").SetupPhaseSpent++;
            }
    }
}