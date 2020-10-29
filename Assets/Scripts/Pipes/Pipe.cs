using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour 
{

	public float pipeRadius;
	public int pipeSegmentCount;

	public float ringDistance;

	public float minCurveRadius, maxCurveRadius;
	public int minCurveSegmentCount, maxCurveSegmentCount;

	float curveRadius;
	int curveSegmentCount;

	Mesh mesh;
	Vector3[] vertices;
	Vector2[] uv;
	int[] triangles;

	float curveAngle;
	float relativeRotation;

	float timeToPassPipe = 0;

	public RandomPlacer notePlacer;

	private void Awake () 
	{
		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.name = "Pipe";
	}


	public void GeneratePipe() 
	{
		curveRadius = Random.Range(minCurveRadius, maxCurveRadius);
		curveSegmentCount = Random.Range(minCurveSegmentCount, maxCurveSegmentCount + 1);
		mesh.Clear();
		SetVertices();
		SetUV();
		SetTriangles();
		mesh.RecalculateNormals();
	}

	public float GenerateMusicNotes(List<(float, float)> songMusicNotes, float timeWhenPipeEntered,float worldAbsoluteRotation, bool isFirstPipe = false)
    {
		Player player = GameObject.FindGameObjectWithTag("Player").GetComponent(typeof(Player)) as Player;

		// How long will it take to pass this pipe
		float arcLength = curveAngle / 360 * (2f * Mathf.PI * curveRadius);
		timeToPassPipe = arcLength / player.velocity;

		// Since we start the game at the second pipe, do not generate notes for the first one
		if (!isFirstPipe)
		{
			List<(float, float, float)> notesForThisPipe = new List<(float, float, float)> { };
			for (int i = 0; i < songMusicNotes.Count; i++)
			{
				float noteAppearTime = songMusicNotes[i].Item1;
				if (noteAppearTime > timeWhenPipeEntered && noteAppearTime < timeWhenPipeEntered + timeToPassPipe)
				{
					noteAppearTime -= timeWhenPipeEntered;

					float noteAppearSegment = Remap(noteAppearTime, 0, timeToPassPipe, 0, curveAngle);
					(float, float, float) tmp = (noteAppearSegment, songMusicNotes[i].Item2, songMusicNotes[i].Item1);

					notesForThisPipe.Add(tmp);
				}
			}

			// Generate notes for this pipe
			notePlacer.GenerateItems(this, notesForThisPipe);

			// Remove notes for this pipe from original list to avoid repetition
			for (int i = 0; i < notesForThisPipe.Count; i++)
			{
				for (int j = 0; j < songMusicNotes.Count; j++)
				{
					if (notesForThisPipe[i].Item1 == songMusicNotes[j].Item1)
					{
						songMusicNotes.RemoveAt(j);
						break;
					}
				}
			}
		}

		return timeWhenPipeEntered + timeToPassPipe;
	}

	public static float Remap(float value, float from1, float to1, float from2, float to2)
	{
		return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
	}

	private void SetVertices () 
	{
		vertices = new Vector3[pipeSegmentCount * curveSegmentCount * 4];

		float uStep = ringDistance / curveRadius;
		curveAngle = uStep * curveSegmentCount * (360f / (2f * Mathf.PI));
		CreateFirstQuadRing(uStep);
		int iDelta = pipeSegmentCount * 4;
		for (int u = 2, i = iDelta; u <= curveSegmentCount; u++, i += iDelta)
		{
			CreateQuadRing(u * uStep, i);
		}
		mesh.vertices = vertices;
	}

	private void CreateFirstQuadRing (float u) {
		float vStep = (2f * Mathf.PI) / pipeSegmentCount;

		Vector3 vertexA = GetPointOnTorus(0f, 0f);
		Vector3 vertexB = GetPointOnTorus(u, 0f);
		for (int v = 1, i = 0; v <= pipeSegmentCount; v++, i += 4)
		{
			vertices[i] = vertexA;
			vertices[i + 1] = vertexA = GetPointOnTorus(0f, v * vStep);
			vertices[i + 2] = vertexB;
			vertices[i + 3] = vertexB = GetPointOnTorus(u, v * vStep);
		}
	}

	private void CreateQuadRing (float u, int i) {
		float vStep = (2f * Mathf.PI) / pipeSegmentCount;
		int ringOffset = pipeSegmentCount * 4;

		Vector3 vertex = GetPointOnTorus(u, 0f);
		for (int v = 1; v <= pipeSegmentCount; v++, i += 4)
		{
			vertices[i] = vertices[i - ringOffset + 2];
			vertices[i + 1] = vertices[i - ringOffset + 3];
			vertices[i + 2] = vertex;
			vertices[i + 3] = vertex = GetPointOnTorus(u, v * vStep);
		}
	}

	private void SetUV () {
		uv = new Vector2[vertices.Length];
		for (int i = 0; i < vertices.Length; i+= 4)
		{
			uv[i] = Vector2.zero;
			uv[i + 1] = Vector2.right;
			uv[i + 2] = Vector2.up;
			uv[i + 3] = Vector2.one;
		}
		mesh.uv = uv;
	}

	private void SetTriangles () {
		triangles = new int[pipeSegmentCount * curveSegmentCount * 6];
		for (int t = 0, i = 0; t < triangles.Length; t += 6, i += 4) 
		{
			triangles[t] = i;
			triangles[t + 1] = triangles[t + 4] = i + 2;
			triangles[t + 2] = triangles[t + 3] = i + 1;
			triangles[t + 5] = i + 3;
		}
		mesh.triangles = triangles;
	}

	private Vector3 GetPointOnTorus (float u, float v) 
	{
		Vector3 p;
		float r = (curveRadius + pipeRadius * Mathf.Cos(v));
		p.x = r * Mathf.Sin(u);
		p.y = r * Mathf.Cos(u);
		p.z = pipeRadius * Mathf.Sin(v);
		return p;
	}

	public float AlignWith (Pipe pipe, float worldAbsoluteRotation) 
	{
		// Random rotation for a new pipe restricted by segment count to allign perfectly
		relativeRotation = Random.Range(0, curveSegmentCount) * 360f / pipeSegmentCount;

		// Futher restrictions to limit rotation in <-180, 180)
		if(relativeRotation > 360)
        {
			int tmp = (int)(relativeRotation / 360f);
			tmp *= 360;
			relativeRotation -= tmp;
        }
		if (relativeRotation >= 180)
			relativeRotation -= 360;

		worldAbsoluteRotation += relativeRotation;
		if (worldAbsoluteRotation < -180)
			worldAbsoluteRotation += 360;
		else if (worldAbsoluteRotation >= 180)
			worldAbsoluteRotation -= 360;

		transform.SetParent(pipe.transform, false);
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.Euler(0f, 0f, -pipe.curveAngle);
		transform.Translate(0f, pipe.curveRadius, 0f);
		transform.Rotate(relativeRotation, 0f, 0f);
		transform.Translate(0f, -curveRadius, 0f);
		transform.SetParent(pipe.transform.parent);
		transform.localScale = Vector3.one;

		return worldAbsoluteRotation;
	}

	public void destroyChildren()
    {
		foreach (Transform child in transform)
        {
			GameObject.Destroy(child.gameObject);
        }
    }

	public int GetPipeSegmentCount{ get { return pipeSegmentCount; } }
	public int GetCurveSegmentCount{ get { return curveSegmentCount; } }
	public float GetCurveAngle { get { return curveAngle; } }
	public float GetCurveRadius { get { return curveRadius; } }
	public float GetRelativeRotation { get { return relativeRotation; } }

	public float GetTimeToPassPipe { get { return timeToPassPipe; } }

}