using UnityEngine;

namespace Coconut.Ewys.Entity {
	public class Player : Weight { }
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
