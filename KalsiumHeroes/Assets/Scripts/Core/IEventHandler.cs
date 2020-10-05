
using System;

interface IEventHandler {
  /// <summary> Whether or not the event has finished. </summary>
  bool EventIsFinished();
  /// <summary> Request skipping of the event. Returns true if the event was skipped. </summary>
  bool SkipEvent();
}

interface IEventHandler<T> : IEventHandler where T : GameEvent {
  /// <summary> Starts the animation of the event. </summary>
  void StartEvent(T data);
}