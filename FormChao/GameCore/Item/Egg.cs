﻿using System.Drawing.Drawing2D;
using System.Numerics;
using BebooGarden.GameCore.Pet;
using BebooGarden.GameCore.World;
using BebooGarden.Interface.ScriptedScene;
using FmodAudio;

namespace BebooGarden.GameCore.Item;

public class Egg(string color) : Item
{
  protected override string _translateKeyName { get; } = "egg.name";
  protected override string _translateKeyDescription { get; } = "egg.description";
  public override Vector3? Position { get; set; } // position null=in inventory
  public override bool IsTakable { get; set; } = false;
  public override bool IsWaterProof { get; set; } = true;
  public override Channel? Channel { get; set; }
  private string Color { get; set; } = color;

  public override void Action() => Hatch();
  public override void Take() => Hatch();
  public override void BebooAction(Beboo beboo) => Hatch();
  private void Hatch()
  {
    Game.Map?.Items.Remove(this);
    SoundLoopTimer.Stop();
    Game.SoundSystem.PlayCinematic(Game.SoundSystem.CinematicHatch);
    var name = NewBeboo.Run();
    var swimLevel = (Game.Map?.IsInLake(Position ?? new(0, 0, 0)) ?? false) ? 10 : 0;
    Game.Map?.Beboos.Add(new Beboo(name, 1, DateTime.MinValue, 5, swimLevel, false) { Position = this.Position ?? new(0, 0, 0) });
    Game.Flags.NewGame = false;
    Game.UpdateMapMusic();
  }

  public override void PlaySound()
  {
    if (Position == null) return;
    Channel = Game.SoundSystem.PlaySoundAtPosition(Game.SoundSystem.EggKrakSounds, (Vector3)Position);
  }
}