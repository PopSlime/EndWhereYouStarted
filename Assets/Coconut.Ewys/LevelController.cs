using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace Coconut.Ewys {
	public class LevelController : MonoBehaviour {
		public static LevelController Instance;

		static List<string> _index;
		public static int CurrentLevel;

		void Awake() {
			Instance = this;
			if (_index == null) _index = JsonConvert.DeserializeObject<List<string>>(Resources.Load<TextAsset>("Levels/Index").text);
			new LevelIO(_index[CurrentLevel], transform).Read();
		}
	}
}
