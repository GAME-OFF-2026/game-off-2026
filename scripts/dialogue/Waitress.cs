using Godot;

[GlobalClass]
public partial class Waitress : Node
{
    private int _pose;
    [Export] public WaitressPhaseData[] Phases { get; set; } = new WaitressPhaseData[4];

    public void SetWaitressPhase(FutureTimeline timeline, int pose)
    {
        if (pose != -1)
            _pose = pose;

        GetNode<Sprite2D>("%Body").Texture = Phases[(int)timeline].Corps[_pose];

        GetNode<Sprite2D>("%Head").Texture = Phases[(int)timeline].Tetes[_pose];
    }
}