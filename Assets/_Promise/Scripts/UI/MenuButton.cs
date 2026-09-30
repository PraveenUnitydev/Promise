using UnityEngine;
public class MenuButton : MonoBehaviour
{
	[SerializeField] MenuController menuController;
	[SerializeField] Animator animator;
	[SerializeField] int thisIndex;
	[SerializeField] AnimatorFunctionsPk animatorFunctions;

	private void Start()
	{
		animator = GetComponent<Animator>();
	}
	private void Update()
	{
		if (menuController.index == thisIndex)
		{
			animator.SetBool("Selected", true);
			if (GameInput.SubmitPressed)
			{
				animatorFunctions.ButtonsClicked();
				animator.SetBool("Clicked", true);	
			}else if (animator.GetBool("Clicked"))
			{
				animator.SetBool("Clicked", false);
			}
		}
		else
		{
			animator.SetBool("Selected", false);
		}
	}
}
