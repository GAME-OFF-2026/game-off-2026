using Godot;

[GlobalClass]
public partial class GameManager : Node
{
    [Signal]
    public delegate void SetupPhaseEndEventHandler();

    public GamePhase GamePhase { get; private set; } = GamePhase.Setup;

    public void EndSetupPhase()
    {
        if (GamePhase != GamePhase.Setup)
        {
            GD.PrintErr("GameManager: EndSetupPhase called in phase: " + GamePhase);
            return;
        }

        GamePhase = GamePhase.Protest;
        EmitSignalSetupPhaseEnd();
    }
}