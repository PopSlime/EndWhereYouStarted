namespace Coconut.Ewys.Entity {
	public class Treasure : EntityBase {
		protected override void FromDataImpl(EntityData data) { }

		public override EntityData ToDataImpl() {
			throw new System.NotImplementedException();
		}

		public override bool IsVisible => base.IsVisible && !_pickedUp;

		bool _pickedUp;
		public void PickUp() {
			_pickedUp = true;
			UpdateState();
		}
	}
}
