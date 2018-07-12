using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
	private Image model;					// DH: Image in the child GameObject

	private Color cleanColor = Color.red;

	public float speed = 1f;
	public bool normalSpeed = true;			// DH: false for 1/3 speed, when "splipping"

	public Transform gridTransform;
	public NodeIndex currentNodeIndex;
	public NodeIndex desiredEndNodeIndex;

	public void Awake()
	{
		model = gameObject.GetComponentInChildren<Image>();

        
	}

    public void SetPosition(NodeIndex desiredNodeIndex, Transform trans)
	{
		Vector3 pos = transform.position;
		pos = trans.position;
		transform.position = pos;

		currentNodeIndex = desiredNodeIndex;

//		Debug.Log("Player::SetPosition: currentNodeIndex: " + currentNodeIndex.x + ", " + currentNodeIndex.y);
	}

	public IEnumerator MoveWait(NodeIndex desiredNodeIndex, Transform trans)
	{
//		Debug.Log("Player::MoveWait ... FROM : " + currentNodeIndex.x + ", " + currentNodeIndex.y + " : TO : " + desiredNodeIndex.x + ", " + desiredNodeIndex.y);

		if (trans && gridTransform)
		{
			float scaleFactor = 1.1f * speed;
			if (!normalSpeed)
			{
				scaleFactor *= .333333f;
			}
			float completionTime = Vector3.Distance(transform.position, trans.position) / scaleFactor;

			yield return StartCoroutine(MoveOverTime(transform.position, trans.position, completionTime));
			normalSpeed = true;
		}

		currentNodeIndex = desiredEndNodeIndex;
		
		yield return null;
	}
	 
	public IEnumerator CleanWait(float time = 4f)
	{
		if (model)
		{
//			Debug.Log("Player::Clean Wait: " + time);

			Color startColor = model.color;

			model.color = cleanColor;

			yield return new WaitForSeconds(time);

			model.color = startColor;
		}
		else
		{
			Debug.LogError("Player::CleanWait: No model Image...");
		}
	}

	public IEnumerator MoveOverTime (Vector3 startPos, Vector3 endPos, float time)
	{
		float t = 0f;
		float rate = 1f / time;
		while (t < 1.0)
		{
			t += Time.deltaTime * rate;
			transform.position = Vector3.Lerp(startPos, endPos, t);
			yield return null; 
		}
	}

    public void WeShouldMove()
    {
        Debug.Log("We should move");
    }
}
