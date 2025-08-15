using Godot;
using System;

[GlobalClass]
public partial class DialogChoice : DE
{
    [Export] public Texture2D speaker_img;
    [Export] public int speaker_img_Hframes = 1;
    [Export] public int speaker_img_select_frame = 0;

    [Export(PropertyHint.MultilineText)] public new string text;

    [Export] public Godot.Collections.Array<String> txt_seleccion { get; set; }
    [Export] public Godot.Collections.Array<DialogFunc> func_call_seleccion { get; set; }

}
