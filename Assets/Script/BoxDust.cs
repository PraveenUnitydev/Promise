/*
* Copyright (c) ArtoGames
* PraveenKrishnaGames
*/

using UnityEngine;

public class BoxDust : MonoBehaviour
{
	[SerializeField] ParticleSystem dust;
	Transform defaultTrans;
	private void Start()
	{
		Transform defaultTrans = gameObject.transform;
	}
	private void Update()
	{
		if (gameObject.transform!=defaultTrans)
		{
			CreateDust();
		}
	}
	
	
	void CreateDust()
	{
		dust.Play();
		if (dust.isPlaying)
		{
			dust.Stop();
		}
	
	}
}
