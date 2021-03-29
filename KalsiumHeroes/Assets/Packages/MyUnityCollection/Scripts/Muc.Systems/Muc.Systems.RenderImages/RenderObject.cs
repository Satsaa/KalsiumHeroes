

namespace Muc.Systems.RenderImages {

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

		[SerializeField] protected UpdateMode updateMode = UpdateMode.Initialize;
		[SerializeField] protected Transform renderRoot;
		[HideInInspector] public bool rendered = false;

		private Camera _camera;
		new public Camera camera {
			get {
				if (_camera == null) _camera = GetComponent<Camera>();
				return _camera;
			}
		}

		protected void Awake() {
			camera.enabled = false;
			Debug.Assert(renderRoot, this);
		}

		protected void OnDestroy() {
			Destroy(gameObject);
		}

		protected void LateUpdate() {
			switch (updateMode) {
				case UpdateMode.Initialize:
					if (!rendered) {
						rendered = true;
						camera.Render();
					}
					break;
				case UpdateMode.Always:
					camera.Render();
					break;
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