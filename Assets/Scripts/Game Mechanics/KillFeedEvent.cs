using System.Collections;
using System.Collections.Generic;

public class KillFeedEvent {
	public string killerName;
	public string deadManName;
	public string deathType;
	public float eventEndTime;
	public KillFeedEvent (string _killerName, string _deadManName, string _deathType, float _eventEndTime) {
		killerName = _killerName;
		deadManName = _deadManName;
		deathType = _deathType;
		eventEndTime = _eventEndTime;
	}
	public override string ToString() {
		return killerName + deathType + deadManName;
	}
}
