using Godot;

using System;
using System.Threading.Tasks;

public partial class TextBoxSystem : Node2D
{
    public static readonly PackedScene DialogueButtonPreload = GD.Load<PackedScene>("res://Scenes/Btn_Dialog.tscn");

    private RichTextLabel DialogueLabel;
    private Sprite2D SpeakerSprite;

    private Godot.Collections.Array<DE> dialogue = new Godot.Collections.Array<DE>();
    private int current_Dialogue_item = 0;
    private bool nextItem = true;

    private CharacterBody2D player_node;

    public override void _Ready()
    {
        DialogueLabel = GetNode<RichTextLabel>("HBoxContainer/VBoxContainer/RichTextLabel");
        SpeakerSprite = GetNode<Sprite2D>("HBoxContainer/SpeakerParent/Sprite2D");

        Visible = false;
        GetNode<Control>("HBoxContainer/VBoxContainer/Button_Container").Visible = false;

        foreach (Node i in GetTree().GetNodesInGroup("player")) {
            player_node = i as CharacterBody2D;
            if (player_node != null)
                break;
        }
    }

    public override void _Process(double delta)
    {
        if (current_Dialogue_item == dialogue.Count) 
        {
            if (player_node == null) 
            { 
                foreach (Node i in GetTree().GetNodesInGroup("Player"))
                {
                    player_node = i as CharacterBody2D;
                    return;
                }
            }
            if (player_node != null)
                player_node.Set("moverse", true);
            QueueFree();
            return;
        }
        if (nextItem)
        {
            nextItem = false;
            var i = dialogue[current_Dialogue_item];

            if (i is DialogFunc df)
            {
                Visible = !df.hide_dialogue_box;
                _FuncionResource(df);
            }
            else if (i is DialogChoice dc)
            {
                Visible = true;
                _SeleccionResource(dc);
            }
            else if (i is TxtDialogue txtd)
            {
                Visible = true;
                _TxtResource(txtd);
            }
            else
            {
                GD.PrintErr("Accidentalmente a�adiste un recurso DE");
                current_Dialogue_item += 1;
                nextItem = true;
            }
        }
    }

    public async void _FuncionResource(DialogFunc i) {
        var target_node = GetNode(i.target_path);
        if (target_node.HasMethod(i.nom_funcion))
        {
            if (i.arg_funcion.Count == 0)
                target_node.Call(i.nom_funcion);
            else
                target_node.Callv(i.nom_funcion, i.arg_funcion);
        }
        if (!string.IsNullOrEmpty(i.aviso_esperar))
        {
            string nom_aviso = i.aviso_esperar;
            if (target_node.HasSignal(nom_aviso))
            {
                _signalTcs = new TaskCompletionSource();
                target_node.Connect(nom_aviso, new Callable(this, nameof(OnSignalContinue)), (uint)ConnectFlags.OneShot);

                await _signalTcs.Task;
            }
        }
    }
    private TaskCompletionSource _signalTcs;
    private void OnSignalContinue() => _signalTcs.TrySetResult();

    public void _SeleccionResource(DialogChoice i)
    {
        DialogueLabel.Text = i.text;
        DialogueLabel.VisibleCharacters = -1;

        if (i.speaker_img != null)
        {
            GetNode<Control>("HBoxContainer/SpeakerNPC").Visible = true;

            SpeakerSprite.Texture = i.speaker_img;
            SpeakerSprite.Hframes = i.speaker_img_Hframes;
            SpeakerSprite.Frame = Math.Min(i.speaker_img_select_frame, i.speaker_img_Hframes - 1);
        }
        else
        {
            GetNode<Control>("HBoxContainer/SpeakerParent").Visible = false;
            GetNode<Control>("HBoxContainer/VBoxContainer/Button_Container").Visible = true;
        }

        for (int item = 0; item < i.txt_seleccion.Count; item++)
        {
            var DialogBtnVar = (Button)DialogueButtonPreload.Instantiate();
            DialogBtnVar.Text = i.txt_seleccion[item];

            DialogFunc recurso_func = i.func_call_seleccion[item];

            if (recurso_func != null)
            {
                Node target_nodo = GetNode(recurso_func.target_path);

                Callable callable = new Callable(target_nodo, recurso_func.nom_funcion);
                DialogBtnVar.Connect(
                    "pressed",
                    callable,
                    (uint)ConnectFlags.OneShot
                );
                
                if (recurso_func.hide_dialogue_box)
                {
                    DialogBtnVar.Connect(
                        "pressed",
                        new Callable(this, "Hide"),
                        (uint)ConnectFlags.OneShot
                    );
                }
            }
            var container = GetNode<Container>("HBoxContainer/VBoxContainer/Button_Container");
            container.AddChild(DialogBtnVar);

            if (item == 0)
                DialogBtnVar.GrabFocus();
        }

    }

    public async void _Btn_Pressed_Choice(Node target_nodo, String wait_for_signal_to_continue)
    {
        GetNode<Control>("HBoxContainer/VBoxContainer/Button_Container").Visible = false;

        foreach (Node i in GetNode("HBoxContainer/VBoxContainer/Button_Container").GetChildren())
            i.QueueFree();
        if (wait_for_signal_to_continue != null)
        {
            string nom_aviso = wait_for_signal_to_continue;

            if (target_nodo.HasSignal(nom_aviso))
            {
                bool hecho = false;
                target_nodo.Connect(nom_aviso, Callable.From(() => hecho = true));
                while (!hecho)
                    await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            }
        }
        current_Dialogue_item += 1;
        nextItem = true;
    }

    public async void _TxtResource(TxtDialogue i)
    {
        GetNode<AudioStreamPlayer>("AudioStreamPlayer").Stream = i.text_sound;
        GetNode<AudioStreamPlayer>("AudioStreamPlayer").VolumeDb = i.text_volume_db;

        Camera2D camera = GetViewport().GetCamera2D();

        if (camera != null && i.camera_position != new Vector2(999.999f, 999.999f))
        {
            Tween cameratween = CreateTween().SetTrans(Tween.TransitionType.Sine);
            cameratween.TweenProperty(camera, "global_position", i.camera_position, i.camera_transition_time);
        }

        var speakerparent = GetNode<Control>("HboxContainer/SpeakerParent");
        if (i.speaker_img == null)
            speakerparent.Visible = false;
        else
        {
            speakerparent.Visible = true;
            var speakerSprite = GetNode<Sprite2D>("SpeakerSprite");
            speakerSprite.Texture = i.speaker_img;
            speakerSprite.Hframes = i.speaker_img_Hframes;
            speakerSprite.Frame = 0;
        }
        DialogueLabel.VisibleCharacters = 0;
        DialogueLabel.Text = i.text;

        string txt_sin_forma = _Txt_sin_forma(i.text);
        int total_caracteres = txt_sin_forma.Length;
        float timer_caracter = 0.0f;

        while (DialogueLabel.VisibleCharacters < total_caracteres)
        {
            if (Input.IsActionJustPressed("ui_cancel"))
            {
                DialogueLabel.VisibleCharacters = total_caracteres;
                break;
            }

            timer_caracter += (float)GetProcessDeltaTime();
            if (timer_caracter >= (1.0f / i.text_speed) || txt_sin_forma[DialogueLabel.VisibleCharacters] == ' ')
            {
                char _caracter = txt_sin_forma[DialogueLabel.VisibleCharacters];
                DialogueLabel.VisibleCharacters += 1;
                if (_caracter != ' ')
                {
                    var audiostream = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
                    audiostream.PitchScale = (float)GD.RandRange(i.text_volume_pitch_min, i.text_volume_pitch_max);
                    audiostream.Play();
                    if (i.speaker_img_Hframes != 1)
                    {
                        if (SpeakerSprite.Frame < i.speaker_img_Hframes - 1)
                            SpeakerSprite.Frame += 1;
                        else
                            SpeakerSprite.Frame = 0;
                    }
                }
                timer_caracter = 0.0f;
            }
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        }
        SpeakerSprite.Frame = (int)MathF.Min(i.speaker_img_rest_frame, i.speaker_img_Hframes - 1);

        while (true)
        {
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            if (DialogueLabel.VisibleCharacters == total_caracteres)
            {
                if (Input.IsActionJustPressed("ui_accept"))
                {
                    current_Dialogue_item += 1;
                    nextItem = true;
                }
            }
        }
    }

    public String _Txt_sin_forma(String texto)
    {
        string resultado = "";
        bool inside_bracket = false;

        foreach (char i in texto)
        {
            if (i == '[')
            {
                inside_bracket = true;
                continue;
            }
            if (i == ']')
            {
                inside_bracket = false;
                continue;
            }
            if (!inside_bracket)
                resultado += i;
        }
        return resultado;
    }

}
