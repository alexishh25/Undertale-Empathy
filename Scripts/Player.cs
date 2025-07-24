using Godot;
using System;
using System.Diagnostics;
using System.Collections;
using System.Threading.Tasks;
using static Godot.TextServer;
public partial class Player : CharacterBody2D
{
    [Export]
    public float Speed { get; set; } = 400;
    public Vector2 ScreenSize;
    public AnimatedSprite2D AnimatorSprite;
    private string AnimationString = "";
    private Timer timer;
    private Vector2 LastRegisteredDirection = Vector2.Down;
    public override void _Ready()
    {
        ScreenSize = GetViewportRect().Size;
        AnimatorSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        timer = GetNode<Timer>("Timer");
    }
    public override void _Process(double delta)
    {
        MovementControl();
        AnimationChecker();
    }
    void MovementControl()
    {
        Vector2 input = Input.GetVector("MoveLeft", "MoveRight", "MoveUp", "MoveDown");
        Velocity = input.Normalized() * Speed;
        MoveAndSlide();
        GD.Print("X: " + input.X + "Y: " + input.Y);
    }
    void AnimationChecker()
    {
        Vector2 input = Input.GetVector("MoveLeft", "MoveRight", "MoveUp", "MoveDown");
        if(input != Vector2.Zero)
        {
            LastRegisteredDirection = input.Normalized();
        }
        if(input == Vector2.Zero)
        {
            if(LastRegisteredDirection.Y < 0)
            {
                AnimatorSprite.FlipH = false;
                AnimationController("IdleUp");
            }
            if (LastRegisteredDirection.Y > 0)
            {
                AnimatorSprite.FlipH = false;
                AnimationController("IdleDown");
            }
            if (LastRegisteredDirection.X < 0)
            {
                AnimatorSprite.FlipH = false;
                AnimationController("IdleLeft");
            }
            if (LastRegisteredDirection.X > 0)
            {
                AnimatorSprite.FlipH = true;
                AnimationController("IdleLeft");
            }
        }
        if(input.X > 0 && input.Y == 0)
        {
            AnimatorSprite.FlipH = true;
            AnimationController("WalkLeft");
        }
        if (input.X < 0 && input.Y == 0)
        {
            AnimatorSprite.FlipH = false;
            AnimationController("WalkLeft");
        }
        if (input.X == 0 && input.Y < 0)
        {
            AnimatorSprite.FlipH = false;
            AnimationController("WalkUp");
        }
        if (input.X == 0 && input.Y > 0)
        {
            AnimatorSprite.FlipH = false;
            AnimationController("WalkDown");
        }
        if (input.X > 0 && input.Y < 0)
        {
            AnimatorSprite.FlipH = false;
            AnimationController("WalkUp");
        }
        if (input.X < 0 && input.Y < 0)
        {
            AnimatorSprite.FlipH = false;
            AnimationController("WalkUp");
        }
        if (input.X > 0 && input.Y > 0)
        {
            AnimatorSprite.FlipH = false;
            AnimationController("WalkDown");
        }
        if (input.X < 0 && input.Y > 0)
        {
            AnimatorSprite.FlipH = false;
            AnimationController("WalkDown");
        }
    }
   async void AnimationController(string animation, float time = 0f)
    {
        if(time > 0)
        {
            timer.WaitTime = time;
            timer.Start();
            await ToSignal(timer, Timer.SignalName.Timeout);
        }
        else
        {
            validateAnim();
        }
        void validateAnim()
        {
            if(AnimationString != animation)
            {
                AnimationString = animation;
                AnimatorSprite.Play(AnimationString);
            }
        }
    }






}
