using System;
using Godot;

[GlobalClass]
public partial class DialogueDay : Resource
{
    [Export] private DialogueData _fascistVariant;
    [Export] private DialogueData _capitalistVariant;
    [Export] private DialogueData _liberalVariant;
    [Export] private DialogueData _leftistVariant;

    public DialogueData GetVariant(FutureTimeline timeline) =>
        timeline switch
        {
            FutureTimeline.Fascist => _fascistVariant,
            FutureTimeline.Capitalist => _capitalistVariant,
            FutureTimeline.Liberal => _liberalVariant,
            FutureTimeline.Leftist => _leftistVariant,
            _ => throw new ArgumentOutOfRangeException(nameof(timeline), timeline, null)
        };
}