using Godot;

[GlobalClass]
public partial class DialogueLine : Resource
{
    [Export] public DialogueCharacter Character { get; private set; }
    [Export] public int Pose { get; private set; } = -1;
    [Export] public int PositionX { get; private set; }
    [Export] public int PositionY { get; private set; }
    [Export(PropertyHint.MultilineText)] public string Text { get; private set; }
}