using System;

namespace Coconut.Ewys {
	public delegate void FlagAtomDelegate();
	public abstract class OperationAtom {
		public bool Working { get; private set; }
		public void Do() {
			if (Working) throw new InvalidOperationException("Task working");
			Working = true;
			DoImpl(() => Working = false);
		}
		public void Undo() {
			if (Working) throw new InvalidOperationException("Task working");
			Working = true;
			UndoImpl(() => Working = false);
		}
		protected abstract void DoImpl(FlagAtomDelegate d);
		protected abstract void UndoImpl(FlagAtomDelegate d);
	}
	public class DummyAtom : OperationAtom {
		bool _done;

		public DummyAtom() : this(true) { }

		public DummyAtom(bool done) {
			_done = done;
		}

		protected override void DoImpl(FlagAtomDelegate d) { if (_done) d(); }

		protected override void UndoImpl(FlagAtomDelegate d) { if (_done) d(); }
	}
}
