
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class GameClient {

  public int eventNum = 0;
  public GameEvents ge = new GameEvents();

  public void PostEvent(GameEvent e) {
    e.eventNum = eventNum++;

    var packet = new GameEventPacket(e.GetType().Name, JsonUtility.ToJson(e));
    var fullJson = JsonUtility.ToJson(packet);

    // Send to server

    ReceiveEvent(fullJson);
  }

  private void ReceiveEvent(string json) {
    var packet = JsonUtility.FromJson<GameEventPacket>(json);
    ge.InvokeEvent(packet);
  }
}

public class GameEventPacket {
  public GameEventPacket(string name, string json) {
    this.name = name;
    this.json = json;
  }
  public string name;
  public string json;
}
