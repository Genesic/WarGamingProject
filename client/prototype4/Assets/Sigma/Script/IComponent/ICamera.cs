using UnityEngine;
using System.Collections;

public class CameraType
{
	public const int IDLE = 0;
	public const int DIVE = 1;
	public const int FIGHT = 2;
}

public interface ICamera 
{
	void Initial();

	RenderTexture TexRender { get; }

	void SetCommand(CameraCommand _command, params System.Object[] _params);
}
