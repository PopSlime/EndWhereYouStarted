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
				Vector2Int from = m_position;
				m_position = value;
				PositionUpdate?.Invoke(this, new PositionUpdateEventArgs(this, from, value));
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
			FromDataImpl(data);
		}
		protected abstract void FromDataImpl(EntityData data);
		public EntityData ToData() {
			var result = ToDataImpl();
			result.pos = Vector2Int.RoundToInt(transform.position).ToArray();
			result.id = transform.GetSiblingIndex();
			return result;
		}
		public abstract EntityData ToDataImpl();

		Renderer _renderer;
		protected bool IsOnActiveSide { get; private set; }
		public virtual bool IsActive => IsOnActiveSide;
		public virtual bool IsVisible => true;
		public virtual void OnPhaseUpdate(Side side, bool doTransition = true) {
			IsOnActiveSide = _side.HasFlag(side);
			UpdateState();
		}
		protected void UpdateState() {
			if (_renderer == null) _renderer = GetComponent<Renderer>();
			_renderer.material.color = new Color(
				_side.HasFlag(Side.Solar) ? 1 : 0.8f,
				_side.HasFlag(Side.Solar) ? 1 : 0.8f,
				_side.HasFlag(Side.Lunar) ? 1 : 0.8f,
				IsActive ? 1 : 0.5f
			);
			_renderer.enabled = IsVisible;
		}

		const float MOVE_SPEED = 2f;
		int _teleportCount;
		public bool TryMove(Vector2Int delta, FlagAtomDelegate d = null, bool teleport = false) {
			if (d == null) {
				var atom = new EntityMoveAtom(this, delta, teleport);
				PushBlockingAtom(atom);
				return atom.Working;
			}
			Vector2Int dest = Position + delta;
			if (IsBlocked(dest, teleport ? null : delta)) {
				d(); return false;
			}
			if (teleport) {
				if (_teleportCount > 0) {
					d();
					return false;
				}
				else {
					_teleportCount++;
					StartCoroutine(CoTeleport(dest, d));
				}
			}
			else {
				if (_teleportCount > 0) _teleportCount--;
				StartCoroutine(CoMove(dest, d));
			}
			return true;
		}

		IEnumerator CoTeleport(Vector2Int dest, FlagAtomDelegate d) {
			yield return null;
			Position = dest;
			transform.position = (Vector3Int)Position;
			d();
		}

		IEnumerator CoMove(Vector2Int dest, FlagAtomDelegate d) {
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
			d();
		}
	}
	public class EntityMoveAtom : OperationAtom {
		readonly EntityBase _entity;
		readonly Vector2Int _delta;
		readonly bool _teleport;
		public EntityMoveAtom(EntityBase entity, Vector2Int delta, bool teleport = false) {
			_entity = entity;
			_delta = delta;
			_teleport = teleport;
		}

		protected override void DoImpl(FlagAtomDelegate d) => _entity.TryMove(_delta, d, _teleport);

		protected override void UndoImpl(FlagAtomDelegate d) => _entity.TryMove(-_delta, d, _teleport);
	}
}
