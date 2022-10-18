namespace Coconut.Ewys.Entity {
	public class DummyEntity : EntityBase {
		protected override void FromDataImpl(EntityData data) { }

		public override EntityData ToDataImpl() {
			throw new System.NotImplementedException();
		}
	}
}
