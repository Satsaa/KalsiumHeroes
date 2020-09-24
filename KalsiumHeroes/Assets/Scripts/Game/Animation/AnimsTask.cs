
using System;

interface IAnimsProvider {
  bool IsFinished();
}

/// <summary>
/// <para> Implement this inteface on anything that can provide animations for the Anims system. </para>
/// <para> If the type provides multiple animations, use properties as IAnimsProviders. </para>
/// </summary>
interface IAnimsProvider<T> : IAnimsProvider where T : GameEvent {
  void StartAnim(T data);
}