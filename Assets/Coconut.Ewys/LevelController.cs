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
		bool _lunarPhase;

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
			if (_lunarPhase) {
				var op = _ops[_currentOp + 1];
				if (!op.Working) {
					if (_currentOp <= 0) {
						// TODO dest judge
					}
					else {
						_ops[_currentOp--].Undo();
					}
				}
			}
			else {
				var op = _ops[_currentOp - 1];
				if (!op.Working) {
					if (_currentOp > _level.steps) {
						var atom = new LunarPhaseAtomic();
						atom.Do();
						_ops.Insert(_currentOp--, atom);
						_lunarPhase = true;
					}
					else if (_currentOp < _ops.Count) {
						_ops[_currentOp++].Do();
					}
				}
			}
		}

		void OnEntityPositionUpdate(object sender, PositionUpdateEventArgs e) {
			_tiles[e.From].Remove(e.Entity);
			_tiles[e.To].Add(e.Entity);
		}

		public static bool IsBlocked(Vector2Int pos, Vector2Int? delta = null) => Instance.IsBlockedImpl(pos, delta);
		bool IsBlockedImpl(Vector2Int pos, Vector2Int? delta = null) {
			if (!_tiles.ContainsKey(pos)) return true;
			if (_tiles.TryGetValue(pos, out var entities)) {
				foreach (var entity in entities) {
					if (entity is Obstacle) return true;
					if (entity is Weight) return delta == null || !entity.TryMove(delta.Value, null, false);
				}
			}
			return false;
		}
	}
}
