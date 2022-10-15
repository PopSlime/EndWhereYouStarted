using UnityEngine;
using UnityEngine.SceneManagement;

namespace Coconut.Ewys {
	public class ToMenu : MonoBehaviour {
		void Update() {
			//Load menu scene when player press mouse or space
			if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Mouse0)) {
				SceneManager.LoadSceneAsync("Menu");
			}
		}
	}
}
