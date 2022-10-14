namespace Coconut.Ewys {
	public static class Game {
		static bool _init;
		public static void Init() {
			if (_init) return;
			_init = true;
		}
	}
}
