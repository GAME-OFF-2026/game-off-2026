using Godot;
using Godot.Collections;

[GlobalClass]
public partial class DialogueManager : Node
{
    [Export] public Array<DialogueData> DialoguePerDay;

    private Control _namePanel;
    private RichTextLabel _speakerNameLabel;
    private RichTextLabel _dialogueTextLabel;

    private int _dialogueLineIndex;

    private DialogueData CurrentDialogue => DialoguePerDay[Mathf.Clamp(GlobalState.Instance.Day, 0, DialoguePerDay.Count - 1)];

    public override void _Ready()
    {
        _namePanel = GetNode<Control>("%NamePanel");
        _speakerNameLabel = GetNode<RichTextLabel>("%SpeakerName");
        _dialogueTextLabel = GetNode<RichTextLabel>("%DialogueText");

        DisplayDialogue();
    }

    private void DisplayDialogue()
    {
        var line = CurrentDialogue.Lines[_dialogueLineIndex];

        _namePanel.Visible = IsInstanceValid(line.Character);
        if (_namePanel.Visible)
            _speakerNameLabel.Text = line.Character.Name;

        _dialogueTextLabel.Text = line.Text;
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
}