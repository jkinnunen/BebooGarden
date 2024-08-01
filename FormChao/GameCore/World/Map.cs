﻿using System.Numerics;
using BebooGarden.GameCore.Item;
using FmodAudio;
using Newtonsoft.Json;

namespace BebooGarden.GameCore.World;

public class Map
{
  public string Name { get; }
  private int SizeX { get; set; }
  private int SizeY { get; set; }
  public List<TreeLine> TreeLines { get; }
  public Vector3 WaterPoint { get; }
  public List<Item.Item> Items { get; init; } = new();
  public bool IsLullabyPlaying { get; set; } = false;
  public bool IsDansePlaying { get; set; } = false;
  [JsonIgnore]
  public List<Channel> WaterChannels { get; set; } = new();
  [JsonIgnore]
  public List<Channel> TreesChannels { get; set; } = new();
  [JsonIgnore]
  public Channel? BackgroundChannel { get; set; }

  public Map(string name, int sizeX, int sizeY, List<TreeLine> treeLines, Vector3 waterPoint)
  {
    Name = name;
    SizeX = sizeX;
    SizeY = sizeY;
    TreeLines = treeLines;
    WaterPoint = waterPoint;
    TimedBehaviour<Map> ticketPopBehaviour = new(this, 30000 * 60, 60000 * 60, (map) => map.PopTicketPack(), true);
  }

  private void PopTicketPack()
  {
    if (!Items.OfType<TicketPack>().Any())
    {
      Vector3 randPos;
      do
      {
        randPos = new Vector3(Game.Random.Next(-SizeX / 2, SizeX / 2), Game.Random.Next(-SizeY / 2, SizeY / 2), 0);
      } while (IsInLake(randPos) || GetTreeLineAtPosition(randPos) != null);
      AddItem(new TicketPack(Game.Random.Next(4)), randPos);
    }
  }

  public Vector3 Clamp(Vector3 value)
  {
    var x = Math.Clamp(value.X, SizeX / 2 * -1, SizeX / 2);
    var y = Math.Clamp(value.Y, SizeY / 2 * -1, SizeY / 2);
    var z = value.Z;
    var newPos = new Vector3(x, y, z);
    return newPos;
  }
  public bool IsInLake(Vector3 position) => Util.IsInSquare(position, WaterPoint, 5);
  public TreeLine? GetTreeLineAtPosition(Vector3 position)
  {
    return TreeLines.FirstOrDefault(
        treeLine => treeLine != null && treeLine.IsOnLine(position),
        null);
  }

  public bool AddItem(Item.Item item, Vector3 position)
  {
    if (GetTreeLineAtPosition(position) != null) return false;
    Items.Add(item);
    item.Position = position;
    return true;
  }


  public Item.Item? GetItemArroundPosition(Vector3 position)
  {
    if (Items == null || Items.Count == 0) return null;
    return Items.FirstOrDefault(item => item != null && item.Position != null && Util.IsInSquare(item.Position.Value, position, 1),
            null);
  }
  public bool IsArrundShop(Vector3 position)
  {
    return Util.IsInSquare(new Vector3(SizeX / 2, -SizeY / 2, 0), position, 1);
  }
}