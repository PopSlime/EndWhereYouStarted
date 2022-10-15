using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Coconut.Ewys {

	public class Menu : MonoBehaviour {
		/// <summary>
		/// Container used to hold the level selection button
		/// </summary>
		public GameObject levelMenu;

		/// <summary>
		/// 
		/// </summary>
		public GameObject levelButtonPrefab;
		// Start is called before the first frame update
		void Start() {
			//Automatic layout of level selection button
			for (int i = 0; i < 4; i++) {
				for (int j = 0; j < 4; j++) {
					var b = Instantiate(levelButtonPrefab);
					b.transform.parent = levelMenu.transform;
					b.GetComponent<RectTransform>().anchorMin = new Vector2((j + 1) * 0.025f, (i + 1) * 0.025f);
					b.GetComponent<RectTransform>().anchorMax = new Vector2((j + 1) * 0.225f, (i + 1) * 0.225f);
					b.GetComponent<Button>().onClick.AddListener(() => LoadLevel(i * 4 + j + 1));
				}
			}
		}

		// Update is called once per frame
		void Update() {

		}
		/// <summary>
		/// TODO 
		/// </summary>
		/// <param name="level"></param>
		public void LoadLevel(int level) { 
			
		}
	}
}


