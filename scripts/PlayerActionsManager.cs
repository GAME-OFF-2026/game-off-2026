using Godot;
using Godot.Collections;

[GlobalClass]
public partial class PlayerActionsManager : Control
{
    [Export] private Array<PlayerActionData> _actions;

    private PackedScene _playerActionButtonScene = GD.Load<PackedScene>("res://ui/player_action_button.tscn");
    private Container _buttonsContainer;

    private PlayerActionItem _itemBeingPlaced;

    private MobManager _mobManager;

    public override void _Ready()
    {
        _mobManager = GetNode<MobManager>("%MobManager");
        _buttonsContainer = GetNode<Container>("ButtonsContainer");

        GetNode<GameManager>("%GameManager").PhaseChanged += phase =>
        {
            // Clear actions
            foreach (var button in _buttonsContainer.GetChildren())
                button.QueueFree();

            // Instantiate all actions for new phase
            foreach (var action in _actions)
            {
                if (action.Phase != phase)
                    continue;

                var newButton = _playerActionButtonScene.Instantiate<Button>();
                newButton.GetNode<TextureRect>("%Icon").Texture = action.Sprite;
                newButton.GetNode<RichTextLabel>("%Label").Text = action.Name;
                _buttonsContainer.AddChild(newButton);

                newButton.Pressed += () =>
                {
                    _itemBeingPlaced?.QueueFree();

                    _itemBeingPlaced = action.Item.Instantiate<PlayerActionItem>();
                    GetNode("/root/Main/Terrain").AddChild(_itemBeingPlaced);
                };
            }
        };
    }

    public override void _Process(double delta)
    {
        if (IsInstanceValid(_itemBeingPlaced))
        {
            _itemBeingPlaced.GlobalPosition = _itemBeingPlaced.GetGlobalMousePosition();
            if (Input.IsActionJustPressed("RightClick"))
            {
                _itemBeingPlaced.QueueFree();
                _itemBeingPlaced = null;
            }
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("Click"))
            if (IsInstanceValid(_itemBeingPlaced))
            {
                _itemBeingPlaced.IsPlaced = true;
                _mobManager.PlacedItems.Add(_itemBeingPlaced);

                _itemBeingPlaced = null;
            }
    }
}