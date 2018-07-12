using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Pathfinding;

public class TargetReached : AILerp
{

    public UnityEvent OnDestinationReached = new UnityEvent();

    public override void OnTargetReached()
    {
        base.OnTargetReached();
        OnDestinationReached.Invoke();
    }
}
