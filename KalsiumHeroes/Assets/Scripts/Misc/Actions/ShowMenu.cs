
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class ShowMenu : MonoBehaviour {

	public Menu prefab;

	public void DoShowMenu() {
		Menus.Show(prefab);
	}
}