using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Coconut.Ewys {
	/// <summary>
	/// Used to control UI 
	/// </summary>
	public class UIControl : MonoBehaviour {
		public static UIControl Instance;

		[SerializeField]
		GameObject m_explainInfo;

		[SerializeField]
		/// <summary>
		/// the transform of hour hand
		/// </summary>
		RectTransform m_hourHandTransform;

		[SerializeField]
		Text m_stepNumberText;
		[SerializeField]
		Text m_explainText;

		private void Awake() {
			Instance = this;
			switch (LevelController.CurrentLevel) {
				case 0:
					SetExplainText("�е�͸��������ֻ�����ϲŻ����\nҪ�����е����Ƕ��õ�����Ŷ");
					break;
				case 1:
					SetExplainText("�����ϰ���ͱ߽�Ļ�,�ͻᶯ����");
					break ;
				case 2:
					SetExplainText("�������Ƶ�̤����,���ܴ򿪱���,������Ҳ������Ŷ");
                    break;
				case 3:
					SetExplainText("���˴�����,�ͻᱻ���͵���һ��");
                    break;
				case 4:
					SetExplainText("����Tab��,�Ϳ����л���Ŷ,��ע�������䲽��");
                    break;
				case 6:
					SetExplainText("�е����ӵ���ҹ��Ҳ���������");
                    break;
				default:
					SetExplainText("���Ͱ�,����");
					break;
			}

			m_stepNumberText.text = LevelController.Instance.GetStep().ToString();
			
		}
		

		void SetExplainText(string explainText) {
			this.m_explainText.text = explainText;
			//LevelController.
		}


		/// <summary>
		/// Restart the game
		/// </summary>
		public void Remake() {
			SceneManager.LoadScene("level");
		}

		public void ShowExplainInfo() {
			m_explainInfo.SetActive(true);
		}

		public void HideExplainInfo() {
			m_explainInfo.SetActive(false);
		}
		/// <summary>
		/// When player costinng the step, call this function to rotate the clock and decline step number of showing
		/// </summary>
		public void RotateClock(int remainStep, int totalStep,bool isUndo=false) {
			m_stepNumberText.text = remainStep.ToString();
			//TODO If mulitple coroutines exist at the same time
			StartCoroutine(RotateAround(360 / totalStep, 0.1f,isUndo?1:-1));
		}


		IEnumerator RotateAround(float angel, float time,int dir)//Я�̺��� IEnumerator��ͷ
		{
			float number = time/Time.fixedDeltaTime;//��������
			float nextAngel = dir*angel / number;//����Ҫת���� �̶�֡��

			for (int i = 0; i < number; i++) {
				m_hourHandTransform.transform.Rotate(new Vector3(0, 0, nextAngel));//ת��
				yield return new WaitForFixedUpdate();//�̶�֡����һ֡��ִ��
			}

		}
	}
}
