using Godot;
using System;
using System.Text.RegularExpressions;

public partial class BatEnemy : CharacterBody2D
{
    private const int _Range = 128;
    private const int _Speed = 30;
    
    private int _KnockBack = 0;
    private bool IsHit = false;
    private int _MaxHealth = 3;
    private int Health = 3;
    
    private Sprite2D _batSprite;
    private AnimationTree _animationTree;
    private AnimationNodeStateMachinePlayback _playback;
    private RayCast2D _raycast;
    private Area2D _area2D;
    
    private static readonly PackedScene DeathEffect = GD.Load<PackedScene>("res://effects/enemy_death_effect.tscn");
    
    [Export]
    public Resource Stats { get; set; }

    public override void _Ready()
    {
        base._Ready();
        _batSprite = GetNode<Sprite2D>("Bat");
        _animationTree = GetNode<AnimationTree>("AnimationTree");
        _raycast = GetNode<RayCast2D>("RayCast2D");
        _area2D = GetNode<Area2D>("Area2D");

        _playback = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/StateMachine/playback");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (IsHit)
        {
            Velocity = Velocity.MoveToward(Vector2.Zero, (float)(_KnockBack * delta));
            if (Velocity == Vector2.Zero)
            {
                IsHit = false;
            }
        }
        else if (CanSeePlayer())
        {
            _playback.Travel("chase");
        
            var player = GetPlayer();
            if (player != null)
            {
                Velocity = GlobalPosition.DirectionTo(player.GlobalPosition) * _Speed;
                _batSprite.Scale = _batSprite.Scale with { X = Math.Sign(Velocity.X) };
            }

        }
        else
        {
            _playback.Travel("idle");
            Velocity = Vector2.Zero;
        }

        MoveAndSlide();
    }
    
    private Player GetPlayer()
    {
        return GetTree().GetFirstNodeInGroup("player") as Player;
    }

    private bool IsPlayerInRange()
    {
        bool result = false;
        var player = GetPlayer();
        if (player != null)
        {
            var distanceToPlayer = GlobalPosition.DistanceTo(player.GlobalPosition);
            if (distanceToPlayer < _Range)
            {
                result = true;
            }
        }
        return result;
    }

    private bool CanSeePlayer()
    {
        if (!IsPlayerInRange())
        {
            return false;
        }

        Player player = GetPlayer();
        _raycast.TargetPosition = player.GlobalPosition - GlobalPosition;
        bool IsLineOfSightBlocked = _raycast.IsColliding();
        return !IsLineOfSightBlocked;
    }

    private void OnHurt(Area2D hitbox)
    {
        IsHit = true;
        if (Health-- == 0)
        {
            Node2D deathEffectInstance = DeathEffect.Instantiate<Node2D>();
            GetTree().CurrentScene.AddChild(deathEffectInstance);
            deathEffectInstance.GlobalPosition = GlobalPosition;
            QueueFree();
        }
        Velocity = ((HitBox)hitbox).KnockbackDirection * 100;
        _KnockBack = ((HitBox)hitbox).KnockbackAmount;
        _playback.Travel("hit");
    }
}
