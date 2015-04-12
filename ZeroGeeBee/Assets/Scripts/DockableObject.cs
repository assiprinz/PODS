using UnityEngine;
using System.Collections;

public class DockableObject : MonoBehaviour
{

	public DockingPort[] dockingPorts;

	public DockingPort GetFreeDockingPort ()
	{
		foreach (DockingPort port in dockingPorts) {
			if (!port.IsDocked ()) {
				return port;
			}
		}

		return null;
	}
}
