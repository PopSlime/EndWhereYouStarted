namespace Coconut.Ewys.Entity {
	public class Treasure : EntityBase {
		protected override void FromDataImpl(EntityData data) { }

		public override EntityData ToDataImpl() {
			throw new System.NotImplementedException();
		}

		public override bool IsVisible => base.IsVisible && !IsPickedUp;

		public bool IsPickedUp { get; private set; }
		public void PickUp() {
			IsPickedUp = true;
			UpdateState();
		}
	}
}
