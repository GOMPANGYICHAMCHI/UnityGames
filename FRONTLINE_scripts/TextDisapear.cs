using UnityEngine;

public class TextDisapear : MonoBehaviour 
{
	public Animator animator;

	void Start()
	{
		AnimatorClipInfo[] clipinfo = animator.GetCurrentAnimatorClipInfo(0);
		Destroy(gameObject,clipinfo[0].clip.length);
	}
}