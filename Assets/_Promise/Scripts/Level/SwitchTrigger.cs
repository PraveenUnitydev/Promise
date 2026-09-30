using UnityEngine;
public class SwitchTrigger : MonoBehaviour
{
	Animator Anim;
	public GameObject light1,light2;
	[SerializeField] Platform platform;
	bool lights = false;
	private void Awake()
	{
		Anim = GetComponent<Animator>();
	}
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			Anim.SetBool("Switch_On", true);
			SwitchOn();
		}
	}
	private void Update()
	{
		if (lights == true)
		{
			light1.SetActive(true);
			light2.SetActive(true);
		}
	}
	private void SwitchOn()
	{		
		platform.On = true;
		lights = true;
	}
	

}
