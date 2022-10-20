using UnityEngine;

namespace Coconut.Ewys.Entity {
	public class Player : Weight {
		[SerializeField]
		GameObject m_prefabCamp;

		Vector2Int _home;
		public bool IsHome => Position == _home;

		protected override void FromDataImpl(EntityData data) {
			base.FromDataImpl(data);
			_home = Position;

			var camp = GameObject.Instantiate(m_prefabCamp);
			camp.transform.SetParent(LevelController.Instance.transform);
			camp.transform.position = (Vector3Int)_home;
		}

		protected override void OnStartMove(Vector2Int delta, bool teleport) {
			base.OnStartMove(delta, teleport);
			if (!teleport && delta.x != 0) Renderer.flipX = delta.x > 0;
		}
	}
}
