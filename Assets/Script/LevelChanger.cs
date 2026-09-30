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
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else if (instance != this)
		{
			Destroy(gameObject);
		}
		
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
