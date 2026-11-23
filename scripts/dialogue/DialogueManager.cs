using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class DialogueManager : Node
{
    private readonly PackedScene _characterScene = GD.Load<PackedScene>("res://dialogue/character_sprite.tscn");

    [Export] private TimelineVariant _barBackground;
    [Export] private TimelineVariant _tvBackground;

    [Export] private Array<DialogueDay> _dialoguePerDay = [new(), new(), new(), new()];

    private Control _namePanel;
    private RichTextLabel _speakerNameLabel;
    private RichTextLabel _dialogueTextLabel;
    private TextureRect _background;

    private int _dialogueLineIndex;

    private DialogueData CurrentDialogue =>
        _dialoguePerDay[Mathf.Clamp(GlobalState.Instance.Day, 0, _dialoguePerDay.Count - 1)].GetVariant(GlobalState.Instance.Timeline);

    public override void _Ready()
    {
        _namePanel = GetNode<Control>("%NamePanel");
        _speakerNameLabel = GetNode<RichTextLabel>("%SpeakerName");
        _dialogueTextLabel = GetNode<RichTextLabel>("%DialogueText");
        _background = GetNode<TextureRect>("%Background");

        DisplayDialogue();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("Click"))
        {
            _dialogueLineIndex++;

            if (_dialogueLineIndex >= CurrentDialogue.Lines.Count)
            {
                GlobalState.Instance.SwitchToGamePhase();
                return;
            }

            DisplayDialogue();
        }
    }

    private void DisplayDialogue()
    {
        if (_dialogueLineIndex >= CurrentDialogue.Lines.Count)
            return;

        var line = CurrentDialogue.Lines[_dialogueLineIndex];
        _background.Texture = GetBackground(line.Background, GlobalState.Instance.Timeline);

        _namePanel.Visible = IsInstanceValid(line.Speaker);
        if (_namePanel.Visible)
            _speakerNameLabel.Text = line.Speaker.Name;

        foreach (var child in GetChildren())
            child.QueueFree();

        foreach (var character in line.Characters)
        {
            var newSprite = _characterScene.Instantiate<TextureRect>();
            newSprite.Texture = character.GetCharacterBody();
            newSprite.GetNode<TextureRect>("Head").Texture = character.GetCharacterHead();
            newSprite.Scale = new Vector2(character.Scale, character.Scale);
            AddChild(newSprite);
        }

        _dialogueTextLabel.Text = line.Text;
    }

    private Texture2D GetBackground(BackgroundType type, FutureTimeline timeline) =>
        type switch
        {
            BackgroundType.Bar => _barBackground.GetVariant(timeline),
            BackgroundType.Tv => _tvBackground.GetVariant(timeline),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
}