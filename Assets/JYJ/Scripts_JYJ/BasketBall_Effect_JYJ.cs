using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketBall_Effect_JYJ : MonoBehaviour
{
	// Pool_Prefeb > Particle_Prefeb에 적용
	// 농구공 넣으면 가져올 풀링 오브젝트임
	// 농구 골대에 공을 넣으면 파티클 터지는 기능

	Pooling_JYJ pool;

	// ParticlePrefeb 속성 지정
	[SerializeField] Color[] colors;

	float moveSpeed;
	float minSize;
	float maxSize;
	float sizeSpeed;
	float maxDistance;

	Vector2 direction;
	Vector3 oripos;

	Material material;

    void Awake()
    {
		pool = FindObjectOfType<Pooling_JYJ>();
	}

	void Start()
	{
		Init();
		ParticleCreate();
	}

	void Update()
	{
		ParticleMove();
	}

	void Init()
	{
		moveSpeed = 0.1f;
		minSize = 0.2f;
		maxSize = 0.3f;
		sizeSpeed = 0.3f;
		maxDistance = 0.8f;
	}

	void ParticleMove()
    {
		// 랜덤 방향으로 퍼짐
		transform.Translate(direction * moveSpeed);

		// 멀어질 수록 크기 작아지게 
		transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime * sizeSpeed);

		// 멀어지면 프리펩 삭제
		if (Vector3.Distance(gameObject.transform.position, oripos) >= maxDistance)
		{
			ParticleCreate();
			pool.DestroyObj(0, gameObject);
		}
	}

	void ParticleCreate()
    {
		// 퍼지는 방향 랜덤하게 지정
		direction = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(0f, 1.0f), Random.Range(-1.0f, 1.0f));
		oripos = transform.position;

		// 크기 랜덤하게 생성
		float size = Random.Range(minSize, maxSize);
		transform.localScale = new Vector3(size, size, size);

		// 지정한 색 중 랜덤하게 생성
		material = GetComponent<MeshRenderer>().material;
		material.SetColor("_Color", colors[Random.Range(0, colors.Length)]);
	}
}
