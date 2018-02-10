using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTipDriver : MonoBehaviour {
    private Clickable dismissButton;
    private Animator anim;

    // Use this for initialization
	void Start ()
	{
	    var allClickables = GetComponentsInChildren<Clickable>();
	    foreach (var clickable in allClickables)
	    {
            Debug.Log(clickable.gameObject.name);
	        if (clickable.gameObject.name == "dismissButton")
	        {
	            dismissButton = clickable;
	        }
	    }
        dismissButton.setClickReleaseCallback(dismiss);

	    anim = GetComponent<Animator>();

	}

    public void dismiss()
    {
        anim.SetTrigger("Die");
    }

    public void TipDie()
    {
        GameObject.Destroy(this.gameObject);
    }
}
