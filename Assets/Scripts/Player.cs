using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

/**
 * Player controller script. This is the only script modified from the original implementation. It may break the old Demo implementation
 * */
public class Player : MonoBehaviour
{


    public float baseSpeed = 3.0f;
	public float speed = 1f;
	public bool normalSpeed = true;			// DH: false for 1/3 speed, when "splipping"
    public bool isMoving;
    public bool playerWillStopToClean;


    public Transform gridTransform;
	public NodeIndex currentNodeIndex;
	public NodeIndex desiredEndNodeIndex;

    [SerializeField]
    private float SpeedReduction = 3.0f;
    private NodeIndex targetNodeIndex;
    private Color startColor = Color.white;
    public TargetReached lerp;
    private AIDestinationSetter aIDestinationSetter;
    private bool isCleaning = false;
    private Vector3 targetPosition;
    private SpriteRenderer model;                   // DH: Image in the child GameObject

    private Color cleanColor = Color.red;

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

        lerp.OnDestinationReached.AddListener(delegate { DestinationReached(); });
    }


    public void MoveToTarget(NodeIndex targetNodeIndex, Transform target)
    {
        
        lerp.speed = speed;
        aIDestinationSetter.target = target;
        SetPosition(targetNodeIndex, target);
    }

    public void SetPosition(NodeIndex desiredNodeIndex, Transform trans)
	{
        targetPosition = trans.position;

        targetNodeIndex = desiredNodeIndex;
	}

    public void ReduceSpeed()
    {
        normalSpeed = false;
        lerp.speed = speed / SpeedReduction;
    }

    public void ResetSpeed()
    {
        normalSpeed = true;
        lerp.speed = speed;
    }
	 
	public IEnumerator CleanWait(float time = 4f)
	{
		if (model)
		{
            isCleaning = true;

            model.color = cleanColor;

			yield return new WaitForSeconds(time);

			model.color = startColor;
            GridManager.instance.CleanSpill(currentNodeIndex);
            isCleaning = false;
		}
		else
		{
			Debug.LogError("Player::CleanWait: No model Image...");
		}
	}

    private void DestinationReached()
    {
        //Because A* is constantly checking path's we need to ensure that we've reached our destination by distance as well. 
        if (Mathf.Abs(transform.position.x - targetPosition.x) < 0.2  && Mathf.Abs(transform.position.y - targetPosition.y) < 0.2)
        {
            //This line is necessary to keep flo from appaering behind the the spills. 
            transform.position = transform.position - Vector3.forward;

            currentNodeIndex = targetNodeIndex;
            if (GridManager.instance.tiles[currentNodeIndex.x, currentNodeIndex.y].conatinsSplill)
            {
                if (!isCleaning)
                {
                    StopAllCoroutines();
                    StartCoroutine(CleanWait());
                }

                
            }
        }
    }
}
