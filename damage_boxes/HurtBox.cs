using Godot;
using System;

public partial class HurtBox : Area2D
{
    [Signal]
    public delegate void HurtEventHandler(Area2D hitbox);
	
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        AreaEntered += _OnAreaEntered;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
	
    private void _OnAreaEntered(Area2D area2D)
    {
        if (area2D is not HitBox) return;
        EmitSignal(SignalName.Hurt, area2D);
    }
}
