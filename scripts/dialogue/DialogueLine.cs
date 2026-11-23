using Godot;
using Godot.Collections;

public enum BackgroundType
{
    Bar,
    Tv
}

[GlobalClass]
public partial class DialogueLine : Resource
{
    [Export] public BackgroundType Background { get; private set; }
    [Export] public DialogueCharacter Speaker { get; private set; }
    [Export] public Array<CharacterPose> Characters { get; private set; }
    [Export(PropertyHint.MultilineText)] public string Text { get; private set; }
}