/*
* Copyright (c) ArtoGames
* PraveenKrishnaGames
*/

using UnityEngine;
using UnityEngine.Events;


public class AnimatorFunctionsPk : MonoBehaviour
{
	//public GameObject MenuButtons;

    public UnityEvent _onButtonClicked;


  /*	public void CameraIntroFinishes()
	{
		MenuButtons.SetActive(true);
	}
    */
	public void ButtonsClicked()
	{
        _onButtonClicked.Invoke();
	}
}
