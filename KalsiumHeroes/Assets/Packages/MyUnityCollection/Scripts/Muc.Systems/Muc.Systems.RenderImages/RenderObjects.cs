

namespace Muc.Systems.RenderImages {

	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using Muc.Components.Extended;
	using Muc.Extensions;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.UI;

#if (MUC_HIDE_COMPONENTS || MUC_HIDE_SYSTEM_COMPONENTS)
	[AddComponentMenu("")]
#else
	[AddComponentMenu("MyUnityCollection/" + nameof(Muc.Systems.RenderImages) + "/" + nameof(RenderObjects))]
#endif
	public class RenderObjects : Singleton<RenderObjects> {

		[SerializeField] float distance = 200;

		[SerializeField, HideInInspector] int steps = 1;
		[SerializeField, HideInInspector] int dir; // 0 - 3
		[SerializeField, HideInInspector] int dirI;
		[SerializeField, HideInInspector] Vector2Int pos;

		public void AddObject(RenderObject renderObject) {
			var scaledPos = pos.Mul(distance);
			renderObject.transform.localPosition = scaledPos.x0y();

			AdvancePos();

		}

		void AdvancePos() {

			switch (dir) {
				case 0:
					pos += new Vector2Int(1, 0);
					break;
				case 1:
					pos += new Vector2Int(0, 1);
					break;
				case 2:
					pos += new Vector2Int(-1, 0);
					break;
				case 3:
					pos += new Vector2Int(0, -1);
					break;
			}
			dirI++;
			if (dirI >= steps) {
				dir++;
				dirI = 0;
				if (dir >= 4) dir = 0;
				if (dir % 2 == 0) {
					steps++;
				}
			}
		}


	}

}
