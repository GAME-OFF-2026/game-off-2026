using Godot;

[GlobalClass]
public partial class CharacterPose : Resource
{
    [Export] public DialogueCharacter Character { get; private set; }
    [Export] public int Pose { get; private set; } = -1;
    [Export] public int Head { get; private set; } = -1;
    [Export] public int X { get; private set; } = -1;
    [Export] public int Y { get; private set; } = -1;
    [Export] public float Scale { get; private set; } = 1;

    public Texture2D GetCharacterBody() =>
        Character.Body.Length == 0 ? null : Character.Body[Mathf.Clamp(Pose, 0, Character.Body.Length - 1)].GetVariant(GlobalState.Instance.Timeline);

    public Texture2D GetCharacterHead() =>
        Character.Head.Length == 0 ? null : Character.Head[Mathf.Clamp(Head, 0, Character.Head.Length - 1)].GetVariant(GlobalState.Instance.Timeline);
}