using Godot;

[GlobalClass]
public partial class DialogueCharacter : Resource
{
    [Export] public string Name { get; private set; }
}