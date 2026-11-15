using Godot;

[GlobalClass]
public partial class DialogueManager : Node
{
    [Export] public DialogueData DialogueData;

    private RichTextLabel _speakerNameLabel;
    private RichTextLabel _dialogueTextLabel;

    private int _dialogueLineIndex;

    public override void _Ready()
    {
        _speakerNameLabel = GetNode<RichTextLabel>("%SpeakerName");
        _dialogueTextLabel = GetNode<RichTextLabel>("%DialogueText");

        DisplayDialogue();
    }

    private void DisplayDialogue()
    {
        var character = DialogueData.Lines[_dialogueLineIndex].Character;
        if (IsInstanceValid(character))
            _speakerNameLabel.Text = character.Name;

        _dialogueTextLabel.Text = DialogueData.Lines[_dialogueLineIndex].Text;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("Click"))
        {
            _dialogueLineIndex++;

            if (_dialogueLineIndex >= DialogueData.Lines.Count)
            {
                GetTree().ChangeSceneToFile("res://scenes/JeanMace.tscn");
                return;
            }

            DisplayDialogue();
        }
    }
}