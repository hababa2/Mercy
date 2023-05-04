using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FillBar : MonoBehaviour
{
	[SerializeField] RectTransform backing;
	[SerializeField] RectTransform fill;
	[SerializeField] bool horizontal = true;

	private float length;
	private float width;

	private void Awake()
	{
		if (horizontal)
		{
			length = backing.sizeDelta.x;
			width = backing.sizeDelta.y;
			fill.sizeDelta = new Vector2(length, width);
		}
		else
		{
			length = backing.sizeDelta.y;
			width = backing.sizeDelta.x;
			fill.sizeDelta = new Vector2(width, length);
		}
	}

	public void SetPercent(float percent)
	{
		if(horizontal) { fill.sizeDelta = new Vector2(length * percent, width); }
		else { fill.sizeDelta = new Vector2(width, length * percent); }
	}
}
