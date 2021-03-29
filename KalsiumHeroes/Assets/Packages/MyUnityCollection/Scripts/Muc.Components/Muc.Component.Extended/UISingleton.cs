

namespace Muc.Components.Extended {

	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using Object = UnityEngine.Object;

	public abstract class UISingleton<T> : ExtendedUIBehaviour where T : ExtendedUIBehaviour {

		public static T instance => _instance;
		private static T _instance;

		new public static RectTransform rectTransform => instance.rectTransform;

		new protected void OnValidate() {
			if (_instance != null && _instance != this) {
				Debug.LogWarning($"Multiple {typeof(T).Name} GameObjects!");
			} else {
				_instance = this as T;
			}
			base.OnValidate();
		}

		new protected void Awake() {
			if (_instance != null && _instance != this) {
				Debug.LogWarning($"Multiple {typeof(T).Name} GameObjects!");
			} else {
				_instance = this as T;
			}
			base.Awake();
		}

		new protected void OnDestroy() {
			if (_instance == this) {
				_instance = null;
			}
			base.OnDestroy();
		}

	}

}