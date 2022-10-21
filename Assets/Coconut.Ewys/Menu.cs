using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Coconut.Ewys {
	public class Menu : MonoBehaviour {
		/// <summary>
		/// Container used to hold the level selection button
		/// </summary>
		[SerializeField]
		GameObject m_levelMenu;

		[SerializeField]
		GameObject m_levelButtonPrefab;

		void Start() {
			LevelController.LoadIndex();
			for (int i = 0; i < LevelController.LevelCount - 1; i++) {
				var b = Instantiate(m_levelButtonPrefab);
				var t = b.transform .GetChild(0);
				t.GetComponent<Text>().text = (i + 1).ToString();
				b.transform.SetParent(m_levelMenu.transform, false);
				int j = i;
				b.GetComponent<Button>().onClick.AddListener(() => LoadLevel(j));
			}
		}

		public void LoadLevel(int level) {
			LevelController.CurrentLevel = level;
			SceneManager.LoadScene("Level");
		}
	}
}
