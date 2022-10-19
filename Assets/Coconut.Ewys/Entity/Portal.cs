using System;

namespace Coconut.Ewys.Entity {
	public class Portal : EntityBase {
		public int? TargetID { get; private set; }

		protected override void FromDataImpl(EntityData data) {
			var d = data as PortalData;
			TargetID = d.target;
		}

		public override EntityData ToDataImpl() {
			throw new NotImplementedException();
		}
	}
}
