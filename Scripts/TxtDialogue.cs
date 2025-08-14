using Godot;
using System;

[GlobalClass]
public partial class TxtDialogue : DE
{
    [Export] public String speaker_name;
    [Export] public Texture2D speaker_img;
    [Export] public int speaker_img_Hframes = 5;
    [Export] public int speaker_img_rest_frame = 0;

    [Export(PropertyHint.MultilineText)] public new string text;
    [Export(PropertyHint.Range, "0.1,30.0,0.1")] public float text_speed = 1.0f;

    [Export] public AudioStream text_sound;
    [Export] public int text_volume_db;
    [Export] public float text_volume_pitch_min = 0.85f;
    [Export] public float text_volume_pitch_max = 1.15f;

    [Export] public Vector2 camera_position = new Vector2(999.999f, 999.999f);
    [Export(PropertyHint.Range, "0.05,10.0,0.05")] public float camera_transition_time = 1.0f;
}
