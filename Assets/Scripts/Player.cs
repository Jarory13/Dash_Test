using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Player : MonoBehaviour
{
	private SpriteRenderer model;					// DH: Image in the child GameObject

	private Color cleanColor = Color.red;

	public float speed = 1f;
	public bool normalSpeed = true;			// DH: false for 1/3 speed, when "splipping"
    public bool isMoving;
    public bool playerWillStopToClean;


    public Transform gridTransform;
	public NodeIndex currentNodeIndex;
	public NodeIndex desiredEndNodeIndex;

    private Color startColor = Color.white;
    private Seeker seeker;
    private AILerp lerp;
    private AIDestinationSetter aIDestinationSetter;


    public void Awake()
	{
		model = gameObject.GetComponent<SpriteRenderer>();

        if (model)
        {
            startColor = model.color;
        }
        lerp = GetComponent<AILerp>();
        aIDestinationSetter = GetComponent<AIDestinationSetter>();
    }


    public void MoveToTarget(NodeIndex targetNodeIndex, Transform target)
    {
        lerp.speed = speed;
        aIDestinationSetter.target = target;
        SetPosition(targetNodeIndex, target);
    }

    public void SetPosition(NodeIndex desiredNodeIndex, Transform trans)
	{
		Vector3 pos = transform.position;
		pos = trans.position;
		//transform.position = pos;

		currentNodeIndex = desiredNodeIndex;
	}
	 
	public IEnumerator CleanWait(float time = 4f)
	{
		if (model)
		{
            //			Debug.Log("Player::Clean Wait: " + time);

			model.color = cleanColor;

			yield return new WaitForSeconds(time);

			model.color = startColor;
		}
		else
		{
			Debug.LogError("Player::CleanWait: No model Image...");
		}
	}
}
