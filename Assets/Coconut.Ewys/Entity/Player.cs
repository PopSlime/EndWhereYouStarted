using UnityEngine;

namespace Coconut.Ewys.Entity {
	public class Player : Weight { }
	public abstract class PlayerAtom : OperationAtom {
		protected Player Player { get; private set; }
		protected PlayerAtom(Player player) {
			Player = player;
		}
	}
	public class PlayerMoveAtom : PlayerAtom {
		Vector2Int _delta;
		public PlayerMoveAtom(Player player, Vector2Int delta) : base(player) {
			_delta = delta;
		}

		protected override bool DoImpl(FlagAtomDelegate d) {
			return Player.TryMove(_delta, d);
		}

		protected override bool UndoImpl(FlagAtomDelegate d) {
			return Player.TryMove(-_delta, d);
		}
	}
}
