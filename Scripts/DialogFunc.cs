using Godot;
using System;
using System.Collections;

[GlobalClass]
public partial class DialogFunc : DE
{
    [Export] public NodePath target_path;
    [Export] public String nom_funcion;
    [Export] public Godot.Collections.Array arg_funcion { get; set; }

    [Export] public bool hide_dialogue_box;
    [Export] public String aviso_esperar = "";
}
