using Cryville.Common.Unity;
using System;
using System.Collections;
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

		protected int State { get; private set; }
		public void SetState(int flag, bool inverse) {
			if (inverse) State &= ~flag;
			else State |= flag;
			OnSetState();
		}
		protected virtual void OnSetState() { }

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

		protected bool IsOnActiveSide { get; private set; }
		public virtual bool IsActive => IsOnActiveSide;
		public virtual void OnPhaseUpdate(Side side, bool doTransition = true) {
			GetComponent<Renderer>().enabled = IsOnActiveSide = _side.HasFlag(side);
		}

		const float MOVE_SPEED = 2f;
		public bool TryMove(Vector2Int delta, FlagAtomDelegate d, bool teleport = false) {
			Vector2Int dest = Position + delta;
			if (IsBlocked(dest, teleport ? null : delta)) {
				d(); return true;
			}
			StartCoroutine(Move(dest, d));
			return true;
		}

		IEnumerator Move(Vector2Int dest, FlagAtomDelegate d) {
			while (true) {
				yield return new WaitForFixedUpdate();
				var delta = (Vector3Int)dest - transform.position;
				var mag = delta.magnitude;
				var step = MOVE_SPEED * Time.fixedDeltaTime;
				if (step >= mag) break;
				transform.position += delta.normalized * step;
			}
			Position = dest;
			transform.position = (Vector3Int)Position;
			d?.Invoke();
		}
	}
}
