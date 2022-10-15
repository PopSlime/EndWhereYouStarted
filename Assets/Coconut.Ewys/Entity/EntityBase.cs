using Cryville.Common.Unity;
using System;
using UnityEngine;
using static Coconut.Ewys.LevelController;

namespace Coconut.Ewys.Entity {
	public abstract class EntityBase : MonoBehaviour {
		Side _side;
		Vector2Int m_position;
		public Vector2Int Position {
			get => m_position;
			set {
				m_position = value;
				PositionUpdate?.Invoke(this, this);
			}
		}
		public event EventHandler<EntityBase> PositionUpdate;
		public void FromData(EntityData data) {
			transform.position = (Vector3Int)data.pos.ToVector2Int();
			Position = data.pos.ToVector2Int();
			_side = data.side;
		}
		protected abstract void FromDataImpl(EntityData data);
		public EntityData ToData() {
			var result = ToDataImpl();
			result.pos = Vector2Int.RoundToInt(transform.position).ToArray();
			result.id = transform.GetSiblingIndex();
			return result;
		}
		public abstract EntityData ToDataImpl();

		const float MOVE_SPEED = 2f;
		FlagAtomicDelegate _onMoveDone;
		Vector2Int? _moveDest;
		public bool TryMove(Vector2Int delta, FlagAtomicDelegate d) {
			Vector2Int dest = Position + delta;
			if (IsBlocked(dest)) {
				d(); return true;
			}
			_onMoveDone = d;
			_moveDest = dest;
			return true;
		}

		void Update() {
			if (_moveDest != null) {
				var delta = (Vector3Int)_moveDest.Value - transform.position;
				var mag = delta.magnitude;
				var step = MOVE_SPEED * Time.deltaTime;
				if (step >= mag) {
					Position = _moveDest.Value;
					_moveDest = null;
					_onMoveDone();
				}
				else {
					transform.position += delta.normalized * step;
				}
			}
		}
	}
}
