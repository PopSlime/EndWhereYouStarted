using Coconut.Ewys.Entity;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace Coconut.Ewys {
	public class LevelController : MonoBehaviour {
		public static LevelController Instance;

		static List<string> _index;
		public static int CurrentLevel;

		List<Vector2Int> _tiles = new();
		Dictionary<Vector2Int, EntityBase> _entities = new();

		void Awake() {
			Instance = this;
			_index ??= JsonConvert.DeserializeObject<List<string>>(Resources.Load<TextAsset>("Levels/Index").text);
			new LevelIO(_index[CurrentLevel], transform).Read(_tiles, _entities);
			(_entities[Vector2Int.zero] as Player).TryMove(Vector2Int.down, () => { });
		}

		public static bool IsBlocked(Vector2Int pos) => Instance.IsBlockedImpl(pos);
		bool IsBlockedImpl(Vector2Int pos) {
			if (!_tiles.Contains(pos)) return true;
			if (_entities.TryGetValue(pos, out EntityBase entity)) {
				if (entity is Obstacle) return true;
				// TODO Movable objects
			}
			return false;
		}
	}
}
