using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draw_JYJ : MonoBehaviour
{
    // Chalks_Prefeb > Chalk, Eraser에 적용
    // 컴퍼넌트로 추가하면 칠판에 그림을 그리고 지우는 기능

    SoundChange_JYJ soundClip;

    // 분필인지 지우개인지 구별
    [SerializeField] int value; 

    // JYJ_Prefeb 폴더에 Line
    [SerializeField] GameObject linePrefeb;

    // Hierarchy창의 Lines(빈 오브젝트), 생성되는 Line들의 parent
    [SerializeField] GameObject lines;

    // 그림이 그려질 칠판
    [SerializeField] GameObject board;

    // 라인의 한 점만 지우는 방법이 없어서 지우는 대신 뒤로 이동시킴
    [SerializeField] Transform eraserPos;

    List<Vector3> points;
    LineRenderer lR;
    Rigidbody rig;

    RaycastHit hit;

    float maxDistance;
    float maxDrawDistance;
    float timer;
    bool isDraw;


    void Awake()
    {
        rig = GetComponent<Rigidbody>();
        soundClip = GetComponent<SoundChange_JYJ>();
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        if (rig.isKinematic)
        {
            // 잡은 물건이 분필이면 그림 그려지게
            if (value == 1)
            {               
                Draw();
            }

            // 잡은 물건이 지우개면 지워지게
            else if (value == 2)
            {
                DrawClear();
            }
        }
    }

    // 변수 초기화
    void Init()
    {
        points = new List<Vector3>();
        maxDistance = 10f;
        maxDrawDistance = 0.25f;
        isDraw = false;
        timer = 0f;
    }

    void Draw()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance))
        {
            if (hit.transform.gameObject == board)
            {
                // 분필이 칠판에 처음 닿았을 때
                if (hit.distance <= maxDrawDistance && isDraw == false)
                {
                    GameObject go = Instantiate(linePrefeb);
                    go.transform.SetParent(lines.transform);
                    lR = go.GetComponent<LineRenderer>();

                    points.Add(hit.point);

                    lR.positionCount = 1; // 선의 정점 수!
                    lR.SetPosition(0, points[0]); //선의 정점의 위치를 지정

                    if(SoundSystem_JYJ.source.clip == soundClip.clip)
                    {
                        SoundSystem_JYJ.source.Play();
                    }
                    isDraw = true;
                }

                // 닿고 있는 중
                else if (hit.distance <= maxDrawDistance && isDraw == true)
                {
                    Vector3 pos = hit.point;
                    if (Vector3.Distance(points[points.Count - 1], pos) > 0.1f) // 0.1f 이상 움직이면 그려짐
                    {
                        points.Add(pos);
                        lR.positionCount++;
                        lR.SetPosition(lR.positionCount - 1, pos);
                        
                        // 닿은 채로 멈췄다가 움직이면 다시 소리남
                        if (!SoundSystem_JYJ.source.isPlaying && SoundSystem_JYJ.source.clip == soundClip.clip)
                        {
                            SoundSystem_JYJ.source.UnPause();
                        }
                        timer = 0f;
                    }
                    else // 칠판에 닿았지만 분필이 움직이지 않고 있으면
                    {
                        // 0.2초가 지나도 안움직이면 소리 멈춤
                        timer += Time.deltaTime;

                        if (timer > 0.2f && SoundSystem_JYJ.source.clip == soundClip.clip && SoundSystem_JYJ.source.isPlaying)
                        {
                            SoundSystem_JYJ.source.Pause();
                            timer = 0f;
                        }
                        return;
                    }
                }

                // 떨어졌을 때
                else if (hit.distance > maxDrawDistance && isDraw == true) 
                {
                    if(SoundSystem_JYJ.source.clip == soundClip.clip)
                    {
                        SoundSystem_JYJ.source.Stop();
                    }
                    points.Clear();
                    isDraw = false;
                }
                else return;
            }

            // 다른 물체에 닿았을 때
            else if (hit.transform.gameObject != board && isDraw == true)
            {
                if (SoundSystem_JYJ.source.clip == soundClip.clip && SoundSystem_JYJ.source.isPlaying)
                {
                    SoundSystem_JYJ.source.Stop();
                }
                points.Clear();
                isDraw = false;
            }
        }
    }

    void DrawClear()
    {
        if (Physics.Raycast(transform.position, -transform.forward, out hit, maxDistance))
        {
            // raycast 닿은 물체가 칠판이면
            if (hit.transform.gameObject == board)
            {
                // 생성된 모든 라인에 지우는 효과 넣기 위해서 for문 이용
                for (int j = 0; j < lines.transform.childCount; j++)
                {
                    lR = lines.transform.GetChild(j).GetComponent<LineRenderer>();

                    // 라인 안의 각 정점들 중 지우개 범위 안에 있는 정점 찾기 위해서 이중 for문 이용
                    for (int i = 0; i < lR.positionCount; i++)
                    {
                        if (Vector3.Distance(lR.GetPosition(i), hit.point) < 0.3f)
                        {
                            // LineRenderer에 중간 정점 값을 null로 지정하는 방법이 없으므로 정점의 위치를 이동해서 지우는 효과 적용
                            lR.SetPosition(i, eraserPos.position);
                        }
                    }
                }
            }
        }
    }
}
