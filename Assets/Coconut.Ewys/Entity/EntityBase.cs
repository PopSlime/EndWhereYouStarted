using Cryville.Common.Unity;
using System;
using UnityEngine;
using static Coconut.Ewys.LevelController;

namespace Coconut.Ewys.Entity {
	public class PositionUpdateEventArgs : EventArgs {
		public EntityBase Entity { get; set; }
		public Vector2Int From { get; set; }
		public Vector2Int To { get; set; }
		public PositionUpdateEventArgs(EntityBase entity, Vector2Int from, Vector2Int to) {
			Entity = entity;
			From = from;
			To = to;
		}
	}
	public abstract class EntityBase : MonoBehaviour {
		Side _side;
		Vector2Int m_position;
		public Vector2Int Position {
			get => m_position;
			set {
				PositionUpdate?.Invoke(this, new PositionUpdateEventArgs(this, m_position, value));
				m_position = value;
			}
		}
		public event EventHandler<PositionUpdateEventArgs> PositionUpdate;
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
		public bool TryMove(Vector2Int delta, FlagAtomicDelegate d, bool teleport = false) {
			if (_moveDest != null) return false;
			Vector2Int dest = Position + delta;
			if (IsBlocked(dest, teleport ? null : delta)) {
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
					transform.position = (Vector3Int)Position;
					_moveDest = null;
					_onMoveDone?.Invoke();
				}
				else {
					transform.position += delta.normalized * step;
				}
			}
		}
	}
}
