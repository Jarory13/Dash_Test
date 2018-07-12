using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Player : MonoBehaviour
{
	private SpriteRenderer model;					// DH: Image in the child GameObject

	private Color cleanColor = Color.red;

    public float baseSpeed = 3.0f;
	public float speed = 1f;
	public bool normalSpeed = true;			// DH: false for 1/3 speed, when "splipping"
    public bool isMoving;
    public bool playerWillStopToClean;


    public Transform gridTransform;
	public NodeIndex currentNodeIndex;
	public NodeIndex desiredEndNodeIndex;

    private NodeIndex targetNodeIndex;
    private Color startColor = Color.white;
    private Seeker seeker;
    public TargetReached lerp;
    private AIDestinationSetter aIDestinationSetter;
    private bool speedreduced = false;

    public void Awake()
	{
		model = gameObject.GetComponent<SpriteRenderer>();

        if (model)
        {
            startColor = model.color;
        }
        lerp = GetComponent<TargetReached>();
        aIDestinationSetter = GetComponent<AIDestinationSetter>();

        speed = baseSpeed;

        lerp.OnDestinationReached.AddListener(delegate { CheckForSpill(); });
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

        targetNodeIndex = desiredNodeIndex;
	}
	 
	public IEnumerator CleanWait(float time = 4f)
	{
		if (model)
		{
            //			Debug.Log("Player::Clean Wait: " + time);

			model.color = cleanColor;

			yield return new WaitForSeconds(time);

			model.color = startColor;
            GridManager.instance.CleanSpill(currentNodeIndex);
		}
		else
		{
			Debug.LogError("Player::CleanWait: No model Image...");
		}
	}

    private void CheckForSpill()
    {
        currentNodeIndex = targetNodeIndex;
        if (GridManager.instance.tiles[currentNodeIndex.x, currentNodeIndex.y].conatinsSplill)
        {
            StartCoroutine(CleanWait());
        }
    }
}
