using System;
using Godot;

[GlobalClass]
public partial class TimelineVariant : Resource
{
    [Export] private Texture2D _fascistVariant;
    [Export] private Texture2D _capitalistVariant;
    [Export] private Texture2D _liberalVariant;
    [Export] private Texture2D _leftistVariant;

    public Texture2D GetVariant(FutureTimeline timeline) =>
        timeline switch
        {
            FutureTimeline.Fascist => _fascistVariant,
            FutureTimeline.Capitalist => _capitalistVariant,
            FutureTimeline.Liberal => _liberalVariant,
            FutureTimeline.Leftist => _leftistVariant,
            _ => throw new ArgumentOutOfRangeException(nameof(timeline), timeline, null)
        };
}