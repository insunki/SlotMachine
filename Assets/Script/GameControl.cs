using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    public List<int> currenticon; // 슬롯 아이콘 현재값 (25개)
    public List<Sprite> slotIcon; // 슬롯 아이콘 이미지 (8개)
    public List<Icon> icon; // 슬롯아이콘 칸 (25개)
    public List<Resulte> Resultes; // 아이콘마다의 당첨금액
    public Slot[] slot; // 슬롯 줄(5줄)
    public Text BetMoney; // 배팅금액 텍스트
    public Text ResulteMoney; // 결과창 텍스트
    public Text PlayerMoney; // 플레이어 텍스트
    public float Betting = 100; // 기본배팅 금액
    public float HouseMoney = 0; // 게임시작 플레이어 보유 머니
    private float MaxBetting = 100000; // 배팅금액 한도
    private float ResultePlayerMoney; // 슬롯 플레이 결과 플레이어 돈
    public ResulteLine[] ResulteLine; // 라인결과
    private int LineNum; // 라인결과 저장용도
    public GameObject[] Effect; // 이펙트
    public AudioClip[] clip; // 오디오
    private Bonus bonus;
    public Text BonusResulteMoney; // 보너스상황일때 결과창 텍스트
    public Button AutoModeBtn; // 오토 버튼
    public Button StartBtn; // 스타트 버튼

    public static event Action SpinStart; // 이벤트
    public bool isCheckResult = false; // 당첨결과
    public bool isAutoMode = false; // 자동모드
    public bool isEffect = false; // 이펙트 로딩중
    public bool isBonus = false; // 보너스모드
    void Awake()
    {
        bonus = FindObjectOfType<Bonus>();
        AutoModeBtn = GameObject.Find("AutoBtn").GetComponent<Button>();
        StartBtn = GameObject.Find("StartBtn").GetComponent<Button>();
    }
    void Update()
    {
        if (HouseMoney <= 0)
        {
            PlayerMoney.text = 0.ToString();
        }
        else
        {
            PlayerMoney.text = CommaText(HouseMoney).ToString();
        }
        BetMoney.text = CommaText(Betting).ToString();
        if (!slot[0].isSlotsStop || !slot[1].isSlotsStop || !slot[2].isSlotsStop || !slot[3].isSlotsStop || !slot[4].isSlotsStop)
        {
            isCheckResult = false;
        }
        if (slot[0].isSlotsStop && slot[1].isSlotsStop && slot[2].isSlotsStop && slot[3].isSlotsStop && slot[4].isSlotsStop && !isCheckResult)
        {
            CheckResults();
        }
    }

    int GetRandom()
    {
        return UnityEngine.Random.Range(1, slotIcon.Count);
    }
    public void IconRandom() // 아이콘 랜덤
    {
        for (int i = 0; i < currenticon.Count; i++)
        {
            int cnt = GetRandom();
            currenticon[i] = cnt;
            icon[i].number = cnt;
        }
    }
    public void Setting() // 아이콘 랜덤 셋팅
    {
        for (int i = 0; i < icon.Count; i++)
        {
            icon[i].spriteRenderer.sprite = slotIcon[currenticon[i]];
        }
    }
    public void Spinslot() // Start 버튼
    {
        isAutoMode = false;
        if ((slot[0].isSlotsStop && slot[1].isSlotsStop && slot[2].isSlotsStop && slot[3].isSlotsStop && slot[4].isSlotsStop) && (Betting <= HouseMoney))
        {
            StartCoroutine(StartSpin());
            HouseMoney -= Betting;
            PlayerMoney.text = CommaText(HouseMoney).ToString();
        }
        if (AutoModeBtn.interactable == false)
        {
            AutoModeBtn.interactable = true;
        }
    }
    IEnumerator StartSpin()
    {
        if (!isEffect)
        {
            Sound.instance.EffectSound("spin", clip[0]);
            SpinStart();
            yield return new WaitForSeconds(3f);
        }
    }

    public void AutoBtn() // Auto 버튼
    {
        if (isBonus)
        {
            isAutoMode = false;
            StartCoroutine(BonusAutoSpin());
        }
        else
        {
            isAutoMode = true;
            StartCoroutine(AutoSpinMode());
        }
    }
    IEnumerator AutoSpinMode()
    {
        if (isAutoMode && Betting <= HouseMoney)
        {
            for (int i = 0; i < 10; i++)
            {
                AutoModeBtn.interactable = false;
                if ((slot[0].isSlotsStop && slot[1].isSlotsStop && slot[2].isSlotsStop && slot[3].isSlotsStop && slot[4].isSlotsStop) && (Betting <= HouseMoney))
                {
                    StartCoroutine(StartSpin());
                    HouseMoney -= Betting;
                    PlayerMoney.text = CommaText(HouseMoney).ToString();
                }
                while (!isEffect)
                {
                    if (slot[0].isSlotsStop && slot[1].isSlotsStop && slot[2].isSlotsStop && slot[3].isSlotsStop && slot[4].isSlotsStop && !isCheckResult)
                    {
                        CheckResults();
                    }
                    yield return new WaitForSeconds(1f);
                    break;
                }
                yield return new WaitForSeconds(3f);
                if (!isAutoMode || Betting > HouseMoney)
                    break;
            }
            isAutoMode = false;
            AutoModeBtn.interactable = true;
        }
    }
    IEnumerator BonusAutoSpin()
    {
        for (int i = 0; i < bonus.Count; i++)
        {
            AutoModeBtn.interactable = false;
            StartBtn.interactable = false;
            if ((slot[0].isSlotsStop && slot[1].isSlotsStop && slot[2].isSlotsStop && slot[3].isSlotsStop && slot[4].isSlotsStop) && (Betting <= HouseMoney))
            {
                StartCoroutine(StartSpin());
                bonus.num--;
                bonus.BonusCount.text = CommaText(bonus.num).ToString();
                if(bonus.num == 0)
                {
                    bonus.BonusCount.text = 0.ToString();
                }
                
            }
            while (!isEffect)
            {
                if (slot[0].isSlotsStop && slot[1].isSlotsStop && slot[2].isSlotsStop && slot[3].isSlotsStop && slot[4].isSlotsStop && !isCheckResult)
                {
                    CheckResults();
                }
                yield return new WaitForSeconds(1f);
                break;
            }
            yield return new WaitForSeconds(3f);
        }
        AutoModeBtn.interactable = true;
        StartBtn.interactable = true;
        isBonus = false;
        bonus.BonusCountUI.SetActive(false);
        bonus.BonusResulteMoney.SetActive(false);
        bonus.Logo.SetActive(true);
        bonus.BonusLogo.SetActive(false);
    }

    public void CheckResults() // 슬롯 당첨 정보
    {

        if (icon[0].number == icon[4].number ? icon[4].number == icon[8].number : false)
        {
            if (icon[0].number == icon[12].number)
            {
                if (icon[0].number == icon[16].number) // 5개
                {
                    for (int i = 0; i < Resultes.Count; i++)
                    {
                        if (Resultes[i].number == icon[0].number)
                        {
                            if (icon[0].number == 2)
                            {
                                ResultePlayerMoney = Resultes[2].ResulteBet * Betting * 0.7f;
                                StartCoroutine(ResulteMoneyDelay());
                                StartCoroutine(BonusEffect());
                                StartCoroutine(PlusHouseMoney());
                                StartCoroutine(fadeResulteMoney());
                                LineNum = 4;
                                StartCoroutine(Resulte5LineDelay());
                            }
                            else
                            {
                                if (isBonus)
                                {
                                    ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.7f * bonus.multiply;
                                }
                                else
                                {
                                    ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.7f;
                                }
                                StartCoroutine(ResulteMoneyDelay());
                                StartCoroutine(fiveWin());
                                StartCoroutine(PlusHouseMoney());
                                StartCoroutine(fadeResulteMoney());
                                LineNum = 4;
                                StartCoroutine(Resulte5LineDelay());
                            }
                        }
                    }
                }
                else // 4개
                {
                    for (int i = 0; i < Resultes.Count; i++)
                    {
                        if (Resultes[i].number == icon[0].number)
                        {
                            if (icon[0].number == 2)
                            {
                                ResultePlayerMoney = Resultes[2].ResulteBet * Betting * 0.3f;
                                StartCoroutine(ResulteMoneyDelay());
                                StartCoroutine(BonusEffect());
                                StartCoroutine(PlusHouseMoney());
                                StartCoroutine(fadeResulteMoney());
                                LineNum = 4;
                                StartCoroutine(Resulte4LineDelay());
                            }
                            else
                            {
                                if (isBonus)
                                {
                                    ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.3f * bonus.multiply;
                                }
                                else
                                {
                                    ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.3f;
                                }
                                StartCoroutine(ResulteMoneyDelay());
                                StartCoroutine(fourWin());
                                StartCoroutine(PlusHouseMoney());
                                StartCoroutine(fadeResulteMoney());
                                LineNum = 4;
                                StartCoroutine(Resulte4LineDelay());
                            }
                        }
                    }
                }
            }
            else // 3개
            {
                for (int i = 0; i < Resultes.Count; i++)
                {
                    if (Resultes[i].number == icon[0].number)
                    {
                        if (icon[0].number == 2)
                        {
                            ResultePlayerMoney = Resultes[2].ResulteBet * Betting * 0.1f;
                            StartCoroutine(ResulteMoneyDelay());
                            StartCoroutine(BonusEffect());
                            StartCoroutine(PlusHouseMoney());
                            StartCoroutine(fadeResulteMoney());
                            LineNum = 4;
                            StartCoroutine(Resulte3LineDelay());
                        }
                        else
                        {
                            if (isBonus)
                            {
                                ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.1f * bonus.multiply;
                            }
                            else
                            {
                                ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.1f;
                            }
                            StartCoroutine(ResulteMoneyDelay());
                            StartCoroutine(ThreeWin());
                            StartCoroutine(PlusHouseMoney());
                            StartCoroutine(fadeResulteMoney());
                            LineNum = 4;
                            StartCoroutine(Resulte3LineDelay());
                        }
                    }
                }
            }
        } //  5번째
        else if (icon[1].number == icon[5].number ? icon[5].number == icon[9].number : false)
        {
            if (icon[1].number == icon[13].number)
            {
                if (icon[1].number == icon[17].number)
                {
                    for (int i = 0; i < Resultes.Count; i++)
                    {
                        if (Resultes[i].number == icon[1].number) // 5개
                        {
                            if (icon[1].number == 2)
                            {
                                ResultePlayerMoney = Resultes[2].ResulteBet * Betting * 0.7f;
                                StartCoroutine(ResulteMoneyDelay());
                                StartCoroutine(BonusEffect());
                                StartCoroutine(PlusHouseMoney());
                                StartCoroutine(fadeResulteMoney());
                                LineNum = 5;
                                StartCoroutine(Resulte5LineDelay());
                            }
                            else
                            {
                                if (isBonus)
                                {
                                    ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.7f * bonus.multiply;
                                }
                                else
                                {
                                    ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.7f;
                                }
                                StartCoroutine(ResulteMoneyDelay());
                                StartCoroutine(fiveWin());
                                StartCoroutine(PlusHouseMoney());
                                StartCoroutine(fadeResulteMoney());
                                LineNum = 5;
                                StartCoroutine(Resulte5LineDelay());
                            }
                        }
                    }
                }
                else // 4개
                {
                    for (int i = 0; i < Resultes.Count; i++)
                    {
                        if (Resultes[i].number == icon[1].number)
                        {
                            if (icon[1].number == 2)
                            {
                                ResultePlayerMoney = Resultes[2].ResulteBet * Betting * 0.3f;
                                StartCoroutine(ResulteMoneyDelay());
                                StartCoroutine(BonusEffect());
                                StartCoroutine(PlusHouseMoney());
                                StartCoroutine(fadeResulteMoney());
                                LineNum = 5;
                                StartCoroutine(Resulte4LineDelay());
                            }
                            else
                            {
                                if (isBonus)
                                {
                                    ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.3f * bonus.multiply;
                                }
                                else
                                {
                                    ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.3f;
                                }
                                StartCoroutine(ResulteMoneyDelay());
                                StartCoroutine(fourWin());
                                StartCoroutine(PlusHouseMoney());
                                StartCoroutine(fadeResulteMoney());
                                LineNum = 5;
                                StartCoroutine(Resulte4LineDelay());
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < Resultes.Count; i++)
                {
                    if (Resultes[i].number == icon[1].number)
                    {
                        if (icon[1].number == 2)
                        {
                            ResultePlayerMoney = Resultes[2].ResulteBet * Betting * 0.1f;
                            StartCoroutine(ResulteMoneyDelay());
                            StartCoroutine(BonusEffect());
                            StartCoroutine(PlusHouseMoney());
                            StartCoroutine(fadeResulteMoney());
                            LineNum = 5;
                            StartCoroutine(Resulte3LineDelay());
                        }
                        else
                        {
                            if (isBonus)
                            {
                                ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.1f * bonus.multiply;
                            }
                            else
                            {
                                ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.1f;
                            }
                            StartCoroutine(ResulteMoneyDelay());
                            StartCoroutine(ThreeWin());
                            StartCoroutine(PlusHouseMoney());
                            StartCoroutine(fadeResulteMoney());
                            LineNum = 5;
                            StartCoroutine(Resulte3LineDelay());
                        }
                    }
                }
            }
        } //  6번째
        else if (icon[2].number == icon[6].number ? icon[6].number == icon[10].number : false)
        {
            if (icon[2].number == icon[14].number)
            {
                if (icon[2].number == icon[18].number)
                {
                    for (int i = 0; i < Resultes.Count; i++)
                    {
                        if (Resultes[i].number == icon[2].number)
                        {
                            if (icon[2].number == 2)
                            {
                                ResultePlayerMoney = Resultes[2].ResulteBet * Betting * 0.7f;
                                StartCoroutine(ResulteMoneyDelay());
                                StartCoroutine(BonusEffect());
                                StartCoroutine(PlusHouseMoney());
                                StartCoroutine(fadeResulteMoney());
                                LineNum = 6;
                                StartCoroutine(Resulte5LineDelay());
                            }
                            else
                            {
                                if (isBonus)
                                {
                                    ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.7f * bonus.multiply;
                                }
                                else
                                {
                                    ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.7f;
                                }
                                StartCoroutine(ResulteMoneyDelay());
                                StartCoroutine(fiveWin());
                                StartCoroutine(PlusHouseMoney());
                                StartCoroutine(fadeResulteMoney());
                                LineNum = 6;
                                StartCoroutine(Resulte5LineDelay());
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < Resultes.Count; i++)
                    {
                        if (Resultes[i].number == icon[2].number)
                        {
                            if (icon[2].number == 2)
                            {
                                ResultePlayerMoney = Resultes[2].ResulteBet * Betting * 0.3f;
                                StartCoroutine(ResulteMoneyDelay());
                                StartCoroutine(BonusEffect());
                                StartCoroutine(PlusHouseMoney());
                                StartCoroutine(fadeResulteMoney());
                                LineNum = 6;
                                StartCoroutine(Resulte4LineDelay());
                            }
                            else
                            {
                                if (isBonus)
                                {
                                    ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.3f * bonus.multiply;
                                }
                                else
                                {
                                    ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.3f;
                                }
                                StartCoroutine(ResulteMoneyDelay());
                                StartCoroutine(fourWin());
                                StartCoroutine(PlusHouseMoney());
                                StartCoroutine(fadeResulteMoney());
                                LineNum = 6;
                                StartCoroutine(Resulte4LineDelay());
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < Resultes.Count; i++)
                {
                    if (Resultes[i].number == icon[2].number)
                    {
                        if (icon[2].number == 2)
                        {
                            ResultePlayerMoney = Resultes[2].ResulteBet * Betting * 0.1f;
                            StartCoroutine(ResulteMoneyDelay());
                            StartCoroutine(BonusEffect());
                            StartCoroutine(PlusHouseMoney());
                            StartCoroutine(fadeResulteMoney());
                            LineNum = 6;
                            StartCoroutine(Resulte3LineDelay());
                        }
                        else
                        {
                            if (isBonus)
                            {
                                ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.1f * bonus.multiply;
                            }
                            else
                            {
                                ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.1f;
                            }
                            StartCoroutine(ResulteMoneyDelay());
                            StartCoroutine(ThreeWin());
                            StartCoroutine(PlusHouseMoney());
                            StartCoroutine(fadeResulteMoney());
                            LineNum = 6;
                            StartCoroutine(Resulte3LineDelay());
                        }
                    }
                }
            }
        } //  7번째
        else if (icon[2].number == icon[6].number ? icon[6].number == icon[9].number : false)
        {
            if (icon[2].number == icon[14].number)
            {
                if (icon[2].number == icon[18].number)
                {
                    for (int i = 0; i < Resultes.Count; i++)
                    {
                        if (Resultes[i].number == icon[2].number)
                        {
                            if (icon[2].number == 2)
                            {
                                ResultePlayerMoney = Resultes[2].ResulteBet * Betting * 0.7f;
                                StartCoroutine(ResulteMoneyDelay());
                                StartCoroutine(BonusEffect());
                                StartCoroutine(PlusHouseMoney());
                                StartCoroutine(fadeResulteMoney());
                                LineNum = 0;
                                StartCoroutine(Resulte5LineDelay());
                            }
                            else
                            {
                                if (isBonus)
                                {
                                    ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.7f * bonus.multiply;
                                }
                                else
                                {
                                    ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.7f;
                                }
                                StartCoroutine(ResulteMoneyDelay());
                                StartCoroutine(fiveWin());
                                StartCoroutine(PlusHouseMoney());
                                StartCoroutine(fadeResulteMoney());
                                LineNum = 0;
                                StartCoroutine(Resulte5LineDelay());
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < Resultes.Count; i++)
                    {
                        if (Resultes[i].number == icon[2].number)
                        {
                            if (icon[2].number == 2)
                            {
                                ResultePlayerMoney = Resultes[2].ResulteBet * Betting * 0.3f;
                                StartCoroutine(ResulteMoneyDelay());
                                StartCoroutine(BonusEffect());
                                StartCoroutine(PlusHouseMoney());
                                StartCoroutine(fadeResulteMoney());
                                LineNum = 0;
                                StartCoroutine(Resulte4LineDelay());
                            }
                            else
                            {
                                if (isBonus)
                                {
                                    ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.3f * bonus.multiply;
                                }
                                else
                                {
                                    ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.3f;
                                }
                                StartCoroutine(ResulteMoneyDelay());
                                StartCoroutine(fourWin());
                                StartCoroutine(PlusHouseMoney());
                                StartCoroutine(fadeResulteMoney());
                                LineNum = 0;
                                StartCoroutine(Resulte4LineDelay());
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < Resultes.Count; i++)
                {
                    if (Resultes[i].number == icon[2].number)
                    {
                        if (icon[2].number == 2)
                        {
                            ResultePlayerMoney = Resultes[2].ResulteBet * Betting * 0.1f;
                            StartCoroutine(ResulteMoneyDelay());
                            StartCoroutine(BonusEffect());
                            StartCoroutine(PlusHouseMoney());
                            StartCoroutine(fadeResulteMoney());
                            LineNum = 0;
                            StartCoroutine(Resulte3LineDelay());
                        }
                        else
                        {
                            if (isBonus)
                            {
                                ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.1f * bonus.multiply;
                            }
                            else
                            {
                                ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.1f;
                            }
                            StartCoroutine(ResulteMoneyDelay());
                            StartCoroutine(ThreeWin());
                            StartCoroutine(PlusHouseMoney());
                            StartCoroutine(fadeResulteMoney());
                            LineNum = 0;
                            StartCoroutine(Resulte3LineDelay());
                        }
                    }
                }
            }
        } //  1번째
        else if (icon[0].number == icon[4].number ? icon[4].number == icon[9].number : false)
        {
            if (icon[0].number == icon[12].number)
            {
                if (icon[0].number == icon[16].number)
                {
                    for (int i = 0; i < Resultes.Count; i++)
                    {
                        if (Resultes[i].number == icon[0].number)
                        {
                            if (icon[0].number == 2)
                            {
                                ResultePlayerMoney = Resultes[2].ResulteBet * Betting * 0.7f;
                                StartCoroutine(ResulteMoneyDelay());
                                StartCoroutine(BonusEffect());
                                StartCoroutine(PlusHouseMoney());
                                StartCoroutine(fadeResulteMoney());
                                LineNum = 1;
                                StartCoroutine(Resulte5LineDelay());
                            }
                            else
                            {
                                if (isBonus)
                                {
                                    ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.7f * bonus.multiply;
                                }
                                else
                                {
                                    ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.7f;
                                }
                                StartCoroutine(ResulteMoneyDelay());
                                StartCoroutine(fiveWin());
                                StartCoroutine(PlusHouseMoney());
                                StartCoroutine(fadeResulteMoney());
                                LineNum = 1;
                                StartCoroutine(Resulte5LineDelay());
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < Resultes.Count; i++)
                    {
                        if (Resultes[i].number == icon[0].number)
                        {
                            if (icon[0].number == 2)
                            {
                                ResultePlayerMoney = Resultes[2].ResulteBet * Betting * 0.3f;
                                StartCoroutine(ResulteMoneyDelay());
                                StartCoroutine(BonusEffect());
                                StartCoroutine(PlusHouseMoney());
                                StartCoroutine(fadeResulteMoney());
                                LineNum = 1;
                                StartCoroutine(Resulte4LineDelay());
                            }
                            else
                            {
                                if (isBonus)
                                {
                                    ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.3f * bonus.multiply;
                                }
                                else
                                {
                                    ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.3f;
                                }
                                StartCoroutine(ResulteMoneyDelay());
                                StartCoroutine(fourWin());
                                StartCoroutine(PlusHouseMoney());
                                StartCoroutine(fadeResulteMoney());
                                LineNum = 1;
                                StartCoroutine(Resulte4LineDelay());
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < Resultes.Count; i++)
                {
                    if (Resultes[i].number == icon[0].number)
                    {
                        if (icon[0].number == 2)
                        {
                            ResultePlayerMoney = Resultes[2].ResulteBet * Betting * 0.1f;
                            StartCoroutine(ResulteMoneyDelay());
                            StartCoroutine(BonusEffect());
                            StartCoroutine(PlusHouseMoney());
                            StartCoroutine(fadeResulteMoney());
                            LineNum = 1;
                            StartCoroutine(Resulte3LineDelay());
                        }
                        else
                        {
                            if (isBonus)
                            {
                                ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.1f * bonus.multiply;
                            }
                            else
                            {
                                ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.1f;
                            }
                            StartCoroutine(ResulteMoneyDelay());
                            StartCoroutine(ThreeWin());
                            StartCoroutine(PlusHouseMoney());
                            StartCoroutine(fadeResulteMoney());
                            LineNum = 1;
                            StartCoroutine(Resulte3LineDelay());
                        }
                    }
                }
            }
        } //  2번째
        else if (icon[1].number == icon[6].number ? icon[6].number == icon[10].number : false)
        {
            if (icon[1].number == icon[14].number)
            {
                if (icon[1].number == icon[17].number)
                {
                    for (int i = 0; i < Resultes.Count; i++)
                    {
                        if (Resultes[i].number == icon[1].number)
                        {
                            if (icon[1].number == 2)
                            {
                                ResultePlayerMoney = Resultes[2].ResulteBet * Betting * 0.7f;
                                StartCoroutine(ResulteMoneyDelay());
                                StartCoroutine(BonusEffect());
                                StartCoroutine(PlusHouseMoney());
                                StartCoroutine(fadeResulteMoney());
                                LineNum = 2;
                                StartCoroutine(Resulte5LineDelay());
                            }
                            else
                            {
                                if (isBonus)
                                {
                                    ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.7f * bonus.multiply;
                                }
                                else
                                {
                                    ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.7f;
                                }
                                StartCoroutine(ResulteMoneyDelay());
                                StartCoroutine(fiveWin());
                                StartCoroutine(PlusHouseMoney());
                                StartCoroutine(fadeResulteMoney());
                                LineNum = 2;
                                StartCoroutine(Resulte5LineDelay());
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < Resultes.Count; i++)
                    {
                        if (Resultes[i].number == icon[1].number)
                        {
                            if (icon[1].number == 2)
                            {
                                ResultePlayerMoney = Resultes[2].ResulteBet * Betting * 0.3f;
                                StartCoroutine(ResulteMoneyDelay());
                                StartCoroutine(BonusEffect());
                                StartCoroutine(PlusHouseMoney());
                                StartCoroutine(fadeResulteMoney());
                                LineNum = 2;
                                StartCoroutine(Resulte4LineDelay());
                            }
                            else
                            {
                                if (isBonus)
                                {
                                    ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.3f * bonus.multiply;
                                }
                                else
                                {
                                    ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.3f;
                                }
                                StartCoroutine(ResulteMoneyDelay());
                                StartCoroutine(fourWin());
                                StartCoroutine(PlusHouseMoney());
                                StartCoroutine(fadeResulteMoney());
                                LineNum = 2;
                                StartCoroutine(Resulte4LineDelay());
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < Resultes.Count; i++)
                {
                    if (Resultes[i].number == icon[1].number)
                    {
                        if (icon[1].number == 2)
                        {
                            ResultePlayerMoney = Resultes[2].ResulteBet * Betting * 0.1f;
                            StartCoroutine(ResulteMoneyDelay());
                            StartCoroutine(BonusEffect());
                            StartCoroutine(PlusHouseMoney());
                            StartCoroutine(fadeResulteMoney());
                            LineNum = 2;
                            StartCoroutine(Resulte3LineDelay());
                        }
                        else
                        {
                            if (isBonus)
                            {
                                ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.1f * bonus.multiply;
                            }
                            else
                            {
                                ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.1f;
                            }
                            StartCoroutine(ResulteMoneyDelay());
                            StartCoroutine(ThreeWin());
                            StartCoroutine(PlusHouseMoney());
                            StartCoroutine(fadeResulteMoney());
                            LineNum = 2;
                            StartCoroutine(Resulte3LineDelay());
                        }
                    }
                }
            }
        } //  3번째
        else if (icon[1].number == icon[4].number ? icon[4].number == icon[8].number : false)
        {
            if (icon[1].number == icon[12].number)
            {
                if (icon[1].number == icon[17].number)
                {
                    for (int i = 0; i < Resultes.Count; i++)
                    {
                        if (Resultes[i].number == icon[1].number)
                        {
                            if (icon[1].number == 2)
                            {
                                ResultePlayerMoney = Resultes[2].ResulteBet * Betting * 0.7f;
                                StartCoroutine(ResulteMoneyDelay());
                                StartCoroutine(BonusEffect());
                                StartCoroutine(PlusHouseMoney());
                                StartCoroutine(fadeResulteMoney());
                                LineNum = 3;
                                StartCoroutine(Resulte5LineDelay());
                            }
                            else
                            {
                                if (isBonus)
                                {
                                    ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.7f * bonus.multiply;
                                }
                                else
                                {
                                    ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.7f;
                                }
                                StartCoroutine(ResulteMoneyDelay());
                                StartCoroutine(fiveWin());
                                StartCoroutine(PlusHouseMoney());
                                StartCoroutine(fadeResulteMoney());
                                LineNum = 3;
                                StartCoroutine(Resulte5LineDelay());
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < Resultes.Count; i++)
                    {
                        if (Resultes[i].number == icon[1].number)
                        {
                            if (icon[1].number == 2)
                            {
                                ResultePlayerMoney = Resultes[2].ResulteBet * Betting * 0.3f;
                                StartCoroutine(ResulteMoneyDelay());
                                StartCoroutine(BonusEffect());
                                StartCoroutine(PlusHouseMoney());
                                StartCoroutine(fadeResulteMoney());
                                LineNum = 3;
                                StartCoroutine(Resulte4LineDelay());
                            }
                            else
                            {
                                if (isBonus)
                                {
                                    ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.3f * bonus.multiply;
                                }
                                else
                                {
                                    ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.3f;
                                }
                                StartCoroutine(ResulteMoneyDelay());
                                StartCoroutine(fourWin());
                                StartCoroutine(PlusHouseMoney());
                                StartCoroutine(fadeResulteMoney());
                                LineNum = 3;
                                StartCoroutine(Resulte4LineDelay());
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < Resultes.Count; i++)
                {
                    if (Resultes[i].number == icon[1].number)
                    {
                        if (icon[1].number == 2)
                        {
                            ResultePlayerMoney = Resultes[2].ResulteBet * Betting * 0.1f;
                            StartCoroutine(ResulteMoneyDelay());
                            StartCoroutine(BonusEffect());
                            StartCoroutine(PlusHouseMoney());
                            StartCoroutine(fadeResulteMoney());
                            LineNum = 3;
                            StartCoroutine(Resulte3LineDelay());
                        }
                        else
                        {
                            if (isBonus)
                            {
                                ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.1f * bonus.multiply;
                            }
                            else
                            {
                                ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.1f;
                            }
                            StartCoroutine(ResulteMoneyDelay());
                            StartCoroutine(ThreeWin());
                            StartCoroutine(PlusHouseMoney());
                            StartCoroutine(fadeResulteMoney());
                            LineNum = 3;
                            StartCoroutine(Resulte3LineDelay());
                        }
                    }
                }
            }
        } // 4번째
        else if (icon[2].number == icon[5].number ? icon[5].number == icon[8].number : false)
        {
            if (icon[2].number == icon[13].number)
            {
                if (icon[2].number == icon[18].number)
                {
                    for (int i = 0; i < Resultes.Count; i++)
                    {
                        if (Resultes[i].number == icon[2].number)
                        {
                            if (icon[2].number == 2)
                            {
                                ResultePlayerMoney = Resultes[2].ResulteBet * Betting * 0.7f;
                                StartCoroutine(ResulteMoneyDelay());
                                StartCoroutine(BonusEffect());
                                StartCoroutine(PlusHouseMoney());
                                StartCoroutine(fadeResulteMoney());
                                LineNum = 7;
                                StartCoroutine(Resulte5LineDelay());
                            }
                            else
                            {
                                if (isBonus)
                                {
                                    ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.7f * bonus.multiply;
                                }
                                else
                                {
                                    ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.7f;
                                }
                                StartCoroutine(ResulteMoneyDelay());
                                StartCoroutine(fiveWin());
                                StartCoroutine(PlusHouseMoney());
                                StartCoroutine(fadeResulteMoney());
                                LineNum = 7;
                                StartCoroutine(Resulte5LineDelay());
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < Resultes.Count; i++)
                    {
                        if (Resultes[i].number == icon[2].number)
                        {
                            if (icon[2].number == 2)
                            {
                                ResultePlayerMoney = Resultes[2].ResulteBet * Betting * 0.3f;
                                StartCoroutine(ResulteMoneyDelay());
                                StartCoroutine(BonusEffect());
                                StartCoroutine(PlusHouseMoney());
                                StartCoroutine(fadeResulteMoney());
                                LineNum = 7;
                                StartCoroutine(Resulte4LineDelay());
                            }
                            else
                            {
                                if (isBonus)
                                {
                                    ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.3f * bonus.multiply;
                                }
                                else
                                {
                                    ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.3f;
                                }
                                StartCoroutine(ResulteMoneyDelay());
                                StartCoroutine(fourWin());
                                StartCoroutine(PlusHouseMoney());
                                StartCoroutine(fadeResulteMoney());
                                LineNum = 7;
                                StartCoroutine(Resulte4LineDelay());
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < Resultes.Count; i++)
                {
                    if (Resultes[i].number == icon[2].number)
                    {
                        if (icon[2].number == 2)
                        {
                            ResultePlayerMoney = Resultes[2].ResulteBet * Betting * 0.1f;
                            StartCoroutine(ResulteMoneyDelay());
                            StartCoroutine(BonusEffect());
                            StartCoroutine(PlusHouseMoney());
                            StartCoroutine(fadeResulteMoney());
                            LineNum = 7;
                            StartCoroutine(Resulte3LineDelay());
                        }
                        else
                        {
                            if (isBonus)
                            {
                                ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.1f * bonus.multiply;
                            }
                            else
                            {
                                ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.1f;
                            }
                            StartCoroutine(ResulteMoneyDelay());
                            StartCoroutine(ThreeWin());
                            StartCoroutine(PlusHouseMoney());
                            StartCoroutine(fadeResulteMoney());
                            LineNum = 7;
                            StartCoroutine(Resulte3LineDelay());
                        }
                    }
                }
            }
        }//  8번째
        else if (icon[0].number == icon[5].number ? icon[5].number == icon[10].number : false)
        {
            if (icon[0].number == icon[13].number)
            {
                if (icon[0].number == icon[16].number)
                {
                    for (int i = 0; i < Resultes.Count; i++)
                    {
                        if (Resultes[i].number == icon[0].number)
                        {
                            if (icon[0].number == 2)
                            {
                                ResultePlayerMoney = Resultes[2].ResulteBet * Betting * 0.7f;
                                StartCoroutine(ResulteMoneyDelay());
                                StartCoroutine(BonusEffect());
                                StartCoroutine(PlusHouseMoney());
                                StartCoroutine(fadeResulteMoney());
                                LineNum = 8;
                                StartCoroutine(Resulte5LineDelay());
                            }
                            else
                            {
                                if (isBonus)
                                {
                                    ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.7f * bonus.multiply;
                                }
                                else
                                {
                                    ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.7f;
                                }
                                StartCoroutine(ResulteMoneyDelay());
                                StartCoroutine(fiveWin());
                                StartCoroutine(PlusHouseMoney());
                                StartCoroutine(fadeResulteMoney());
                                LineNum = 8;
                                StartCoroutine(Resulte5LineDelay());
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < Resultes.Count; i++)
                    {
                        if (Resultes[i].number == icon[0].number)
                        {
                            if (icon[0].number == 2)
                            {
                                ResultePlayerMoney = Resultes[2].ResulteBet * Betting * 0.3f;
                                StartCoroutine(ResulteMoneyDelay());
                                StartCoroutine(BonusEffect());
                                StartCoroutine(PlusHouseMoney());
                                StartCoroutine(fadeResulteMoney());
                                LineNum = 8;
                                StartCoroutine(Resulte4LineDelay());
                            }
                            else
                            {
                                if (isBonus)
                                {
                                    ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.3f * bonus.multiply;
                                }
                                else
                                {
                                    ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.3f;
                                }
                                StartCoroutine(ResulteMoneyDelay());
                                StartCoroutine(fourWin());
                                StartCoroutine(PlusHouseMoney());
                                StartCoroutine(fadeResulteMoney());
                                LineNum = 8;
                                StartCoroutine(Resulte4LineDelay());
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < Resultes.Count; i++)
                {
                    if (Resultes[i].number == icon[0].number)
                    {
                        if (icon[0].number == 2)
                        {
                            ResultePlayerMoney = Resultes[2].ResulteBet * Betting * 0.1f;
                            StartCoroutine(ResulteMoneyDelay());
                            StartCoroutine(BonusEffect());
                            StartCoroutine(PlusHouseMoney());
                            StartCoroutine(fadeResulteMoney());
                            LineNum = 8;
                            StartCoroutine(Resulte3LineDelay());
                        }
                        else
                        {
                            if (isBonus)
                            {
                                ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.1f * bonus.multiply;
                            }
                            else
                            {
                                ResultePlayerMoney = Resultes[i].ResulteBet * Betting * 0.1f;
                            }
                            StartCoroutine(ResulteMoneyDelay());
                            StartCoroutine(ThreeWin());
                            StartCoroutine(PlusHouseMoney());
                            StartCoroutine(fadeResulteMoney());
                            LineNum = 8;
                            StartCoroutine(Resulte3LineDelay());
                        }
                    }
                }
            }
        }  //  9번째
        isCheckResult = true;
    }
    IEnumerator ResulteMoneyDelay() // 당첨금액화면 딜레이
    {
        yield return new WaitForSeconds(0.1f);
        ResulteMoney.text = CommaText(ResultePlayerMoney).ToString();
        BonusResulteMoney.text = CommaText(ResultePlayerMoney).ToString();
    }
    IEnumerator PlusHouseMoney() // 하우스머니 딜레이
    {
        yield return new WaitForSeconds(1.5f);
        Sound.instance.EffectSound("spin", clip[1]);
        StartCoroutine(CountHouseMoney(ResultePlayerMoney, 0));
    }
    IEnumerator CountHouseMoney(float target, float current)
    {
        float duration = 0.5f;
        float offset = (target - current) / duration;
        while (current < target)
        {
            current += offset * Time.deltaTime;
            PlayerMoney.text = ((int)current).ToString();
            yield return null;
        }
        HouseMoney += ResultePlayerMoney;
        current = target;
        PlayerMoney.text = ((int)current).ToString();
    }
    IEnumerator fadeResulteMoney() // 당첨금액화면 초기화
    {
        yield return new WaitForSeconds(1.5f);
        ResulteMoney.text = "";
        BonusResulteMoney.text = "";
    }

    IEnumerator Resulte3LineDelay() // 당첨 라인 딜레이
    {
        isEffect = true;
        if (isEffect)
        {
            for (int i = 0; i < ResulteLine.Length; i++)
            {
                if (ResulteLine[i].num == LineNum)
                {
                    ResulteLine[i].LineFrames[0].gameObject.SetActive(true);
                    ResulteLine[i].LineFrames[1].gameObject.SetActive(true);
                    ResulteLine[i].LineFrames[2].gameObject.SetActive(true);
                    ResulteLine[i].gameObject.SetActive(true);
                    yield return new WaitForSeconds(1.5f);
                    ResulteLine[i].LineFrames[0].gameObject.SetActive(false);
                    ResulteLine[i].LineFrames[1].gameObject.SetActive(false);
                    ResulteLine[i].LineFrames[2].gameObject.SetActive(false);
                    ResulteLine[i].gameObject.SetActive(false);
                }
            }
        }
        isEffect = false;
    }
    IEnumerator Resulte4LineDelay() // 당첨 라인 딜레이
    {
        isEffect = true;
        if (isEffect)
        {
            for (int i = 0; i < ResulteLine.Length; i++)
            {
                if (ResulteLine[i].num == LineNum)
                {
                    ResulteLine[i].LineFrames[0].gameObject.SetActive(true);
                    ResulteLine[i].LineFrames[1].gameObject.SetActive(true);
                    ResulteLine[i].LineFrames[2].gameObject.SetActive(true);
                    ResulteLine[i].LineFrames[3].gameObject.SetActive(true);
                    ResulteLine[i].gameObject.SetActive(true);
                    yield return new WaitForSeconds(1.5f);
                    ResulteLine[i].LineFrames[0].gameObject.SetActive(false);
                    ResulteLine[i].LineFrames[1].gameObject.SetActive(false);
                    ResulteLine[i].LineFrames[2].gameObject.SetActive(false);
                    ResulteLine[i].LineFrames[3].gameObject.SetActive(false);
                    ResulteLine[i].gameObject.SetActive(false);
                }
            }
        }
        isEffect = false;
    }
    IEnumerator Resulte5LineDelay() // 당첨 라인 딜레이
    {
        isEffect = true;
        if (isEffect)
        {
            for (int i = 0; i < ResulteLine.Length; i++)
            {
                if (ResulteLine[i].num == LineNum)
                {
                    ResulteLine[i].LineFrames[0].gameObject.SetActive(true);
                    ResulteLine[i].LineFrames[1].gameObject.SetActive(true);
                    ResulteLine[i].LineFrames[2].gameObject.SetActive(true);
                    ResulteLine[i].LineFrames[3].gameObject.SetActive(true);
                    ResulteLine[i].LineFrames[4].gameObject.SetActive(true);
                    ResulteLine[i].gameObject.SetActive(true);
                    yield return new WaitForSeconds(1.5f);
                    ResulteLine[i].LineFrames[0].gameObject.SetActive(false);
                    ResulteLine[i].LineFrames[1].gameObject.SetActive(false);
                    ResulteLine[i].LineFrames[2].gameObject.SetActive(false);
                    ResulteLine[i].LineFrames[3].gameObject.SetActive(false);
                    ResulteLine[i].LineFrames[4].gameObject.SetActive(false);
                    ResulteLine[i].gameObject.SetActive(false);
                }
            }
        }
        isEffect = false;
    }
    IEnumerator ThreeWin() // 3개 당첨이펙트 
    {
        isEffect = true;
        if (isEffect)
        {
            Effect[0].SetActive(true);
            yield return new WaitForSeconds(1.5f);
            Effect[0].SetActive(false);
        }
        isEffect = false;
    }
    IEnumerator fourWin() // 4개 당첨이펙트
    {
        isEffect = true;
        if (isEffect)
        {
            Effect[1].SetActive(true);
            Effect[2].SetActive(true);
            yield return new WaitForSeconds(1.5f);
            Effect[1].SetActive(false);
            Effect[2].SetActive(false);
        }
        isEffect = false;
    }
    IEnumerator fiveWin()  // 5개 당첨이펙트
    {
        isEffect = true;
        if (isEffect)
        {
            Effect[3].SetActive(true);
            yield return new WaitForSeconds(1.5f);
            Effect[3].SetActive(false);
        }
        isEffect = false;
    }
    IEnumerator BonusEffect()
    {
        isEffect = true;
        isAutoMode = false;
        if (isEffect)
        {
            Effect[4].SetActive(true);
            yield return new WaitForSeconds(2f);
            Effect[4].SetActive(false);
            yield return new WaitForSeconds(1f);
            bonus.BonusWindow.SetActive(true);
            isBonus = true;

        }
    } // 보너스 이펙트


    public void BetBtn()
    {
        if (isBonus)
        {
            BetMoney.text = CommaText(Betting).ToString();
        }
        else
        {
            if (Betting < MaxBetting)
            {
                Betting += 100;
                BetMoney.text = CommaText(Betting).ToString();
                if (Betting >= MaxBetting)
                {
                    Betting = MaxBetting;
                    BetMoney.text = CommaText(Betting).ToString();
                }
            }
        }
    } // 베팅 버튼
    public void DoubleBetBtn()
    {
        if (isBonus)
        {
            BetMoney.text = CommaText(Betting).ToString();
        }
        else
        {
            if (Betting < MaxBetting)
            {
                Betting *= 2;
                BetMoney.text = CommaText(Betting).ToString();
                if (Betting >= MaxBetting)
                {
                    Betting = MaxBetting;
                    BetMoney.text = CommaText(Betting).ToString();
                }
            }
        }
    } // 더블베팅 버튼
    public void BetCancel()
    {
        if (isBonus)
        {
            BetMoney.text = CommaText(Betting).ToString();
        }
        else
        {
            Betting = 100;
            BetMoney.text = CommaText(Betting).ToString();
        }
    } // 배팅 초기화 버튼
    public string CommaText(float data)
    {
        return string.Format("{0:#,###}", data);
    }
}