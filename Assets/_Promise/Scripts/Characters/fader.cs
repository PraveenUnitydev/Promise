using UnityEngine;
using DragonBones;

public class fader : MonoBehaviour
{
    static readonly int FadeId = Shader.PropertyToID("_Fade");

    bool isDissolving=false;
    // Whitey's shared Dissolve material. Kept for reference only: the fade is applied per renderer with a
    // MaterialPropertyBlock, so the material asset (also used by Whitey in the main menu) is never modified.
    public Material Dis;
    float fade=1f;
    [SerializeField] private UnityArmatureComponent Whitey;
    MaterialPropertyBlock _block;

	private void Start()
    {
        _block = new MaterialPropertyBlock();
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
			ApplyFade(fade);
		}
    }

    // Re-applied every frame while dissolving: DragonBones can swap slot renderers and materials at any time.
    void ApplyFade(float value)
    {
        foreach (var r in Whitey.GetComponentsInChildren<MeshRenderer>())
        {
            r.GetPropertyBlock(_block);
            _block.SetFloat(FadeId, value);
            r.SetPropertyBlock(_block);
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
