using UnityEngine;

namespace Coconut.Ewys.Entity {
	public class Player : Weight {
		Vector2Int _home;
		public bool IsHome => Position == _home;

		protected override void FromDataImpl(EntityData data) {
			base.FromDataImpl(data);
			_home = Position;
		}

		protected override void OnStartMove(Vector2Int delta, bool teleport) {
			base.OnStartMove(delta, teleport);
			if (!teleport && delta.x != 0) Renderer.flipX = delta.x > 0;
		}
	}
}
