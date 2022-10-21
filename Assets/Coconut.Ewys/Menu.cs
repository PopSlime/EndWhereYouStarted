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
			//	for (int j = 0; j < 4; j++) {
			//		var b = Instantiate(m_levelButtonPrefab);
			//		b.transform.parent = m_levelMenu.transform;
			//		b.GetComponent<RectTransform>().anchorMin = new Vector2((j + 1) * 0.025f, (i + 1) * 0.025f);
			//		b.GetComponent<RectTransform>().anchorMax = new Vector2((j + 1) * 0.225f, (i + 1) * 0.225f);
			//		b.GetComponent<Button>().onClick.AddListener(() => LoadLevel(i * 4 + j + 1));
			//	}
			//}
				var b = Instantiate(m_levelButtonPrefab);
				var t = b.transform .GetChild(0);
				t.GetComponent<Text>().text = (i+1).ToString();
				b.transform.SetParent(m_levelMenu.transform, false);
				int j = i;
				b.GetComponent<Button>().onClick.AddListener(() => LoadLevel(j));
			}

		}

		/// <summary>
		/// TODO 
		/// </summary>
		/// <param name="level"></param>
		public void LoadLevel(int level) {
			LevelController.CurrentLevel = level;
			SceneManager.LoadScene("Level");
		}
	}
}
