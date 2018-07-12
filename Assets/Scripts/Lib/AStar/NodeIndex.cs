using UnityEngine;

[System.Serializable]
public class NodeIndex
{
	public NodeIndex() {}
	public NodeIndex(int _x, int _y)
	{
		x = _x;
		y = _y;
	}

	public int x = -1;	// DH: ROW Index
	public int y = -1;  // DH: COLUMN Index
}