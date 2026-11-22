using Godot;
using Godot.Collections;

[GlobalClass]
public partial class DialogueManager : Node
{
    [Export] public Array<DialogueData> DialoguePerDay;

    private Control _namePanel;
    private RichTextLabel _speakerNameLabel;
    private RichTextLabel _dialogueTextLabel;
    private Waitress _waitress;
    private Bar _bar;

    private int _dialogueLineIndex;

    private DialogueData CurrentDialogue => DialoguePerDay[Mathf.Clamp(GlobalState.Instance.Day, 0, DialoguePerDay.Count - 1)];

    public override void _Ready()
    {
        _namePanel = GetNode<Control>("%NamePanel");
        _speakerNameLabel = GetNode<RichTextLabel>("%SpeakerName");
        _dialogueTextLabel = GetNode<RichTextLabel>("%DialogueText");
        _waitress = GetNode<Waitress>("%Waitress");
        _bar = GetNode<Bar>("%Bar");

        DisplayDialogue();
    }

    private void DisplayDialogue()
    {
        if (_dialogueLineIndex < CurrentDialogue.Lines.Count)
        {
            var line = CurrentDialogue.Lines[_dialogueLineIndex];
            var pose = line.Pose;
            _waitress.SetWaitressPhase(GlobalState.Instance.Timeline, pose);
            _bar.SetBarPhase(GlobalState.Instance.Timeline);

            _namePanel.Visible = IsInstanceValid(line.Character);
            if (_namePanel.Visible)
                _speakerNameLabel.Text = line.Character.Name;

            _dialogueTextLabel.Text = line.Text;
        }
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