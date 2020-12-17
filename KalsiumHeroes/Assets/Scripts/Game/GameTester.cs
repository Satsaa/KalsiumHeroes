

using System;
using System.Linq;

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


[ExecuteAlways, RequireComponent(typeof(GridTester))]
public class GameTester : MonoBehaviour {

	public Game game;
	public GridTester tester;

	[HideInInspector] public int index;

	[Tooltip("The main selection coordinate. Use copy and paste for quick and easy values for data.")]
	public Vector3Int currentPos;

	[SerializeReference, Tooltip("Data sent with PostEvent")]
	public object data;

	void OnValidate() {
		if (game == null) game = GetComponent<Game>();
		if (tester == null) tester = GetComponent<GridTester>();
	}

}


#if UNITY_EDITOR

[CustomEditor(typeof(GameTester))]
public class GameTesterEditor : Editor {

	GameTester t => (GameTester)target;

	public override bool RequiresConstantRepaint() => true;

	public int index => t.index;
	Type[] types = Events.events.Values.ToArray();
	string[] names = Events.events.Values.Select(t => $"{t.Name}").ToArray();


	public override void OnInspectorGUI() {
		serializedObject.Update();
		DrawDefaultInspector();

		t.index = EditorGUILayout.Popup("EventType", index, names);
		var eventName = names[index];
		var dataType = types[index];

		if (t.data == null || t.data.GetType() != dataType) {
			var val = (GameEvent)Activator.CreateInstance(dataType);
			val.eventNum = -1;
			t.data = val;
		}

		if (GUILayout.Button($"Post Event ({eventName})")) {
			GameEvent e = (GameEvent)t.data;
			if (e.eventNum == -1) e.eventNum = Game.client.eventNum++;
			Game.client.PostEvent(e);
		}

		t.currentPos = t.tester.main.hex.pos;
		serializedObject.ApplyModifiedProperties();

	}

}
#endif