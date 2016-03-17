using UnityEngine;
using System;
using System.Collections;

public enum RoleAction
{
	Dive,
	Fight,
}

public interface IRole
{
	Action<IRole, string> AnimEventCallback { set; }

	void Initial();

	void SetCommand(RoleCommand _command, params System.Object[] _params);
}
