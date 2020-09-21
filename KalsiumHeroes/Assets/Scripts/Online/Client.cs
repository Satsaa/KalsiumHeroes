
using UnityEngine;
using UnityEngine.Events;

public class Client {


  public void PostEvent<T>(GameEvent<T> e) {
    var json = JsonUtility.ToJson(e);

    // TODO: Send to server

    ReceiveEvent(json);
  }

  private void ReceiveEvent(string json) {
    // Json parse

    // Use 


  }
}