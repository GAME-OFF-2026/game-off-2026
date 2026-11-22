using Godot;
using System;

[GlobalClass]
public partial class WaitressPhaseData : Resource
{
    [Export] public Texture2D[] Corps { get; set; } = new Texture2D[3];
    [Export] public Texture2D[] Tetes { get; set; } = new Texture2D[3];
}