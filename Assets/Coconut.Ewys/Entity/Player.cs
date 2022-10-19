using UnityEngine;

namespace Coconut.Ewys.Entity {
	public class Player : Weight {
		protected override void OnStartMove(Vector2Int delta, bool teleport) {
			base.OnStartMove(delta, teleport);
			if (!teleport && delta.x != 0) Renderer.flipX = delta.x > 0;
		}
	}
}