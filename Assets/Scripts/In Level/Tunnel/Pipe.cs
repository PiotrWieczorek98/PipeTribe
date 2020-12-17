using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour
{

	public float pipeRadius;
	public int pipeSegmentCount;

	public float ringDistance;

	public float minCurveRadius, maxCurveRadius;
	public int minCurveSegmentCount, maxCurveSegmentCount;
	Mesh mesh;
	Vector3[] vertices;
	Vector2[] uv;
	int[] triangles;
	public RandomPlacer notePlacer;

	private void Awake()
	{
		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.name = "Pipe";
	}


	public void GeneratePipe()
	{
		CurveRadius = Random.Range(minCurveRadius, maxCurveRadius);
		CurveSegmentCount = Random.Range(minCurveSegmentCount, maxCurveSegmentCount + 1);
		mesh.Clear();
		SetVertices();
		SetUV();
		SetTriangles();
		mesh.RecalculateNormals();
	}

	public float GenerateMusicNotes(List<(float, float)> songMusicNotes, float timeWhenPipeEntered, bool isFirstPipe = false)
	{
		PlayerMovement player = GameObject.FindGameObjectWithTag("Player").GetComponent(typeof(PlayerMovement)) as PlayerMovement;

		// How long will it take to pass this pipe
		float arcLength = CurveAngle / 360 * (2f * Mathf.PI * CurveRadius);
		TimeToPassPipe = arcLength / FindObjectOfType<GameSettings>().GetDifficultyValue(GameSettings.DifficultyType.Speed);

		// Since we start the game at the second pipe, do not generate notes for the first one
		if (!isFirstPipe)
		{
			List<(float, float, float)> notesForThisPipe = new List<(float, float, float)> { };
			for (int i = 0; i < songMusicNotes.Count; i++)
			{
				float noteAppearTime = songMusicNotes[i].Item1;
				if (noteAppearTime > timeWhenPipeEntered && noteAppearTime < timeWhenPipeEntered + TimeToPassPipe)
				{
					noteAppearTime -= timeWhenPipeEntered;

					float noteAppearSegment = CrossSceneData.Remap(noteAppearTime, 0, TimeToPassPipe, 0, CurveAngle);
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
		//Debug.Log("When entered: " + timeWhenPipeEntered + " To Pass: " + timeToPassPipe);
		return timeWhenPipeEntered + TimeToPassPipe;
	}

	private void SetVertices()
	{
		vertices = new Vector3[pipeSegmentCount * CurveSegmentCount * 4];

		float uStep = ringDistance / CurveRadius;
		CurveAngle = uStep * CurveSegmentCount * (360f / (2f * Mathf.PI));
		CreateFirstQuadRing(uStep);
		int iDelta = pipeSegmentCount * 4;
		for (int u = 2, i = iDelta; u <= CurveSegmentCount; u++, i += iDelta)
		{
			CreateQuadRing(u * uStep, i);
		}
		mesh.vertices = vertices;
	}

	private void CreateFirstQuadRing(float u)
	{
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

	private void CreateQuadRing(float u, int i)
	{
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

	private void SetUV()
	{
		uv = new Vector2[vertices.Length];
		for (int i = 0; i < vertices.Length; i += 4)
		{
			uv[i] = Vector2.zero;
			uv[i + 1] = Vector2.right;
			uv[i + 2] = Vector2.up;
			uv[i + 3] = Vector2.one;
		}
		mesh.uv = uv;
	}

	private void SetTriangles()
	{
		triangles = new int[pipeSegmentCount * CurveSegmentCount * 6];
		for (int t = 0, i = 0; t < triangles.Length; t += 6, i += 4)
		{
			triangles[t] = i;
			triangles[t + 1] = triangles[t + 4] = i + 2;
			triangles[t + 2] = triangles[t + 3] = i + 1;
			triangles[t + 5] = i + 3;
		}
		mesh.triangles = triangles;
	}

	private Vector3 GetPointOnTorus(float u, float v)
	{
		Vector3 p;
		float r = (CurveRadius + pipeRadius * Mathf.Cos(v));
		p.x = r * Mathf.Sin(u);
		p.y = r * Mathf.Cos(u);
		p.z = pipeRadius * Mathf.Sin(v);
		return p;
	}

	public float AlignWith(Pipe pipe, float worldAbsoluteRotation)
	{
		// Random rotation for a new pipe restricted by segment count to allign perfectly
		RelativeRotation = Random.Range(0, CurveSegmentCount) * 360f / pipeSegmentCount;

		// Futher restrictions to limit rotation in <-180, 180)
		if (RelativeRotation > 360)
		{
			int tmp = (int)(RelativeRotation / 360f);
			tmp *= 360;
			RelativeRotation -= tmp;
		}
		if (RelativeRotation >= 180)
			RelativeRotation -= 360;

		worldAbsoluteRotation += RelativeRotation;
		if (worldAbsoluteRotation < -180)
			worldAbsoluteRotation += 360;
		else if (worldAbsoluteRotation >= 180)
			worldAbsoluteRotation -= 360;

		transform.SetParent(pipe.transform, false);
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.Euler(0f, 0f, -pipe.CurveAngle);

		transform.Translate(0f, pipe.CurveRadius, 0f);
		transform.Rotate(RelativeRotation, 0f, 0f);
		transform.Translate(0f, -CurveRadius, 0f);

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

	public int PipeSegmentCount { get { return pipeSegmentCount; } }
	public int CurveSegmentCount { get; private set; }
	public float CurveAngle { get; private set; }
	public float CurveRadius { get; private set; }
	public float RelativeRotation { get; private set; }

	public float TimeToPassPipe { get; private set; } = 0;

}