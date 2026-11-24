using Godot;
using Godot.Collections;

[GlobalClass]
public partial class TutorialStep : RichTextLabel
{
    [Export] private Array<CanvasItem> _highlightNodes;

    public void SetActive(bool active)
    {
        Visible = active;
        if (_highlightNodes != null)
            foreach (var node in _highlightNodes)
                node.ZIndex = active ? 1 : 0;
    }
}