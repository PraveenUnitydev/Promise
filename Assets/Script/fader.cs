using UnityEngine;
using DragonBones;

public class fader : MonoBehaviour
{
    bool isDissolving=false;
    public Material Dis;
    float fade=1f;
    [SerializeField] private UnityArmatureComponent Whitey;
	private void Start()
    {
        Dis.SetFloat("_Fade",1f);
        Whitey.animation.FadeIn("Idle", .25f, -1, 0);     
    }
  private void Update() {
        if (isDissolving)
		{
			fade -= Time.deltaTime;

			if (fade <= 0f)
			{
				fade = 0f;
				isDissolving = false;
			}
			Dis.SetFloat("_Fade", fade);
		}
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
       if(collision.gameObject.tag == "Player")
        {
            FindAnyObjectByType<LevelChanger>().FadeToNextLevel();
             isDissolving=true;
            collision.gameObject.GetComponent<PlayerMovement>()._freeze=true;
        }
    }
}  
