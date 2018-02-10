using System.Collections;
using System.Collections.Generic;
using Assets.common;
using UnityEngine;

public class ToolTipDriver : MonoBehaviour
{
    public List<Sprite> AllTips;

    private Clickable dismissButton;
    private Animator anim;

    private static RandomSpinner<Sprite> spinner;
    private SpriteRenderer tipSprite;

    // Use this for initialization
    void Start()
    {
        var allClickables = GetComponentsInChildren<Clickable>();
        foreach (var clickable in allClickables)
        {
            if (clickable.gameObject.name == "dismissButton")
            {
                dismissButton = clickable;
            }
        }
        dismissButton.setClickReleaseCallback(dismiss);

        anim = GetComponent<Animator>();

        //choose random sprite
        if (spinner == null)
        {
            spinner = new RandomSpinner<Sprite>();
            AllTips.ForEach(t => spinner.addNewPossibility(5, t));
        }

        var things = this.GetComponentsInChildren<SpriteRenderer>();
        foreach (var renderer in things)
        {
            if (renderer.gameObject.name == "tip")
            {
                tipSprite = renderer;
            }
        }

        tipSprite.sprite = spinner.getRandom();

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
