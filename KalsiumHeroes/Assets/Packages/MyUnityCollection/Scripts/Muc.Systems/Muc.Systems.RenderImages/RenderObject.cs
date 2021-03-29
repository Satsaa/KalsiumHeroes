

namespace Muc.Systems.RenderImages {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using Muc.Extensions;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.UI;

#if (MUC_HIDE_COMPONENTS || MUC_HIDE_SYSTEM_COMPONENTS)
	[AddComponentMenu("")]
#else
	[AddComponentMenu("MyUnityCollection/" + nameof(Muc.Systems.RenderImages) + "/" + nameof(RenderObject))]
#endif
	[RequireComponent(typeof(Camera))]
	public class RenderObject : MonoBehaviour {

		[SerializeField] protected Transform renderRoot;

		[SerializeField] private UpdateMode _updateMode = UpdateMode.Initialize;
		protected UpdateMode updateMode {
			get => _updateMode;
			set {
				if (_updateMode == value) return;
				_updateMode = value;
				camera.enabled = _updateMode == UpdateMode.Always;
			}
		}

		private Camera _camera;
		new public Camera camera {
			get {
				if (_camera == null) _camera = GetComponent<Camera>();
				return _camera;
			}
		}

		protected void Awake() {
			camera.enabled = updateMode == UpdateMode.Always;
			Debug.Assert(renderRoot, this);
		}

		protected void Start() {
			if (updateMode == UpdateMode.Initialize) {
				camera.Render();
			}
		}

		protected void OnDestroy() {
			Destroy(gameObject);
		}

		public void OnTextureChange() {
			if (updateMode == UpdateMode.Initialize) {
				camera.Render();
			}
		}

		protected enum UpdateMode {
			Always,
			Initialize,
			[Tooltip("While never means never it does not mean you cannot manually call Render().")]
			Never,
		}

	}

}