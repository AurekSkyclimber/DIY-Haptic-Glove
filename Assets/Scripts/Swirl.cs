using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swirl: MonoBehaviour {
	public Material swirl;

	void OnRenderImage ( RenderTexture src, RenderTexture dest ) {
		Graphics.Blit(src, dest, swirl);
	}
}
