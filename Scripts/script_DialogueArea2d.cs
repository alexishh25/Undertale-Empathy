using Godot;
using System;

public partial class script_DialogueArea2d : Area2D
{
    public static readonly PackedScene DialogueSystemPreload = GD.Load<PackedScene>("res://dialogue_scene.tscn");

    [Export] public bool activate_instant, only_activate_once, override_dialogue_position;
    [Export] public Vector2 override_position;
    [Export] public Godot.Collections.Array<DE> dialogue;

    public Vector2 dialogue_top_pos = new Vector2(160, 48);
    public Vector2 dialogue_bottom_pos = new Vector2(160, 192);

    public bool player_body_in = false;
    public bool has_activated_already = false;
    public Vector2 desired_dialogue_pos;

    public CharacterBody2D player_node = null;

    public override void _Ready()
    {
        foreach (Node i in GetTree().GetNodesInGroup("Player"))
            player_node = i as CharacterBody2D;
    }

    public override void _Process(double delta)
    {
        if (player_node == null)
        {
            foreach (Node i in GetTree().GetNodesInGroup("Player"))
                player_node = i as CharacterBody2D;
            return;
        }
    }

    public void _activate_dialogue()
    {
        if (player_node is Player p)
            p.podermoverse = false;

        var new_dialogue = DialogueSystemPreload.Instantiate() as script_DialogueArea2d;
        if (override_dialogue_position)
            desired_dialogue_pos = override_position;
        else
        {
            if (player_node.GlobalPosition.Y > GetViewport().GetCamera2D().GetScreenCenterPosition().Y)
                desired_dialogue_pos = dialogue_top_pos;
            else
                desired_dialogue_pos = dialogue_bottom_pos;
        }
        new_dialogue.GlobalPosition = desired_dialogue_pos;
        new_dialogue.dialogue = dialogue;
        GetParent().AddChild(new_dialogue);
    }
    public void _on_body_entered(Node2D body)
    {
        if (only_activate_once && has_activated_already)
        {
            SetProcess(false);
            return;
        }
        if (Input.IsActionJustPressed("ui_accept"))
        {
            _activate_dialogue();
            player_body_in = false;
        }
    }
    public void _on_body_exited(Node2D body)
    {

    }
}
