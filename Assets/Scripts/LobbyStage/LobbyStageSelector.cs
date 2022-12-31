using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyStageSelector : MonoBehaviour
{
    [SerializeField] Transform stagesTransform;
    [SerializeField] LayerMask mLevelLayerMask;
    [SerializeField] LevelSelectPopup levelSelectPopup;
    [SerializeField] ProgressData progressData;

    List<LobbyStage> stageList;
    LineRenderer lineRenderer;
    InputManager mInputManager;
	LobbyStage mCurrentLobbyStage;

	private void Awake()
	{
        lineRenderer = GetComponent<LineRenderer>();
	}

	void Start()
    {
        stageList = new List<LobbyStage>();
        foreach(Transform gameObject in stagesTransform)
            stageList.Add(gameObject.GetComponent<LobbyStage>());

        if (mCurrentLobbyStage == null)
            mCurrentLobbyStage = stageList[progressData.lastClearStage];

        SetCurrentStage(progressData.currentStage);
        SoundManager.Instance.Play("lobbySound", Sound.BGM);

        mInputManager = new InputManager(Camera.main.transform);
		levelSelectPopup.gameObject.SetActive(false);

        //클리어 애니메이션을 재생하지 않았다면
        if(!progressData.bPlayClearAnimation)
		{
			progressData.bPlayClearAnimation = true;
			StartCoroutine(LoadClearLobbyProcess());
		}
		else
		{
            SetLobbyMap();
            stageList[progressData.lastClearStage].AppearNextStage();
		}
    }

    void Update()
    {

        if(mInputManager.isTouchUp)
		{
            Debug.Log("click");
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mInputManager.touchPosition), Vector2.zero, float.MaxValue, mLevelLayerMask);
            if (hit.collider != null)
			{
                LobbyStage lobbyStage = hit.collider.GetComponent<LobbyStage>();
                SoundManager.Instance.PlayButtonSound();

                mCurrentLobbyStage = lobbyStage;
                Debug.Log($"select level : {mCurrentLobbyStage.stage}");

                progressData.currentStage = mCurrentLobbyStage.stage;
                levelSelectPopup.gameObject.SetActive(true);
                levelSelectPopup.SetStageInfo(progressData.currentStage);
            }
		}
    }

    // 스테이지 입장버튼
    public void LoadPlayScene()
	{
        SceneManager.LoadScene("PlayScene");
	}

    IEnumerator LoadClearLobbyProcess()
	{
        SetLobbyMapBeforeAnim();
        yield return mCurrentLobbyStage.PlayClearAnimation(progressData.GetStageStarCount(progressData.currentStage));
        yield return stageList[progressData.lastClearStage].AppearNextStage();
        Debug.Log("load Lobby");
        yield return null;
	}

    private void SetLobbyMapBeforeAnim()
	{
        foreach (LobbyStage stage in stageList)
        {
            if (stage.stage >= progressData.lastClearStage)
                break;
            stage.InitStage(progressData.GetStageStarCount(stage.stage));
        }
    }

    private void SetLobbyMap()
	{
        lineRenderer.positionCount = stageList.Count;
        lineRenderer.SetPositions(stageList.Select(x => x.transform.position).ToArray());

        foreach(LobbyStage stage in stageList)
		{
            if (stage.stage > progressData.lastClearStage)
                break;
            stage.InitStage(progressData.GetStageStarCount(stage.stage));
        }
        stageList[progressData.lastClearStage].SetIcon();
    }

    private void SetCurrentStage(int stage)
	{
        foreach(LobbyStage lobbyStage in stageList)
		{
            if (lobbyStage.stage == stage)
                mCurrentLobbyStage = lobbyStage;
		}
	}
}
