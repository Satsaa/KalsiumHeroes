
using System;

interface IEventHandler {
  bool EventIsFinished();
  void SkipEvent();
}

interface IEventHandler<T> : IEventHandler where T : GameEvent {
  void StartEvent(T data);
}