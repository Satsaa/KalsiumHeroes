

using System;
using System.Linq;

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


[ExecuteAlways, RequireComponent(typeof(Client))]
public class ClientTester : MonoBehaviour {

	public Client client;

	[HideInInspector] public int index;

	[Tooltip("The main selection coordinate. Use copy and paste for quick and easy values for data.")]
	public Vector3Int currentPos;

	[SerializeReference, Tooltip("Data sent with PostEvent")]
	public object data;

	void OnValidate() {
		if (client == null) Debug.Assert((client = GetComponent<Client>()) != null, this);
	}

}


#if UNITY_EDITOR

[CustomEditor(typeof(ClientTester))]
public class ClientTesterEditor : Editor {

	ClientTester t => (ClientTester)target;

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
			var val = (GameEvent)Activator.CreateInstance(dataType, true);
			val.gameEventNum = -1;
			t.data = val;
		}

		if (GUILayout.Button($"Post Event ({eventName})")) {
			var e = (GameEvent)t.data;
			if (e.gameEventNum == -1) e.gameEventNum = Game.instance.gameEventNum++;
			t.client.PostEvent(e);
		}

		serializedObject.ApplyModifiedProperties();

	}

}
#endif