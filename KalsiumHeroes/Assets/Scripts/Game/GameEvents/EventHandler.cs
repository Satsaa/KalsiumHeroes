
using System;

[Serializable]
public abstract class EventHandler {
	/// <summary> Whether or not the event has ended. </summary>
	public abstract bool EventHasEnded();
	/// <summary> Request ending of the event. Returns true if the event was ended. </summary>
	public abstract bool End();
	/// <summary> Per frame update. </summary>
	public abstract void Update();
}

[Serializable]
public abstract class EventHandler<T> : EventHandler where T : Event {

	public T data;

	public EventHandler(T data) {
		this.data = data;
	}
}