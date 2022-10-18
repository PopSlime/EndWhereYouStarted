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
}
