using Coconut.Ewys.Entity;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Coconut.Ewys {
	public partial class LevelController : MonoBehaviour {
		public static LevelController Instance;

		static List<string> _index;
		public static int CurrentLevel;

		[SerializeField]
		Light m_light;

		int _currentPlayer = 0;
		readonly List<OperationAtom> _ops = new() { new DummyAtom() };
		readonly List<OperationAtom> _bops = new();
		int _currentOp = 1;
		bool _lunarPhase;

		void Awake() {
			Instance = this;
			_index ??= JsonConvert.DeserializeObject<List<string>>(Resources.Load<TextAsset>("Levels/Index").text);
			Read(_index[CurrentLevel]);
		}

		void Update() {
			if (Input.GetKeyDown(KeyCode.Tab)) { _currentPlayer++; _currentPlayer %= _players.Count; }
			if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow   )) _ops.Add(new EntityMoveAtom(_players[_currentPlayer], Vector2Int.up   ));
			if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow )) _ops.Add(new EntityMoveAtom(_players[_currentPlayer], Vector2Int.down ));
			if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow )) _ops.Add(new EntityMoveAtom(_players[_currentPlayer], Vector2Int.left ));
			if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) _ops.Add(new EntityMoveAtom(_players[_currentPlayer], Vector2Int.right));
			while (_bops.Count > 0 && !_bops[0].Working) _bops.RemoveAt(0);
			if (_bops.Count > 0) return;
			if (_lunarPhase) {
				var op = _ops[_currentOp + 1];
				if (!op.Working) {
					if (_currentOp <= 0) {
						CurrentLevel++;
						SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
						foreach (var entity in _entities) entity.OnPhaseUpdate(Side.Lunar);
						var atom = new LunarPhaseAtom(); atom.Do();
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
			if (e.Entity is Weight w) {
				if (_tiles.TryGetValue(e.From, out List<EntityBase> entities)) {
					foreach (var entity in entities) {
						if (!entity.IsActive) continue;
						// TODO step off trigger
					}
				}
				if (_tiles.TryGetValue(e.To, out entities)) {
					var p = w is Player ? w as Player : null;
					foreach (var entity in entities) {
						if (!entity.IsActive) continue;
						if (p != null) {
							if (entity is Treasure treasure) {
								treasure.PickUp();
							}
						}
						// TODO teleport
						// TODO step on trigger
					}
				}
			}
		}

		public static void PushBlockingAtom(OperationAtom atom) => Instance.PushBlockingAtomImpl(atom);
		public void PushBlockingAtomImpl(OperationAtom atom) {
			atom.Do();
			_bops.Add(atom);
		}

		public static bool IsBlocked(Vector2Int pos, Vector2Int? delta = null) => Instance.IsBlockedImpl(pos, delta);
		bool IsBlockedImpl(Vector2Int pos, Vector2Int? delta = null) {
			if (!_tiles.ContainsKey(pos)) return true;
			if (_tiles.TryGetValue(pos, out var entities)) {
				foreach (var entity in entities) {
					if (!entity.IsActive) continue;
					if (entity is Obstacle) return true;
					if (entity is Weight) return delta == null || !entity.TryMove(delta.Value, teleport: false);
				}
			}
			return false;
		}

		void ToLunarPhase(FlagAtomDelegate d) => StartCoroutine(CoToLunarPhase(d));
		IEnumerator CoToLunarPhase(FlagAtomDelegate d) {
			float time = 0;
			while (true) {
				yield return new WaitForFixedUpdate();
				time += Time.fixedDeltaTime;
				time = Mathf.Min(1, time);
				m_light.color = new Color(
					-0.1f * time + 1,
					-0.1f * time + 1,
					0.1f * time + 0.9f
				);
				m_light.intensity = -0.4f * time + 1;
				if (time == 1) break;
			}
			d();
		}
		public class LunarPhaseAtom : OperationAtom {
			protected override void DoImpl(FlagAtomDelegate d) => Instance.ToLunarPhase(d);

			protected override void UndoImpl(FlagAtomDelegate d) => throw new System.NotSupportedException("Cannot undo this atom.");
		}
	}
}
