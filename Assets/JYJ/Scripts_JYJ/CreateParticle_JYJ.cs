using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateParticle_JYJ : MonoBehaviour
{
	// Basketball_Prefeb > Particle_Trigger에 적용
	// 농구공을 골대에 넣으면 파티클 생기는 효과

	Pooling_JYJ pool;
	SoundOneShot_JYJ soundClip;

	[SerializeField] Transform ball;

	RaycastHit hit;

	float maxRay;
	int maxCreates;
	int creates;
	bool isHit;

	void Awake()
    {
		pool = FindObjectOfType<Pooling_JYJ>();
		soundClip = GetComponent<SoundOneShot_JYJ>();
	}

    void Start()
    {
		Init();
	}

    void Update()
    {
		WhetherGoal();
	}

	// 농구공이 패스 콜라이더에 위쪽에서 닿았을 경우 파티클 효과 호출
	void OnTriggerEnter(Collider other)
    {
		if(isHit && other.transform == ball)
        {
			SoundSystem_JYJ.source.PlayOneShot(soundClip.clip);
			StartCoroutine(ParticleCount());
			isHit = false;
		}
	}

	// 파티클 효과 함수, maxCreates으로 생성되는 ParticlePrefeb 개수 지정
	IEnumerator ParticleCount()
    {
        while (creates < maxCreates)
		{   
			// 풀링 오브젝트 Particle_Prefeb 가져옴
			GameObject cube = pool.ActiveObj(0);
			cube.transform.position = transform.position;
			creates++;

			yield return new WaitForSeconds(0.01f);
			
		}
		creates = 0;
    }

	// 초기화
	void Init()
    {
		isHit = false;
		maxRay = 0.5f;
		maxCreates = 20;
		creates = 0;
	}

	// 위로 레이 쏴서 위에서 넣었는지 판별.
	void WhetherGoal()
    {
		if (Physics.Raycast(transform.position, Vector3.up, out hit, maxRay) && hit.transform != null)
		{
			isHit = true;
		}
	}
}
