using System;

namespace Coconut.Ewys {
	public delegate void FlagAtomDelegate();
	public abstract class OperationAtom {
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
		protected abstract bool DoImpl(FlagAtomDelegate d);
		protected abstract bool UndoImpl(FlagAtomDelegate d);
	}
	public class DummyAtom : OperationAtom {
		protected override bool DoImpl(FlagAtomDelegate d) { d(); return true; }

		protected override bool UndoImpl(FlagAtomDelegate d) { d(); return true; }
	}
}
