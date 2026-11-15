using System.Collections.Generic;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class DialogueData : Resource
{
    [Export] private Array<DialogueLine> _lines = [];

    public IList<DialogueLine> Lines => _lines;
}