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
    public AnimationPlayer AnimationPlayer;
    private string AnimationString = "";
    private Timer timer;
    private Vector2 LastRegisteredDirection = Vector2.Down;
    public CollisionShape2D colision;
    public override void _Ready()
    {
        ScreenSize = GetViewportRect().Size;
        AnimatorSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        colision = GetNode<CollisionShape2D>("CollisionShape2D");
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
        if (Input.IsActionPressed("Run"))
        {
            Speed = 125f;
            AnimationPlayer.SpeedScale = 1.25f;
        }
        else if(Input.IsActionJustReleased("Run"))
        {
            Speed = 100f;
            AnimationPlayer.SpeedScale = 1f;
        }

        MoveAndSlide();
        GD.Print("X: " + input.X + " Y: " + input.Y);
    }
    void AnimationChecker()
    {
        //Variables (normalmente solo se vuelve a registrar el input
        //           y alguna cosa que se quiera modificar del objeto)
        Vector2 input = Input.GetVector("MoveLeft", "MoveRight", "MoveUp", "MoveDown");
        var rectShape = colision.Shape as RectangleShape2D;

        #region Deteccion de Movimiento
        if (input != Vector2.Zero)
        {
            LastRegisteredDirection = input.Normalized();
        }
        if (input == Vector2.Zero)
        {
            if (LastRegisteredDirection.Y < 0)
            {
                colision.Position = new Vector2(-1.5f, 16.5f);
                rectShape.Size = new Vector2(15.0f, 4.25f);
                AnimationController("IdleUp");
            }
            if (LastRegisteredDirection.Y > 0)
            {
                colision.Position = new Vector2(-1.5f, 16.5f);
                rectShape.Size = new Vector2(15.0f, 4.25f);
                AnimationController("IdleDown");
            }
            if (LastRegisteredDirection.X < 0)
            {
                colision.Position = new Vector2(-1.5f, 16.5f);
                rectShape.Size = new Vector2(15.0f, 4.25f);
                AnimationController("IdleLeft");
            }
            if (LastRegisteredDirection.X > 0)
            {
                colision.Position = new Vector2(1.5f, 16.5f);
                rectShape.Size = new Vector2(15.0f, 4.25f);
                AnimationController("IdleRight");
            }
        }
        if (input.X > 0 && input.Y == 0)
        {
            colision.Position = new Vector2(1.5f, 16.5f);
            rectShape.Size = new Vector2(15.0f, 4.25f);
            AnimationController("WalkRight");
        }
        if (input.X < 0 && input.Y == 0)
        {
            colision.Position = new Vector2(-1.5f, 16.5f);
            rectShape.Size = new Vector2(15.0f, 4.25f);
            AnimationController("WalkLeft");
        }
        if (input.X == 0 && input.Y < 0)
        {
            colision.Position = new Vector2(-1.5f, 16.5f);
            rectShape.Size = new Vector2(15.0f, 4.25f);
            AnimationController("WalkUp");
        }
        if (input.X == 0 && input.Y > 0)
        {
            colision.Position = new Vector2(-1.5f, 16.5f);
            rectShape.Size = new Vector2(15.0f, 4.25f);
            AnimationController("WalkDown");
        }
        if (input.X > 0 && input.Y < 0)
        {
            AnimationController("WalkUp");
        }
        if (input.X < 0 && input.Y < 0)
        {
            colision.Position = new Vector2(-1.5f, 16.5f);
            rectShape.Size = new Vector2(15.0f, 4.25f);
            AnimationController("WalkUp");
        }
        if (input.X > 0 && input.Y > 0)
        {
            colision.Position = new Vector2(-1.5f, 16.5f);
            rectShape.Size = new Vector2(15.0f, 4.25f);
            AnimationController("WalkDown");
        }
        if (input.X < 0 && input.Y > 0)
        {
            colision.Position = new Vector2(-1.5f, 16.5f);
            rectShape.Size = new Vector2(15.0f, 4.25f);
            AnimationController("WalkDown");
        }
        #endregion

    }
    /*Funcionamiento general de la funcion: 
     *La funcion de AnimationController funciona de
     *la siguiente forma la funcion va a recibir un string
     *con el nombre de la animacion (en el motor) y este
     *va a evaluar si el AnimationString que vendria siendo la animacion
     *actual que se esta reproduciendo si no son iguales 
     *la animacion actual va a cambiar a la ingresada en el valor que 
     *pide la funcion.
    */
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
                AnimationPlayer.Play(AnimationString);
            }
        }
    }






}
