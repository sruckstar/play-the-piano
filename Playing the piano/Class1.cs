using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Media;
using GTA;
using GTA.Native;
using GTA.Math;


public class PlayPiano : Script
{
    SoundPlayer[] classical_note = new SoundPlayer[12];
    Prop piano;
    Prop Bench;
    int PianoCreated = 0;
    Vector3 savepos;

    public PlayPiano()
    {
        KeyUp += onkeyup;
        Aborted += OnAborted;
        Tick += OnTick;

        classical_note[0] = new SoundPlayer(@".\scripts\PianoMod\classical\C.wav");
        classical_note[1] = new SoundPlayer(@".\scripts\PianoMod\classical\D.wav");
        classical_note[2] = new SoundPlayer(@".\scripts\PianoMod\classical\E.wav");
        classical_note[3] = new SoundPlayer(@".\scripts\PianoMod\classical\F.wav");
        classical_note[4] = new SoundPlayer(@".\scripts\PianoMod\classical\G.wav");
        classical_note[5] = new SoundPlayer(@".\scripts\PianoMod\classical\A.wav");
        classical_note[6] = new SoundPlayer(@".\scripts\PianoMod\classical\B.wav");
        classical_note[7] = new SoundPlayer(@".\scripts\PianoMod\classical\Csharp.wav");
        classical_note[8] = new SoundPlayer(@".\scripts\PianoMod\classical\Dsharp.wav");
        classical_note[9] = new SoundPlayer(@".\scripts\PianoMod\classical\Fsharp.wav");
        classical_note[10] = new SoundPlayer(@".\scripts\PianoMod\classical\Gsharp.wav");
        classical_note[11] = new SoundPlayer(@".\scripts\PianoMod\classical\Asharp.wav");
    }

    //-815.7832f, 178.7619f, 76.74049f 197.1852f //Майкл
    //-2.35635f, 521.7017f, 174.6271f 295.4398f //Франклин

    private void OnTick(object sender, EventArgs e)
    {
        int interior_michel = Function.Call<int>(Hash.GET_INTERIOR_AT_COORDS, -815.7832f, 178.7619f, 76.74049f);
        int interior_franklin = Function.Call<int>(Hash.GET_INTERIOR_AT_COORDS, -2.35635f, 521.7017f, 174.6271f);
        int current_interior = Function.Call<int>(Hash.GET_INTERIOR_FROM_ENTITY, Game.Player.Character);

        if (current_interior == interior_michel && PianoCreated == 0)
        {
            Vector3 pos = new Vector3(-815.7832f, 178.7619f, 76.74049f);
            Vector3 pos_2 = new Vector3(-815.3106f, 177.2333f, 77.23698f);
            CreatePiano(pos, pos_2, 197.1852f);
            PianoCreated = 1;
        }
        else
        {
            if (current_interior == interior_franklin && PianoCreated == 0)
            {
                Vector3 pos = new Vector3(-2.35635f, 521.7017f, 174.6271f);
                Vector3 pos_2 = new Vector3(-0.9114903f, 522.389f, 175.1208f);
                CreatePiano(pos, pos_2, 295.4398f);
                PianoCreated = 1;
            }
        }

        if (PianoCreated > 0)
        {
            if (World.GetDistance(Game.Player.Character.Position, Bench.Position) < 1.5f && PianoCreated == 1)
            {
                GTA.UI.Screen.ShowHelpTextThisFrame("Press ~INPUT_FRONTEND_Y~ to play the piano.");
            }

            if (World.GetDistance(Game.Player.Character.Position, Bench.Position) < 1.5f && Function.Call<bool>(Hash.IS_CONTROL_PRESSED, 0, 204) && PianoCreated == 1)
            {
                Function.Call(Hash.DO_SCREEN_FADE_OUT, 500);
                while (Function.Call<bool>(Hash.IS_SCREEN_FADED_OUT) == false) Script.Wait(100);
                PutPlayerOnThePiano();
                Wait(1000);
                Function.Call(Hash.DO_SCREEN_FADE_IN, 500);

            }
        }

        if (PianoCreated == 2 && Function.Call<bool>(Hash.IS_CONTROL_PRESSED, 0, 204))
        {
            Function.Call(Hash.DO_SCREEN_FADE_OUT, 1000);
            while (Function.Call<bool>(Hash.IS_SCREEN_FADED_OUT) == false) Script.Wait(100);

            
            if (current_interior == interior_franklin)
            {
                Game.Player.Character.Position = new Vector3(-3.009442f, 523.4519f, 174.6271f);
                Game.Player.Character.Heading = 27.99888f;
            }
            else
            {
                if (current_interior == interior_michel)
                {
                    Game.Player.Character.Position = new Vector3(-814.3486f, 178.0791f, 76.74078f);
                    Game.Player.Character.Heading = 292.0789f;
                }
            }

            Game.Player.Character.Task.ClearAll();
            Function.Call(Hash.FREEZE_ENTITY_POSITION, Game.Player.Character, false);
            Wait(1000);
            Function.Call(Hash.DO_SCREEN_FADE_IN, 1000);
            PianoCreated = 1;
        }

        if (PianoCreated == 1)
        {
            current_interior = Function.Call<int>(Hash.GET_INTERIOR_FROM_ENTITY, Game.Player.Character);
            if (current_interior != interior_michel && current_interior != interior_franklin)
            {
                DeletePiano();
                PianoCreated = 0;
            }
        }

    }

    void DeletePiano()
    {
        if (piano != null && piano.Exists())
        {
            piano.Delete();
        }
        if (Bench != null && Bench.Exists())
        {
            Bench.Delete();
        }
    }

    private void OnAborted(object sender, EventArgs e)
    {
        DeletePiano();
        Function.Call(Hash.FREEZE_ENTITY_POSITION, Game.Player.Character, false);
        Game.Player.Character.Task.ClearAll();
    }

    private void onkeyup(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.A && PianoCreated == 2)
        {
            PlayNote(0); //C
        }
        else
        {
            if (e.KeyCode == Keys.W && PianoCreated == 2)
            {
                PlayNote(7); //C sharp
            }
            else
            {
                if (e.KeyCode == Keys.S && PianoCreated == 2)
                {
                    PlayNote(1); //D
                }
                else
                {
                    if (e.KeyCode == Keys.E && PianoCreated == 2)
                    {
                        PlayNote(8); //D sharp
                    }
                    else
                    {
                        if (e.KeyCode == Keys.D && PianoCreated == 2)
                        {
                            PlayNote(2); //E
                        }
                        else
                        {
                            if (e.KeyCode == Keys.F && PianoCreated == 2)
                            {
                                PlayNote(3); //F
                            }
                            else
                            {
                                if (e.KeyCode == Keys.T && PianoCreated == 2)
                                {
                                    PlayNote(9); //F sharp
                                }
                                else
                                {
                                    if (e.KeyCode == Keys.G && PianoCreated == 2)
                                    {
                                        PlayNote(4); //G
                                    }
                                    else
                                    {
                                        if (e.KeyCode == Keys.Y && PianoCreated == 2)
                                        {
                                            PlayNote(10); //G sharp
                                        }
                                        else
                                        {
                                            if (e.KeyCode == Keys.H && PianoCreated == 2)
                                            {
                                                PlayNote(5); //A
                                            }
                                            else
                                            {
                                                if (e.KeyCode == Keys.U && PianoCreated == 2)
                                                {
                                                    PlayNote(11); //A sharp
                                                }
                                                else
                                                {
                                                    if (e.KeyCode == Keys.J && PianoCreated == 2)
                                                    {
                                                        PlayNote(6); //B
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void CreatePiano(Vector3 pos, Vector3 pos_2, float heading)
    {
        var temp_model_1 = new Model("sf_prop_sf_bench_piano_01a");
        temp_model_1.Request(250);
        if (temp_model_1.IsInCdImage && temp_model_1.IsValid)
        {
            while (!temp_model_1.IsLoaded) Script.Wait(50);
            Bench = World.CreateProp(temp_model_1, pos, true, true);
            Bench.Heading = heading;
        }
        temp_model_1.MarkAsNoLongerNeeded();
        Function.Call(Hash.PLACE_OBJECT_ON_GROUND_PROPERLY, Bench);
        Function.Call(Hash.FREEZE_ENTITY_POSITION, Bench, true);
        Function.Call(Hash.SET_ENTITY_COLLISION, Bench, false, false);


        var temp_model_2 = new Model("sf_prop_sf_piano_01a");
        temp_model_2.Request(250);

        if (temp_model_2.IsInCdImage && temp_model_2.IsValid)
        {
            while (!temp_model_2.IsLoaded) Script.Wait(50);
            piano = World.CreateProp(temp_model_2, pos_2, true, true);
            piano.Heading = heading;
            Function.Call(Hash.PLACE_OBJECT_ON_GROUND_PROPERLY, piano);
            Function.Call(Hash.FREEZE_ENTITY_POSITION, piano, true);
        }
        temp_model_2.MarkAsNoLongerNeeded();
        PianoCreated = 1;
    }

    private void PutPlayerOnThePiano()
    {
        GTA.Native.Function.Call(GTA.Native.Hash.REQUEST_ANIM_DICT, "anim@amb@board_room@stenographer@computer@");
        while (GTA.Native.Function.Call<bool>(GTA.Native.Hash.HAS_ANIM_DICT_LOADED, "anim@amb@board_room@stenographer@computer@") == false) Script.Wait(100);

        Vector3 pos = Bench.Position;
        float heading = Bench.Heading;
        Game.Player.Character.Position = pos;
        Game.Player.Character.Heading = heading;
        Wait(500);
        Game.Player.Character.Task.PlayAnimation("anim@amb@board_room@stenographer@computer@", "base_left_amy_skater_01", 8.0f, -1, GTA.AnimationFlags.Loop);
        Function.Call(Hash.FREEZE_ENTITY_POSITION, Game.Player.Character, true);
        PianoCreated = 2;
    }

    private void PlayNote(int note)
    {
        classical_note[note].Play();
    }
}