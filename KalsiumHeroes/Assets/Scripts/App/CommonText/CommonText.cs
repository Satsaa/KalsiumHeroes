
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class CommonText : MonoBehaviour {

	[Tooltip("The deliminator between a value and it's name: e.g. Health\": \"500")]
	[field: SerializeField] public TextSource valueDelim;

}