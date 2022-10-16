using Coconut.Ewys.Entity;
using System;
using UnityEngine;

namespace Coconut.Ewys {
	public delegate void FlagAtomicDelegate();
	public abstract class AtomicOperation {
		public bool Working { get; private set; }
		public void Do() {
			if (Working) throw new InvalidOperationException("Task working");
			Working = true;
			if (!DoImpl(() => Working = false)) Working = false;
		}
		public void Undo() {
			if (Working) throw new InvalidOperationException("Task working");
			Working = true;
			if (!UndoImpl(() => Working = false)) Working = false;
		}
		protected abstract bool DoImpl(FlagAtomicDelegate d);
		protected abstract bool UndoImpl(FlagAtomicDelegate d);
	}
	public class DummyAtomic : AtomicOperation {
		protected override bool DoImpl(FlagAtomicDelegate d) { return true; }

		protected override bool UndoImpl(FlagAtomicDelegate d) { return true; }
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
	public class LunarPhaseAtomic : AtomicOperation {
		protected override bool DoImpl(FlagAtomicDelegate d) {
			d();
			return true;
		}

		protected override bool UndoImpl(FlagAtomicDelegate d) {
			d();
			return true;
		}
	}
}
