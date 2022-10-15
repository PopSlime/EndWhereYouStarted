using Coconut.Ewys.Entity;
using UnityEngine;

namespace Coconut.Ewys {
	public delegate void FlagAtomicDelegate();
	public abstract class AtomicOperation {
		public bool Working { get; private set; }
		public void Do() {
			if (Working) throw new System.InvalidOperationException("Task working");
			if (DoImpl(() => Working = false)) Working = true;
		}
		public void Undo() {
			if (Working) throw new System.InvalidOperationException("Task working");
			if (UndoImpl(() => Working = false)) Working = true;
		}
		protected abstract bool DoImpl(FlagAtomicDelegate d);
		protected abstract bool UndoImpl(FlagAtomicDelegate d);
	}
	public abstract class PlayerAtomic : AtomicOperation {
		protected Player Player { get; private set; }
		protected PlayerAtomic(Player player) {
			Player = player;
		}
	}
	public class PlayerMoveAtomic : PlayerAtomic {
		Vector2Int _delta;
		public PlayerMoveAtomic(Player player, Vector2Int delta) : base(player) {
			_delta = delta;
		}

		protected override bool DoImpl(FlagAtomicDelegate d) {
			return Player.TryMove(_delta, d);
		}

		protected override bool UndoImpl(FlagAtomicDelegate d) {
			return Player.TryMove(-_delta, d);
		}
	}
}
