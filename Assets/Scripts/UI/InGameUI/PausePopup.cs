using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PausePopup : MonoBehaviour
{
    public void RestryStage()
	{
		SceneManager.LoadScene("PlayScene");
	}

	public void LoadLobbyScene()
	{
		SceneManager.LoadScene("StageSelectScene");
	}
}
