using System.Collections;
using UnityEngine;
using DG.Tweening;

public class RolyPolyToy_JYJ : MonoBehaviour
{
    // RolyPolyToy_Prefeb > RoltPolyToy에 적용
    // 한번만 부딪히게 수정함

    SoundOneShot_JYJ soundClip;

    // 오뚝이 함수 속성
    IEnumerator coroutine;

    float duration;
    float strength;
    int vibrato;
    bool isHit;

    private void Awake()
    {
        soundClip = GetComponent<SoundOneShot_JYJ>();
    }

    void Start()
    {
        ToyInit();
    }

    // 속성 초기화
    void ToyInit()
    {
        coroutine = ToyRotate();
        duration = 3f;
        strength = 50f;
        vibrato = 3;
        isHit = false;
    }

    // 부딪힌 물체가 Controller이고, 이미 타격된 상태가 아니면 타격 가능
    void OnTriggerEnter(Collider collision)
    {
        if (!isHit && (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("CanTeleport")))
        {
            coroutine = ToyRotate();
            StartCoroutine(coroutine);
        }
    }

    // 흔들리고 duration이 지나면 제자리로 돌아옴. 동적으로 표현하기 위해서 코루틴
    IEnumerator ToyRotate()
    {
        // DoTween이용해서 흔들림 구현
        isHit = true;
        SoundSystem_JYJ.source.PlayOneShot(soundClip.clip);
        transform.DOShakeRotation(duration, strength, vibrato);

        yield return new WaitForSeconds(duration);

        // 제자리로 돌아옴. DoTween이용해서 자연스러운 효과 넣어줌
        transform.DORotate(Vector3.zero, duration, RotateMode.Fast).SetEase(Ease.OutElastic);
        yield return null;
        isHit = false;
    }
}


