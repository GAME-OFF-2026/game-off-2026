using Godot;

[GlobalClass]
public partial class DialogueLine : Resource
{
    [Export] public DialogueCharacter Character { get; private set; }
    [Export(PropertyHint.MultilineText)] public string Text { get; private set; }
}