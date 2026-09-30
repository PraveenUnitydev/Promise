/*
* Copyright (c) ArtoGames
* PraveenKrishnaGames
*/
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
	public int index;
	[SerializeField] bool keyDown;
	[SerializeField] int maxIndex;
	//[SerializeField] GameObject LoadingScreen;

   

    private void Update()
	{
		if (GameInput.Navigate.y != 0)
		{
			if (!keyDown)
			{
				if (GameInput.Navigate.y < 0)
				{
					if (index < maxIndex)
					{
						index++;
					}
					else
					{
						index = 0;
					}
				}else if (GameInput.Navigate.y > 0)
				{
					if (index > 0)
					{
						index--;
					}
					else
					{
						index = maxIndex;
					}
				}
				keyDown = true;
			}
		}
		else
		{
			keyDown = false;
		}
	}
	/*public void LoadLevel(int sceneIndex)
	{
		StartCoroutine(LoadAsynchronously(sceneIndex));
		
	}
	IEnumerator LoadAsynchronously(int sceneIndex)
	{
		AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
		LoadingScreen.SetActive(true);
		while (!operation.isDone)
		{
			float progress = Mathf.Clamp01(operation.progress / .9f);
			Debug.Log(progress);
			yield return null;
		}
	}*/
    ///////////////////////////// MENU FUNCTIONS   /////////////////////////
    ///

    public void NewGame()
    {
		//clear PlayerPrefs and start new game
		//LoadLevel(1);
		FindAnyObjectByType<LevelChanger>().FadeToLevel(1);

	}
	public void Restart()
    {
		FindAnyObjectByType<LevelChanger>().LoadCurrentLevel();
    }
	public void MainMenu()
    {
		FindAnyObjectByType<LevelChanger>().FadeToLevel(0);
	}


    public void Options()
    {

    }
    public void Resume()
    {
        FindAnyObjectByType<GameManagerPk>().ResumeGame();
       
    }
    public void Quit()
    {
        Application.Quit();
    }






	
}
