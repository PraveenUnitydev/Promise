/*
* Copyright (c) ArtoGames
* PraveenKrishnaGames
*/

using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
	public static LevelChanger instance = null;
	Animator animator;
	private int levelToLoad;
	private void Awake()
	{
		// One per level (it lives in GameManager.prefab); each scene's copy plays its own fade-in.
		// It used to call DontDestroyOnLoad, but it sits on a child object where that has no effect.
		instance = this;

		animator = GetComponent<Animator>();
		Debug.Log("Total scenes " + SceneManager.sceneCountInBuildSettings);
	}
	public int i = 0;
	public void FadeToNextLevel()
	{
		i = SceneManager.GetActiveScene().buildIndex;

		if (i < SceneManager.sceneCountInBuildSettings-1)
        {
			FadeToLevel(SceneManager.GetActiveScene().buildIndex + 1);
			
		}
        else
        {
			FadeToLevel(0);
        }
		
	}
	public void FadeToLevel(int levelIndex)
	{
		levelToLoad = levelIndex;
		animator.SetTrigger(("FadeOut"));
	}
	public void OnFadeComplete()
	{
		SceneManager.LoadScene(levelToLoad);
	}
	public void LoadCurrentLevel()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
	
}
