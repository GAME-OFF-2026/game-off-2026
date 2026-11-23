using Godot;

[GlobalClass]
public partial class DialogueCharacter : Resource
{
    [Export] public string Name { get; private set; }
    [Export] public TimelineVariant[] Body { get; private set; }
    [Export] public TimelineVariant[] Head { get; private set; }
}