using Godot;

[GlobalClass]
public partial class PlayerActionButton : Button
{
    private PlayerActionData _action;
    private bool _isBonus;

    private TextureRect _textureRect;
    private RichTextLabel _nameLabel;
    private RichTextLabel _usesLabel;

    public override void _Ready()
    {
        _textureRect = GetNode<TextureRect>("%Icon");
        _nameLabel = GetNode<RichTextLabel>("%NameLabel");
        _usesLabel = GetNode<RichTextLabel>("%UsesLabel");
    }

    public void SetAction(PlayerActionData newAction, bool isBonus)
    {
        _action = newAction;
        _isBonus = isBonus;
        _textureRect.Texture = _action.Sprite;
        _nameLabel.Text = _action.Name;
        _usesLabel.Text = _action.Uses.ToString();
    }

    public override void _Process(double delta)
    {
        _usesLabel.Text = _isBonus ? "" : _action.Uses.ToString();
        Disabled = !_isBonus && _action.Uses <= 0;
    }
}