using Coconut.Ewys.Entity;
using Newtonsoft.Json;
using System;
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

		public int Step => _level.steps;

		public static void LoadIndex() =>
			_index ??= JsonConvert.DeserializeObject<List<string>>(Resources.Load<TextAsset>("Levels/Index").text);
		public static int LevelCount => _index.Count;

		void Awake() {
			Instance = this;
			LoadIndex();
			Read(_index[CurrentLevel]);

			if (_env != null) {
				var camera = Camera.main;
				camera.transform.position = new Vector3(_env.Value.CenterX, _env.Value.CenterY, -10);
				var ew = _env.Value.Width + 2;
				var eh = _env.Value.Height + 2;
				camera.orthographicSize = Mathf.Max(ew * Screen.height / Screen.width, eh) / 2;
			}

			SceneManager.LoadScene("GamingUI", LoadSceneMode.Additive);
		}

		void Update() {
			if (_players.Count > 0) {
				if (Input.GetKeyDown(KeyCode.Tab)) { _currentPlayer++; _currentPlayer %= _players.Count; }
				if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) _ops.Add(new EntityMoveAtom(_players[_currentPlayer], Vector2Int.up));
				if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) _ops.Add(new EntityMoveAtom(_players[_currentPlayer], Vector2Int.down));
				if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) _ops.Add(new EntityMoveAtom(_players[_currentPlayer], Vector2Int.left));
				if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) _ops.Add(new EntityMoveAtom(_players[_currentPlayer], Vector2Int.right));
			}
#if UNITY_EDITOR
			if (Input.GetKeyDown(KeyCode.PageDown)) { CurrentLevel++; SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
			else if (Input.GetKeyDown(KeyCode.PageUp)) { CurrentLevel--; SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
#endif
			while (_bops.Count > 0 && !_bops[0].Working) _bops.RemoveAt(0);
			if (_bops.Count > 0) return;
			if (_lunarPhase) {
				var op = _ops[_currentOp + 1];
				if (!op.Working) {
					if (_currentOp <= 0) {
						switch (Judge()) {
							case 0:
								CurrentLevel++;
								SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
								break;
							case -1:
								PushBlockingAtom(new DummyAtom(false));
								PushBlockingAtom(new TintLightAtom(new Color(1, 0, 0), 0.25f, 0.5f));
								UIControl.Instance.SetExplainText("失败：没能回到营地\n点击右上角按钮重试");
								break;
							case -2:
								PushBlockingAtom(new DummyAtom(false));
								UIControl.Instance.SetExplainText("失败：没能获取所有宝藏\n点击右上角按钮重试");
								break;
							default:
								throw new NotImplementedException();
						}
					}
					else {
						_ops[_currentOp--].Undo();
						PushBlockingAtom(new UIControl.RotateClockAtom(_level.steps - _currentOp, _level.steps, true));
					}
				}
			}
			else {
				var op = _ops[_currentOp - 1];
				if (!op.Working) {
					if (_currentOp > _level.steps) {
						foreach (var entity in _entities.Values) entity.OnPhaseUpdate(Side.Lunar);
						foreach (var entity in _entities.Values) entity.Position = entity.Position;
						var atom = new TintLightAtom(new Color(0.9f, 0.9f, 1), 0.6f, 1); atom.Do();
						_ops.Insert(_currentOp--, atom);
						_lunarPhase = true;
					}
					else if (_currentOp < _ops.Count) {
						_ops[_currentOp++].Do();
						PushBlockingAtom(new UIControl.RotateClockAtom(_level.steps - _currentOp + 1, _level.steps));
					}
				}
			}
		}

		/// <summary>
		/// Judges.
		/// </summary>
		/// <returns>
		/// The result of the judgement.
		/// <list type="bullet">
		/// <item><term><c>0</c></term><description>Passed.</description></item>
		/// <item><term><c>-1</c></term><description>Executed.</description></item>
		/// <item><term><c>-2</c></term><description>Returned with insufficient treasure.</description></item>
		/// </list>
		/// </returns>
		int Judge() {
			foreach (var player in _players) {
				if (!player.IsHome) return -1;
			}
			foreach (var entity in _entities) {
				if (entity.Value is Treasure treasure) {
					if (!treasure.IsPickedUp) return -2;
				}
			}
			return 0;
		}

		void OnEntityPositionUpdate(object sender, PositionUpdateEventArgs e) {
			_tiles[e.From].Remove(e.Entity);
			_tiles[e.To].Add(e.Entity);
			if (e.Entity is Weight w) {
				if (_tiles.TryGetValue(e.From, out List<EntityBase> entities)) {
					foreach (var entity in entities) {
						if (!entity.IsActive) continue;
						if (entity is Trigger trigger) {
							_entities[trigger.TargetID].SetState(trigger.Flag, !trigger.IsInverse);
						}
					}
				}
				if (_tiles.TryGetValue(e.To, out entities)) {
					var p = w is Player ? w as Player : null;
					foreach (var entity in entities) {
						if (!entity.IsActive) continue;
						if (p != null) {
							if (entity is Treasure treasure) {
								treasure.TryPickUp();
							}
						}
						if (entity is Portal portal && portal.TargetID != null) {
							w.TryMove(_entities[portal.TargetID.Value].Position - e.Entity.Position, teleport: true);
						}
						if (entity is Trigger trigger) {
							_entities[trigger.TargetID].SetState(trigger.Flag, trigger.IsInverse);
						}
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

		void TintLight(Color color, float intensity, float duration, FlagAtomDelegate d)
			=> StartCoroutine(CoTintLight(color, intensity, duration, d));
		IEnumerator CoTintLight(Color color, float intensity, float duration, FlagAtomDelegate d) {
			float time = 0;
			var c0 = m_light.color;
			var i0 = m_light.intensity;
			while (true) {
				yield return new WaitForFixedUpdate();
				time += Time.fixedDeltaTime;
				time = Mathf.Min(duration, time);
				var mix = time / duration;
				m_light.color = Color.Lerp(c0, color, mix);
				m_light.intensity = Mathf.Lerp(i0, intensity, mix);
				if (time == duration) break;
			}
			d();
		}
		public class TintLightAtom : OperationAtom {
			readonly Color color;
			readonly float intensity;
			readonly float duration;

			public TintLightAtom(Color color, float intensity, float duration) {
				this.color = color;
				this.intensity = intensity;
				this.duration = duration;
			}

			protected override void DoImpl(FlagAtomDelegate d) => Instance.TintLight(color, intensity, duration, d);

			protected override void UndoImpl(FlagAtomDelegate d) => throw new NotSupportedException("Cannot undo this atom.");
		}
	}
}
