using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class CutsceneManager : MonoBehaviour
{
    //public TextMeshProUGUI phoneText;
    public bool endOfCutscene;

    /*
    GameManager gameManager;
    public AudioManager audioManager;
    public CameraEffectManager camEffectManager;

    public float startingPosition;

    public float scrollFactor;

    //public float positionX = 1.0f;
    //public float positionY = 1.0f;

    //2 QOL

    // text too close to edges
    // bubble size updating late

    //[Range(0.0f, 10.0f)]
    //public float scroll;

    //[Tooltip("[WIP] Max charcters per line.\nWon't move the whole word to the next line currently")]
    //public int characterLimit;

    public enum character { ALEX, JAMIE };
    [System.Serializable]
    public struct TextList
    {
        public character person;
        [TextArea(3, 10)]
        public List<string> texts;
    }

    public List<TextList> CutsceneOneDialogue;
    public List<TextList> CutsceneOneAndAHalfDialogue;
    public GameObject intro_panel1;
    public GameObject intro_panel2;
    public GameObject intro_panel3;

    [Space(10)]
    public List<TextList> CutsceneTwoDialogue;
    public List<TextList> CutsceneThreeDialogue;
    public List<TextList> GoodEndingDialogue;
    public List<TextList> BadEndingDialogue;
    public List<TextList> credits;

    [HideInInspector]
    public List<TextList> chosenDialogue;

    public float unskipableDelay;

    [Range(0.0f, 1f)]
    public float messageDelayAlex;

    [Range(0.0f, 0.5f)]
    public float textSpeedAlex;

    [Header("Jamie")]

    [Range(0.0f, 5.0f)]
    public float messageDelayJamie;

    public List<GameObject> currentBubbles = new List<GameObject>();
    public Transform messageParent;
    public GameObject alexMessagePrefab;
    public Transform alexMessageTarget;
    public GameObject jamieMessagePrefab;
    public Transform jamieMessageTarget;

    public RectTransform jamieCallsAlexObject;


    [Space(10)]
    public TextMeshProUGUI levelTMP;

    /*[Header("Typing out the message")]
    public bool typeOutJamie;

    [Range(0.0f, 0.5f)]
    public float textSpeedJamie;*/

    /*
        // Start is called before the first frame update
        void Start()
        {
            gameManager = GameManager.instance;
            audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();

            startingPosition = 0;

            gameManager.cutsceneManager = GetComponent<CutsceneManager>();

            if (gameManager.currGame != currGame.CREDITS)
            {
                gameManager.currGame = currGame.CUTSCENE;
            }


            // debug level 
            levelTMP.text = "" + gameManager.currLevel;

            if (GameManager.instance.currLevel == 1)
            {
                StartCoroutine(IntroPanels());
            }
            else
            {
                intro_panel1.SetActive(false);
                intro_panel2.SetActive(false);
                intro_panel3.SetActive(false);
                StartCoroutine(BeginTextingRoutine(GameManager.instance.currLevel));
            }
        }

        public IEnumerator IntroPanels()
        {
            Debug.Log("INTRO PANELS");
            intro_panel1.SetActive(true);
            intro_panel2.SetActive(false);
            intro_panel3.SetActive(false);

            // << PANEL 1 >>
            camEffectManager.StartFadeIn();
            yield return new WaitUntil(() => !camEffectManager.isFading);

            yield return new WaitForSeconds(2);

            camEffectManager.StartFadeOut(1);
            yield return new WaitUntil(() => !camEffectManager.isFading);

            intro_panel1.SetActive(false);
            yield return new WaitForSeconds(0.25f);

            StartCoroutine(BeginTextingRoutine(GameManager.instance.currLevel));
        }

        public IEnumerator BeginTextingRoutine(int op)
        {
            camEffectManager.StartFadeIn(1);
            yield return new WaitUntil(() => !camEffectManager.isFading);

            yield return new WaitForSeconds(1);

            switch (op)
            {
                case 0:
                case 1:
                    chosenDialogue = CutsceneOneDialogue;
                    break;
                case 2:
                    chosenDialogue = CutsceneTwoDialogue;
                    break;
                case 3:
                    chosenDialogue = CutsceneThreeDialogue;
                    break;
                case 4:
                    chosenDialogue = GoodEndingDialogue;
                    break;
                case 5:
                    chosenDialogue = BadEndingDialogue;
                    break;
                case 6:
                    chosenDialogue = credits;
                    if (!gameManager.currHappyEnding)
                    {
                        audioManager.PlaySong(audioManager.sadCreditsPath);
                    }
                    break;
                case 7:
                    chosenDialogue = CutsceneOneAndAHalfDialogue;
                    break;
                default:
                    chosenDialogue = CutsceneOneDialogue;
                    break;
            }


            foreach (TextList textLine in chosenDialogue)
            {
                if (textLine.person == 0)
                {
                    // ALEX
                    //yield return new WaitForSeconds(unskipableDelay);
                    yield return StartCoroutine(AlexText_TypeLine(textLine.texts));
                }
                else
                {
                    //yield return jamieCountdown();
                    //yield return new WaitForSeconds(unskipableDelay);
                    yield return StartCoroutine(JamieText_InstantPrint(textLine.texts));
                }
            }

            yield return new WaitForSeconds(1);

            if (chosenDialogue != GoodEndingDialogue && chosenDialogue != BadEndingDialogue)
            {
                if (chosenDialogue == CutsceneOneDialogue)
                {
                    //StartCoroutine(IntroPanels());
                    // << SHOW PANEL 2 >>
                    camEffectManager.StartFadeOut(2);
                    yield return new WaitUntil(() => !camEffectManager.isFading);

                    intro_panel2.SetActive(true);
                    yield return new WaitForSeconds(1);

                    camEffectManager.StartFadeIn(1);
                    yield return new WaitUntil(() => !camEffectManager.isFading);

                    yield return new WaitForSeconds(2);

                    // << SHOW PANEL 3 >>
                    camEffectManager.StartFadeOut(0.5f);
                    yield return new WaitUntil(() => !camEffectManager.isFading);

                    intro_panel2.SetActive(false);
                    intro_panel3.SetActive(true);
                    yield return new WaitForSeconds(0.5f);

                    camEffectManager.StartFadeIn(0.5f);
                    yield return new WaitUntil(() => !camEffectManager.isFading);
                    yield return new WaitForSeconds(3);
                    // << SHOW 2ND PART OF TEXTING CUTSCENE >>

                    camEffectManager.StartFadeOut(0.5f);
                    yield return new WaitUntil(() => !camEffectManager.isFading);
                    intro_panel3.SetActive(false);
                    yield return new WaitForSeconds(0.5f);
                    camEffectManager.StartFadeIn(0.5f);
                    yield return new WaitUntil(() => !camEffectManager.isFading);
                    yield return new WaitForSeconds(2);
                    StartCoroutine(BeginTextingRoutine(7));

                }
                else if (chosenDialogue == CutsceneOneAndAHalfDialogue)
                {
                    Debug.Log("starting end of intro cutscene");
                    camEffectManager.StartFadeOut(0.5f);
                    endOfCutscene = true;
                }
                else
                {
                    endOfCutscene = true;
                }

            }
            else if (chosenDialogue == GoodEndingDialogue)
            {
                // Move in ALEXXXX !!!! msg
                RectTransform rectTransform = jamieCallsAlexObject.GetComponent<RectTransform>();
                Vector3 startPosition = rectTransform.anchoredPosition3D;
                Vector3 targetPosition = new Vector3(-200, 0, 0);
                float duration = 0.1f;
                float elapsedTime = 0f;

                if (gameManager.currHappyEnding)
                {
                    Debug.Log("MADE IT PAST gameManager.currHappyEnding");
                    FMODUnity.RuntimeManager.StudioSystem.setParameterByName("happyEnding", gameManager.currHappyEnding ? 1 : 0);
                }

                while (elapsedTime < duration)
                {
                    rectTransform.anchoredPosition3D = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                rectTransform.anchoredPosition3D = targetPosition;

                yield return new WaitForSeconds(1f);

                camEffectManager.StartFadeOut();
                yield return new WaitUntil(() => !camEffectManager.isFading);

                gameManager.LoadGoodEnding();
            }
            else if (chosenDialogue == BadEndingDialogue)
            {
                camEffectManager.StartFadeOut();
                yield return new WaitUntil(() => !camEffectManager.isFading);

                gameManager.LoadBadEnding();
            }
            else
            {
                endOfCutscene = true;
            }
        }

        IEnumerator JamieCountdown()
        {
            for (float timer = messageDelayJamie; timer >= 0; timer -= Time.deltaTime)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    yield break;
                }
                yield return null;
            }

        }

        IEnumerator AlexCountdown()
        {
            for (float timer = messageDelayAlex; timer >= 0; timer -= Time.deltaTime)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    yield break;
                }
                yield return null;
            }

        }

        public void MoveBubblesUp(float amount)
        {
            amount *= scrollFactor;
            //Debug.Log("moved bubbles up by: " + amount);
            foreach (GameObject existingBubble in currentBubbles)
            {
                existingBubble.transform.position += new Vector3(0, amount, 0);
            }
        }

        public IEnumerator AlexText_TypeLine(List<string> l)
        {
            foreach (string s in l)
            {




                GameObject bubble = Instantiate(alexMessagePrefab, messageParent);
                bubble.SetActive(false);
                bubble.transform.position = alexMessageTarget.position;

                yield return new WaitForSeconds(0.1f);
                bubble.SetActive(true);

                bubble.GetComponent<BubbleManager>().Init(character.ALEX, s, this);

                yield return StartCoroutine(bubble.GetComponent<BubbleManager>().TextCrawl(s));
                currentBubbles.Add(bubble);
                if (audioManager != null)
                {
                    audioManager.Play(audioManager.sendTextSFX);
                }
                yield return AlexCountdown();
                yield return new WaitForSeconds(unskipableDelay);

            }
        }

        //for printing the entire message at once
        IEnumerator JamieText_InstantPrint(List<string> characterText)
        {
            //Add each element from phone_texts to phoneText
            foreach (string l in characterText)
            {
                yield return new WaitForSeconds(unskipableDelay);
                yield return JamieCountdown();
                //phoneText.text += l + "\n";

                //insert instance of jamiebubble text += l + "\n"
                GameObject bubble = Instantiate(jamieMessagePrefab, messageParent);
                bubble.SetActive(false);
                bubble.transform.position = jamieMessageTarget.position;

                yield return new WaitForSeconds(0.1f);
                bubble.SetActive(true);

                bubble.GetComponent<BubbleManager>().Init(character.JAMIE, l, this);

                yield return StartCoroutine(bubble.GetComponent<BubbleManager>().InstantTextFill(l));

                currentBubbles.Add(bubble);
                if (audioManager != null)
                {
                    audioManager.Play(audioManager.recieveTextSFX);
                }
            }
            yield return JamieCountdown();
        }

        // Update is called once per frame
        void Update()
        {

        }
        */
}
