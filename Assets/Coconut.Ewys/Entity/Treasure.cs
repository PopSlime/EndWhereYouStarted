using UnityEngine;

namespace Coconut.Ewys.Entity {
	public class Treasure : EntityBase {
		[SerializeField]
		Sprite m_spriteChestClosed;

		[SerializeField]
		Sprite m_spriteChestOpened;

		[SerializeField]
		Sprite m_spriteChestOpenedEmpty;

		protected override void FromDataImpl(EntityData data) { }

		public override EntityData ToDataImpl() {
			throw new System.NotImplementedException();
		}

		public bool IsPickedUp { get; private set; }
		public void TryPickUp() {
			if ((State & 0x2) != 0 && (State & 0x1) == 0) return;
			IsPickedUp = true;
			OnSetState();
		}

		protected override void OnSetState() {
			base.OnSetState();
			if ((State & 0x2) != 0) { // Is chest
				if ((State & 0x1) != 0) // Is opened
					Renderer.sprite = IsPickedUp ? m_spriteChestOpenedEmpty : m_spriteChestOpened;
				else Renderer.sprite = m_spriteChestClosed;
			}
			else if (IsPickedUp) Renderer.sprite = null;
		}
	}
}
