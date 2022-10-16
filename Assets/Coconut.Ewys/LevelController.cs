using Coconut.Ewys.Entity;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace Coconut.Ewys {
	public partial class LevelController : MonoBehaviour {
		public static LevelController Instance;

		static List<string> _index;
		public static int CurrentLevel;

		int _currentPlayer = 0;
		List<AtomicOperation> _ops = new() { new DummyAtomic() };
		int _currentOp = 1;

		void Awake() {
			Instance = this;
			_index ??= JsonConvert.DeserializeObject<List<string>>(Resources.Load<TextAsset>("Levels/Index").text);
			Read(_index[CurrentLevel]);
		}

		void Update() {
			if (Input.GetKeyDown(KeyCode.Tab)) { _currentPlayer++; _currentPlayer %= _players.Count; }
			if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow   )) _ops.Add(new PlayerMoveAtomic(_players[_currentPlayer], Vector2Int.up   ));
			if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow )) _ops.Add(new PlayerMoveAtomic(_players[_currentPlayer], Vector2Int.down ));
			if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow )) _ops.Add(new PlayerMoveAtomic(_players[_currentPlayer], Vector2Int.left ));
			if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) _ops.Add(new PlayerMoveAtomic(_players[_currentPlayer], Vector2Int.right));
			if (_currentOp < _ops.Count) {
				var op = _ops[_currentOp - 1];
				if (!op.Working) {
					_ops[_currentOp].Do();
					_currentOp++;
				}
			}
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
