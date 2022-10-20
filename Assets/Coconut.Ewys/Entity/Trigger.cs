namespace Coconut.Ewys.Entity {
	public class Trigger : EntityBase {
		public int TargetID { get; private set; }
		public int Flag { get; private set; }
		public bool IsInverse { get; private set; }

		protected override void FromDataImpl(EntityData data) {
			var d = data as TriggerData;
			TargetID = d.target;
			Flag = d.flag;
			IsInverse = d.inverse;
			// TODO d.duration
		}

		public override EntityData ToDataImpl() {
			throw new System.NotImplementedException();
		}
	}
}
