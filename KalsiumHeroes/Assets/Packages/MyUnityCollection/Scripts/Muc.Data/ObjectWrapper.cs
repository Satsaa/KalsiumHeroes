
namespace Muc.Data {

	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using System.Linq;
	using Object = UnityEngine.Object;

	[Serializable]
	public class ObjectWrapper<T> where T : class {

		[SerializeField] Object objectValue;

		[SerializeField] T _value;
		public T value {
			get {
				if (!Object.ReferenceEquals(_value, objectValue)) {
					_value = objectValue as T;
				}
				return _value;
			}
			set {
				if (_value == value) return;
				objectValue = value as Object;
				_value = value;
			}
		}

		public void UpdateValue() {
			_value = objectValue as T;
		}
	}

}


#if UNITY_EDITOR
namespace Muc.Data {

	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using static Muc.Editor.PropertyUtil;
	using static Muc.Editor.EditorUtil;
	using Object = UnityEngine.Object;

	[CanEditMultipleObjects]
	[CustomPropertyDrawer(typeof(ObjectWrapper<>), true)]
	internal class ObjectWrapperDrawer : PropertyDrawer {

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

			var objectValue = property.FindPropertyRelative("objectValue");

			using (PropertyScope(position, label, property, out label)) {

				var current = GetFirstValue<object>(property).GetType();
				while (current != typeof(object)) {
					if (current.GetGenericTypeDefinition() == typeof(ObjectWrapper<>)) {
						var type = current.GetGenericArguments().First();
						EditorGUI.ObjectField(position, objectValue, type, label);
						break;
					}
					current = current.BaseType;
				}

			}
		}


		private static void OnSelect(SerializedProperty property, Type type) {
			var values = GetValues<SerializedType>(property);
			Undo.RecordObjects(property.serializedObject.targetObjects, $"Set {property.name}");
			foreach (var value in values) value.type = type;
			foreach (var target in property.serializedObject.targetObjects) EditorUtility.SetDirty(target);
			property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
		}

	}
}
#endif
