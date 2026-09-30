using UnityEngine;
using MyBox;
public class Platform:MonoBehaviour
{
	public bool On;
	/// <summary>
	/// //////////////////////////Platform Enums
	/// </summary>
	public enum PlatformType
	{
		None,
		Moving,
		Appearing,
		Falling,
		Raising
	}
	public PlatformType platformType = PlatformType.None;
	/// <summary>
	/// ///////////////////////////Moving platform Variables
	/// </summary>
	[ConditionalField(nameof(platformType), false, PlatformType.Moving)] [SerializeField] Transform pos1, pos2;
	[ConditionalField(nameof(platformType), false, PlatformType.Moving)] [SerializeField] float speed;
	[ConditionalField(nameof(platformType), false, PlatformType.Moving)] [SerializeField] Transform startPos;
	Vector3 nextPos;
	/// <summary>
	/// //////////////////////////Appearing platform Variables
	/// </summary>
	[ConditionalField(nameof(platformType), false, PlatformType.Appearing)] [SerializeField] GameObject Platf;
	/// <summary>
	/// //////////////////////////Falling platform Variables
	/// </summary>
	[ConditionalField(nameof(platformType), false, PlatformType.Falling)] [SerializeField] Rigidbody2D rb2d;
	[ConditionalField(nameof(platformType), false, PlatformType.Falling)] [SerializeField] Animator animator;
	/// <summary>
	/// /////////////////////////Raising Platform Variables
	/// </summary>
	[ConditionalField(nameof(platformType), false, PlatformType.Raising)] [SerializeField] float Rspeed;
	[ConditionalField(nameof(platformType), false, PlatformType.Raising)] [SerializeField] float Rplatform;
	private void Awake()
	{
		if (platformType == PlatformType.Moving)
		{
			nextPos = startPos.position;
		}
		if (platformType == PlatformType.Falling)
		{
			rb2d = GetComponent<Rigidbody2D>();
		}
	}
	private void Update()
	{
		if (platformType == PlatformType.Moving)
		{
			if (On)
			{
				if (transform.position == pos1.position)
				{
					nextPos = pos2.position;
				}
				if (transform.position == pos2.position)
				{
					nextPos = pos1.position;
				}
				transform.position = Vector3.MoveTowards(transform.position, nextPos, speed * Time.deltaTime);
			}
		}
		if (platformType == PlatformType.Appearing)
		{
			Platf.SetActive(On);
		}
		if (platformType == PlatformType.Raising)
		{
			if (On)
			{
				Vector3 RplatformTrans = new Vector3(transform.position.x, Rplatform, transform.position.z);
				transform.position = Vector3.MoveTowards(transform.position, RplatformTrans, Rspeed * Time.deltaTime);
			}
		}
	}
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (platformType == PlatformType.Moving)
		{
			if (collision.gameObject.tag == "Player")
			{
				collision.collider.transform.SetParent(transform);
			}
		}
		

		//Debug.Log("Colliding with " + collision.gameObject.name);
	}
	private void OnCollisionExit2D(Collision2D collision)
	{
		if (platformType == PlatformType.Moving)
		{
			if (collision.gameObject.tag == "Player")
			{
				collision.collider.transform.SetParent(null);
			}
		}
	}
	void FallPlatform()
	{
		gameObject.transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
	}
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (platformType == PlatformType.Falling)
		{
			if (collision.gameObject.tag == "Player")
			{
				animator.SetBool("Shake", true);
				Invoke("FallPlatform", 1f);
			}
		}
	}

}
