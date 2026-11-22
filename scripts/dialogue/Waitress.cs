using Godot;
using System;
using Godot;
using System.Linq;



[GlobalClass]
public partial class Waitress : Node
{
    [Export] public WaitressPhaseData[] Phases { get; set; } = new WaitressPhaseData[4];


	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

	public void SetWaitressPhase(FutureTimeline timeline, int pose)
	{
		if (Phases.Count() > (int)timeline && pose >= 0 && Phases[(int)timeline].Corps.Count() > pose)
			GetNode<Sprite2D>("%Body").Texture = Phases[(int)timeline].Corps[pose];
		if (Phases.Count() > (int)timeline && pose >= 0 &&  Phases[(int)timeline].Tetes.Count() > pose)
			GetNode<Sprite2D>("%Head").Texture = Phases[(int)timeline].Tetes[pose];
	}

}
