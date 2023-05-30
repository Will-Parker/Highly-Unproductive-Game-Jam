// Credit: https://www.youtube.com/playlist?list=PL3viUl9h9k78KsDxXoAzgQ1yRjhm7p8kl
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;
using System;

public class DialogueManager : MonoBehaviour
{
    [Header("Params")]
    [SerializeField] private float typingSpeed = 0.04f;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialogueMenu;
    [SerializeField] private GameObject continueIcon;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI displayNameText;
    [SerializeField] private RectTransform characterPortraits;

    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

    [Header("Audio")]
    [SerializeField] private DialogueAudioInfoSO defaultAudioInfo;
    [SerializeField] private DialogueAudioInfoSO[] audioInfos;
    [SerializeField] private bool makePredictable;

    private DialogueAudioInfoSO currentAudioInfo;
    private Dictionary<string, DialogueAudioInfoSO> audioInfoDictionary;

    private AudioSource audioSource;

    [Header("Stories")]
    [SerializeField] private TextAsset[] AppleBlueberryStories;
    [SerializeField] private TextAsset[] AppleLemonStories;
    [SerializeField] private TextAsset[] AppleStrawberryStories;
    [SerializeField] private TextAsset[] BlueberryLemonStories;
    [SerializeField] private TextAsset[] BlueberryStrawberryStories;
    [SerializeField] private TextAsset[] LemonStrawberryStories;
    private List<TextAsset> unusedAppleBlueberryStories;
    private List<TextAsset> unusedAppleLemonStories;
    private List<TextAsset> unusedAppleStrawberryStories;
    private List<TextAsset> unusedBlueberryLemonStories;
    private List<TextAsset> unusedBlueberryStrawberryStories;
    private List<TextAsset> unusedLemonStrawberryStories;

    public Story currentStory { get; private set; }
    public bool dialogueIsPlaying { get; private set; }
    private Coroutine displayLineCoroutine;
    private bool canContinueToNextLine = false;

    private static DialogueManager instance;
    public Participant? speaker { get; set; }
    private bool isDialogueInternal = false;

    private const string SPEAKER_TAG = "speaker"; // speaker:name
                                                  // sets speaker text, enlarges speaker portrait, and sets text color to corresponding color
    private const string ENTER_TAG = "enter"; // enter:name,index[optional] 
                                              // instantiates portrait with name, enters with default animation, at index default last
    private const string EXIT_TAG = "exit"; // exit:name
                                            // exits with default animation, destroys object after animation.
    private const string INTERNAL_TAG = "internal"; // internal:bool
                                                    // sets dialogue to italics when true, normal when false
    private const string PORTRAIT_TAG = "portrait"; // portrait:name,anim
                                                    // target with name name switches portrait mood via animation
    private const string SCENE_TAG = "scene"; // scene:anim
                                              // switches scene background via animation
    private const string OPTION_TAG = "option"; // option:good/bad
                                                // determines if the good or bad option was picked
    public List<Participant> participants { get; private set; }

    public AllyType[] alliesInDialogue;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager in the scene");
        }
        instance = this;

        audioSource = gameObject.AddComponent<AudioSource>();
        currentAudioInfo = defaultAudioInfo;
    }

    public static DialogueManager GetInstance()
    {
        return instance;
    }

    private void Start()
    {
        dialogueIsPlaying = false;
        dialogueMenu.SetActive(false);

        // get all of the choices text
        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }

        InitializeAudioInfoDictionary();

        unusedAppleBlueberryStories = new List<TextAsset>(AppleBlueberryStories);
        unusedAppleLemonStories = new List<TextAsset>(AppleLemonStories);
        unusedAppleStrawberryStories = new List<TextAsset>(AppleStrawberryStories);
        unusedBlueberryLemonStories = new List<TextAsset>(BlueberryLemonStories);
        unusedBlueberryStrawberryStories = new List<TextAsset>(BlueberryStrawberryStories);
        unusedLemonStrawberryStories = new List<TextAsset>(LemonStrawberryStories);

        participants = new List<Participant>();
    }

    private DialogueAudioInfoSO GetAudioInfo(string id)
    {
        DialogueAudioInfoSO audioInfo = null;
        audioInfoDictionary.TryGetValue(id, out audioInfo);
        if (audioInfo != null)
        {
            return audioInfo;
        }
        else
        {
            Debug.LogWarning("Audio Info with ID: " + id + ", was not found. Returning default Audio Info.");
            return defaultAudioInfo;
        }

    }

    private void InitializeAudioInfoDictionary()
    {
        audioInfoDictionary = new Dictionary<string, DialogueAudioInfoSO>();
        audioInfoDictionary.Add(defaultAudioInfo.id, defaultAudioInfo);
        foreach (DialogueAudioInfoSO audioInfo in audioInfos)
        {
            audioInfoDictionary.Add(audioInfo.id, audioInfo);
        }
    }

    private void Update()
    {
        // return right away if dialogue isn't playing
        if (!dialogueIsPlaying)
        {
            return;
        }

        // handle continuing to the next line in the dialogue when submit is pressed
        if (canContinueToNextLine
                && currentStory.currentChoices.Count == 0
                && CharacterControl.GetProgressDialoguePressed())
        {
            ContinueStory();
        }
    }

    public void BeginDialogueWith(AllyType ally1, AllyType ally2)
    {
        bool isAppleInScene = ally1 == AllyType.Apple || ally2 == AllyType.Apple;
        bool isBlueberryInScene = ally1 == AllyType.Blueberry || ally2 == AllyType.Blueberry;
        bool isLemonInScene = ally1 == AllyType.Lemon || ally2 == AllyType.Lemon;
        bool isStrawberryInScene = ally1 == AllyType.Strawberry || ally2 == AllyType.Strawberry;

        TextAsset story;
        if (isAppleInScene && isBlueberryInScene)
        {
            story = unusedAppleBlueberryStories[UnityEngine.Random.Range(0, unusedAppleBlueberryStories.Count)];
            unusedAppleBlueberryStories.Remove(story);
            if (unusedAppleBlueberryStories.Count <= 0)
            {
                unusedAppleBlueberryStories = new List<TextAsset>(AppleBlueberryStories);
            }
        }
        else if (isAppleInScene && isLemonInScene)
        {
            story = unusedAppleLemonStories[UnityEngine.Random.Range(0, unusedAppleLemonStories.Count)];
            unusedAppleLemonStories.Remove(story);
            if (unusedAppleLemonStories.Count <= 0)
            {
                unusedAppleLemonStories = new List<TextAsset>(AppleLemonStories);
            }
        }
        else if (isAppleInScene && isStrawberryInScene)
        {
            story = unusedAppleStrawberryStories[UnityEngine.Random.Range(0, unusedAppleStrawberryStories.Count)];
            unusedAppleStrawberryStories.Remove(story);
            if (unusedAppleStrawberryStories.Count <= 0)
            {
                unusedAppleStrawberryStories = new List<TextAsset>(AppleStrawberryStories);
            }
        }
        else if (isBlueberryInScene && isLemonInScene)
        {
            story = unusedBlueberryLemonStories[UnityEngine.Random.Range(0, unusedBlueberryLemonStories.Count)];
            unusedBlueberryLemonStories.Remove(story);
            if (unusedBlueberryLemonStories.Count <= 0)
            {
                unusedBlueberryLemonStories = new List<TextAsset>(BlueberryLemonStories);
            }
        }
        else if (isBlueberryInScene && isStrawberryInScene)
        {
            story = unusedBlueberryStrawberryStories[UnityEngine.Random.Range(0, unusedBlueberryStrawberryStories.Count)];
            unusedBlueberryStrawberryStories.Remove(story);
            if (unusedBlueberryStrawberryStories.Count <= 0)
            {
                unusedBlueberryStrawberryStories = new List<TextAsset>(BlueberryStrawberryStories);
            }
        }
        else if (isLemonInScene && isStrawberryInScene)
        {
            story = unusedLemonStrawberryStories[UnityEngine.Random.Range(0, unusedLemonStrawberryStories.Count)];
            unusedLemonStrawberryStories.Remove(story);
            if (unusedLemonStrawberryStories.Count <= 0)
            {
                unusedLemonStrawberryStories = new List<TextAsset>(LemonStrawberryStories);
            }
        }
        else
        {
            Debug.LogError("Invalid combination of allies to start a dialogue with. Aborting");
            return;
        }

        alliesInDialogue = new AllyType[2] { ally1, ally2 };
        EnterDialogueMode(story);
    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialogueMenu.SetActive(true);

        // reset 
        displayNameText.text = "???";

        ContinueStory();
    }

    private void ExitDialogueMode()
    {
        dialogueIsPlaying = false;
        dialogueMenu.SetActive(false);
        speaker = null;
        foreach (Participant participant in participants)
        {
            Destroy(participant.GO);
        }
        participants.RemoveAll(x => true);
        dialogueText.text = "";

        FindObjectOfType<CharacterControl>().SubToAllGameplayActions();
        FindObjectOfType<CharacterControl>().UnsubFromAllDialogueActions();
    }

    public void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            // get text for the next dialogue line and save it to format after tags are handled
            string unformattedDialogueText = currentStory.Continue();
            // handle tags
            HandleTags(currentStory.currentTags);
            // set dialogue text to formatted version of next dialogue line
            string formattedText = InternalFormatText(ColorFormatTextBasedOnActiveSpeaker(unformattedDialogueText));
            if (displayLineCoroutine != null)
            {
                StopCoroutine(displayLineCoroutine);
            }
            displayLineCoroutine = StartCoroutine(DisplayLine(formattedText));
        }
        else
        {
            ExitDialogueMode();
        }
    }

    private IEnumerator DisplayLine(string line)
    {
        // set the text to the full line, but set the visible characters to 0
        dialogueText.text = line;
        dialogueText.maxVisibleCharacters = 0;
        // hide items while text is typing
        continueIcon.SetActive(false);
        HideChoices();

        canContinueToNextLine = false;

        bool isAddingRichTextTag = false;

        // display each letter one at a time
        foreach (char letter in line.ToCharArray())
        {
            // if the submit button is pressed, finish up displaying the line right away
            if (CharacterControl.GetProgressDialoguePressed())
            {
                dialogueText.maxVisibleCharacters = line.Length;
                break;
            }

            // check for rich text tag, if founf, add it without waiting
            if (letter == '<' || isAddingRichTextTag)
            {
                isAddingRichTextTag = true;
                if (letter == '>')
                {
                    isAddingRichTextTag = false;
                }
            }
            // if not rich text, add the next letter and wait a small time
            else
            {
                dialogueText.maxVisibleCharacters++;
                PlayDialogueSound(dialogueText.maxVisibleCharacters, dialogueText.text[dialogueText.maxVisibleCharacters - 1]);
                yield return new WaitForSeconds(typingSpeed);
            }
        }

        // actions to take after the entire line has finished displaying
        continueIcon.SetActive(true);
        DisplayChoices(); // display choices, if any, for this dialogue line

        canContinueToNextLine = true;
    }

    private void PlayDialogueSound(int currentDisplayedCharacterCount, char currentCharacter)
    {
        // set variables for the below based on our config
        AudioClip[] dialogueTypingSoundClips = currentAudioInfo.dialogueTypingSoundClips;
        int frequencyLevel = currentAudioInfo.frequencyLevel;
        float minPitch = currentAudioInfo.minPitch;
        float maxPitch = currentAudioInfo.maxPitch;
        bool stopAudioSource = currentAudioInfo.stopAudioSource;

        // play the sound based on the config
        if ((currentDisplayedCharacterCount - 1) % frequencyLevel == 0)
        {
            if (stopAudioSource)
            {
                audioSource.Stop();
            }
            AudioClip soundClip = null;
            // create predictable audio from hashing
            if (makePredictable)
            {
                // generate hash code
                int hashCode = GetDeterministicHashCode(currentCharacter); // currentCharacter.GetHashCode();
                // sound clip
                int predictableIndex = hashCode % dialogueTypingSoundClips.Length;
                soundClip = dialogueTypingSoundClips[predictableIndex];
                // pitch
                int minPitchInt = (int)(minPitch * 100);
                int maxPitchInt = (int)(maxPitch * 100);
                int pitchRangeInt = maxPitchInt - minPitchInt;
                // cannot divide by 0, so if there is no range then skip the selection
                if (pitchRangeInt != 0)
                {
                    int predictablePitchInt = (hashCode % pitchRangeInt) + minPitchInt;
                    float predictablePitch = predictablePitchInt / 100f;
                    audioSource.pitch = predictablePitch;
                }
                else
                {
                    audioSource.pitch = minPitch;
                }
            }
            // otherwise, randomize the audio
            else
            {
                // sound clip
                int randomIndex = UnityEngine.Random.Range(0, dialogueTypingSoundClips.Length);
                soundClip = dialogueTypingSoundClips[randomIndex];
                // pitch
                audioSource.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
            }
            // play sound
            audioSource.PlayOneShot(soundClip);
        }
    }

    private void HideChoices()
    {
        foreach (GameObject choiceButton in choices)
        {
            choiceButton.SetActive(false);
        }
    }

    private string InternalFormatText(string unformattedDialogueText)
    {
        if (speaker == null)
        {
            return unformattedDialogueText;
        }
        else
        {
            if (isDialogueInternal)
            {
                return "<i>" + unformattedDialogueText + "</i>";
            }
            else
            {
                return unformattedDialogueText;
            }
        }
    }

    private string ColorFormatTextBasedOnActiveSpeaker(string unformattedDialogueText)
    {
        if (speaker == null)
        {
            return unformattedDialogueText;
        }
        else
        {
            AllyType? allyType = speaker.Value.Type;
            if (allyType != null)
            {
                string colorTag = GetColorTag((AllyType)allyType);
                return colorTag + unformattedDialogueText + "</color>";
            }
            else
            {
                return unformattedDialogueText;
            }
        }
    }

    private void HandleTags(List<string> currentTags)
    {
        // loop  through each tag and handle it accordingly
        foreach (string tag in currentTags)
        {
            // parse the tag
            string[] splitTag = tag.Split(':',',');
            if (splitTag.Length < 2)
            {
                Debug.LogError("Tag could not be appropriately parsed: " + tag);
            }
            string tagKey = splitTag[0].Trim();
            string[] tagValues = new string[splitTag.Length - 1];
            for (int i = 0; i < tagValues.Length; i++)
            {
                tagValues[i] = splitTag[i+1].Trim();
            }

            // handle the tag
            switch (tagKey)
            {
                case SPEAKER_TAG:
                    HandleSpeakerTag(tagKey, tagValues);
                    break;
                case ENTER_TAG:
                    HandleEnterTag(tagKey, tagValues);
                    break;
                case EXIT_TAG:
                    HandleExitTag(tagKey, tagValues);
                    break;
                case INTERNAL_TAG:
                    HandleInternalTag(tagKey, tagValues);
                    break;
                case PORTRAIT_TAG:
                    HandlePortraitTag(tagKey, tagValues);
                    break;
                case SCENE_TAG:
                    Debug.LogWarning("Scene tags are currently unimplemented: " + ParsedTagLog(tagKey, tagValues));
                    break;
                case OPTION_TAG:
                    HandleOptionTag(tagKey, tagValues);
                    break;
                default:
                    Debug.LogWarning("Tag came in but is not currently being handled: " + tag);
                    break;
            }
        }
    }

    private void HandleSpeakerTag(string tagKey, string[] tagValues)
    {
        if (!participants.Exists(participant => participant.Name == tagValues[0]))
        {
            if (tagValues[0].ToLower() == "system")
            {
                if (speaker != null)
                    speaker.Value.Anim.SetBool("isSpeaking", false);
                speaker = null;
                displayNameText.transform.parent.gameObject.SetActive(false);
                isDialogueInternal = true;
                currentAudioInfo = GetAudioInfo("default");
            }
            else
            {
                Debug.LogWarning("Name Mismatch: participant in scene with name " + tagValues[0] + " not found. Adding to scene now.");
                if (speaker != null)
                    speaker.Value.Anim.SetBool("isSpeaking", false);
                displayNameText.transform.parent.gameObject.SetActive(true);
                // Assign the speaker according to the parsed tag
                speaker = new Participant(tagValues[0]);
                participants.Add(speaker.Value);
                // Set name text to text formatted based on active speaker
                displayNameText.text = ColorFormatTextBasedOnActiveSpeaker(tagValues[0]);
                // Update Speaker Animator to enlarge portrait
                speaker.Value.Anim.SetBool("isSpeaking", true);
                currentAudioInfo = speaker.Value.AudioInfo;
            }
        }
        else
        {
            if (speaker != null)
                speaker.Value.Anim.SetBool("isSpeaking", false);
            displayNameText.transform.parent.gameObject.SetActive(true);
            // Assign the speaker according to the parsed tag
            speaker = participants.Find(participant => participant.Name == tagValues[0]);
            // Set name text to text formatted based on active speaker
            displayNameText.text = ColorFormatTextBasedOnActiveSpeaker(tagValues[0]);
            // Update Speaker Animator to enlarge portrait
            speaker.Value.Anim.SetBool("isSpeaking", true);
            currentAudioInfo = speaker.Value.AudioInfo;
        }
        if (tagValues.Length > 1)
        {
            Debug.LogWarning("Parsed tag had more values than expected. Tag: " + ParsedTagLog(tagKey, tagValues));
        }
    }

    private void HandleEnterTag(string tagKey, string[] tagValues)
    {
        if (tagValues.Length == 1)
        {
            Participant participantEntering = new Participant(tagValues[0]);
            participants.Add(participantEntering);

        }
        else if (tagValues.Length == 2)
        {
            Participant participantEntering = new Participant(tagValues[0], int.Parse(tagValues[1]));
            participants.Insert(int.Parse(tagValues[1]), participantEntering);
        }
        else
        {
            Debug.LogWarning("Parsed tag had more values than expected. Tag: " + ParsedTagLog(tagKey, tagValues));
            participants.Add(new Participant(tagValues[0]));
        }
    }

    private void HandleExitTag(string tagKey, string[] tagValues)
    {
        if (tagValues.Length == 1)
        {
            if (!participants.Exists(participant => participant.Name == tagValues[0]))
            {
                Debug.LogWarning("Name Mismatch: participant in scene with name " + tagValues[0] + " not found. Unable to remove.");
            }
            else
            {
                Participant participantExiting = participants.Find(participant => participant.Name == tagValues[0]);
                participantExiting.Anim.SetBool("Exit", true);
            }
        }
        else
        {
            Debug.LogWarning("Parsed tag had more values than expected. Tag: " + ParsedTagLog(tagKey, tagValues));
        }
    }

    private void HandleInternalTag(string tagKey, string[] tagValues)
    {
        if (!bool.TryParse(tagValues[0], out isDialogueInternal))
            Debug.LogWarning("Could not parse boolean value. Tag: " + ParsedTagLog(tagKey, tagValues));
    }

    private void HandlePortraitTag(string tagKey, string[] tagValues)
    {
        if (tagValues.Length < 2)
            Debug.LogWarning("Parsed tag had less values than expected. Tag: " + ParsedTagLog(tagKey, tagValues));
        else if (tagValues.Length > 2)
            Debug.LogWarning("Parsed tag had more values than expected. Tag: " + ParsedTagLog(tagKey, tagValues));
        else
        {
            if (!participants.Exists(participant => participant.Name == tagValues[0]))
            {
                Debug.LogWarning("Name Mismatch: participant in scene with name " + tagValues[0] + " not found. Unable to swap portrait mood.");
            }
            else
            {
                Debug.LogWarning("Scene tags currently unimplemented: " + ParsedTagLog(tagKey, tagValues));
                Participant participant = participants.Find(participant => participant.Name == tagValues[0]);
                participant.Anim.Play(tagValues[1], 1);
            }
        }
    }

    private void HandleOptionTag(string tagKey, string[] tagValues)
    {
        if (tagValues[0] == "good")
        {
            Ally a1 = FindObjectOfType<PartyManager>().GetAlly(alliesInDialogue[0]);
            Ally a2 = FindObjectOfType<PartyManager>().GetAlly(alliesInDialogue[1]);
            a1.isLevelUp = true;
            a2.isLevelUp = true;
            a1.partnerBuffs[a2.type][StatType.MaxHealth] += 3;
            a1.partnerBuffs[a2.type][StatType.Attack] += 1;
            switch (a1.type)
            {
                case AllyType.Apple:
                    a1.partnerBuffs[a2.type][StatType.HeavyAttack] += 2;
                    break;
                case AllyType.Strawberry:
                    a1.partnerBuffs[a2.type][StatType.Heal] += 3;
                    break;
                case AllyType.Lemon:
                    a1.partnerBuffs[a2.type][StatType.Stun] += 2;
                    break;
                case AllyType.Blueberry:
                    a1.partnerBuffs[a2.type][StatType.Bomb] += 2;
                    break;
            }
            a2.partnerBuffs[a1.type][StatType.MaxHealth] += 3;
            a2.partnerBuffs[a1.type][StatType.Attack] += 1;
            switch (a2.type)
            {
                case AllyType.Apple:
                    a2.partnerBuffs[a1.type][StatType.HeavyAttack] += 2;
                    break;
                case AllyType.Strawberry:
                    a2.partnerBuffs[a1.type][StatType.Heal] += 3;
                    break;
                case AllyType.Lemon:
                    a2.partnerBuffs[a1.type][StatType.Stun] += 2;
                    break;
                case AllyType.Blueberry:
                    a2.partnerBuffs[a1.type][StatType.Bomb] += 2;
                    break;
            }
        }
        else if (tagValues[0] == "bad")
        {
            Ally a1 = FindObjectOfType<PartyManager>().GetAlly(alliesInDialogue[0]);
            Ally a2 = FindObjectOfType<PartyManager>().GetAlly(alliesInDialogue[1]);
            a1.isLevelUp = true;
            a2.isLevelUp = true;
            a1.partnerBuffs[a2.type][StatType.MaxHealth] += 1;
            a1.partnerBuffs[a2.type][StatType.Attack] += 0;
            switch (a1.type)
            {
                case AllyType.Apple:
                    a1.partnerBuffs[a2.type][StatType.HeavyAttack] += 1;
                    break;
                case AllyType.Strawberry:
                    a1.partnerBuffs[a2.type][StatType.Heal] += 1;
                    break;
                case AllyType.Lemon:
                    a1.partnerBuffs[a2.type][StatType.Stun] += 1;
                    break;
                case AllyType.Blueberry:
                    a1.partnerBuffs[a2.type][StatType.Bomb] += 1;
                    break;
            }
            a2.partnerBuffs[a1.type][StatType.MaxHealth] += 1;
            a2.partnerBuffs[a1.type][StatType.Attack] += 0;
            switch (a2.type)
            {
                case AllyType.Apple:
                    a2.partnerBuffs[a1.type][StatType.HeavyAttack] += 1;
                    break;
                case AllyType.Strawberry:
                    a2.partnerBuffs[a1.type][StatType.Heal] += 1;
                    break;
                case AllyType.Lemon:
                    a2.partnerBuffs[a1.type][StatType.Stun] += 1;
                    break;
                case AllyType.Blueberry:
                    a2.partnerBuffs[a1.type][StatType.Bomb] += 1;
                    break;
            }
        }
        else
        {
            Debug.LogWarning("Tag had invalid option value. Tag: " + ParsedTagLog(tagKey, tagValues));
        }
    }

    private string ParsedTagLog(string tagKey, string[] tagValues)
    {
        string parsedTagLog = tagKey;
        parsedTagLog += "=" + tagValues[0];
        for (int i = 1; i < tagValues.Length; i++)
        {
            parsedTagLog += "," + tagValues[i];
        }
        return parsedTagLog;
    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        // defensive check to mak sure our UI can support the number of choices coming in
        if (currentChoices.Count > choices.Length)
        {
            Debug.LogError("More choices were given than the UI can support. Number of choices given: " 
                + currentChoices.Count);
        }

        int index = 0;
        // enable and initialize the choices
        foreach (Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = ColorFormatTextBasedOnActiveSpeaker(choice.text);
            index++;
        }
        // go through the remaining choices the UI supports and make sure they're hidden
        for (int i = index; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }

        //StartCoroutine(SelectFirstChoice());
    }

    //private IEnumerator SelectFirstChoice()
    //{
    //    // Event System requires we clear it first, then wait
    //    // for at least one frame before we set the current selected object.
    //    EventSystem.current.SetSelectedGameObject(null);
    //    yield return new WaitForEndOfFrame();
    //    EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
    //}

    public void MakeChoice(int choiceIndex)
    {
        currentStory.ChooseChoiceIndex(choiceIndex);
        ContinueStory();
        CharacterControl.GetProgressDialoguePressed();
    }

    private AllyType? GetAllyType(string allyString)
    {
        return allyString.Trim().ToLower() switch
        {
            "apple" => AllyType.Apple,
            "blueberry" => AllyType.Blueberry,
            "lemon" => AllyType.Lemon,
            "strawberry" => AllyType.Strawberry,
            _ => null,
        };
    }

    private string GetColorTag(AllyType allyType)
    {
        return allyType switch
        {
            AllyType.Apple => "<color=#73FF5A>",
            AllyType.Strawberry => "<color=#FA5961>",
            AllyType.Lemon => "<color=#FFE25A>",
            AllyType.Blueberry => "<color=#5AD4FF>",
            _ => null,
        };
    }

    public readonly struct Participant
    {
        public readonly string Name { get; }
        public readonly GameObject GO { get; }
        public readonly Animator Anim { get; }
        public readonly AllyType? Type { get; }
        public readonly DialogueAudioInfoSO AudioInfo { get; }

        public Participant(string name) : this(name, instance.GetAllyType(name)) { }

        public Participant(string name, AllyType? type)
        {
            Name = name;
            Type = type;
            GO = Type switch
            {
                AllyType.Apple => Instantiate(Resources.Load("Prefabs/UI/Dialogue/Portraits/Apple Portrait") as GameObject, instance.characterPortraits),
                AllyType.Strawberry => Instantiate(Resources.Load("Prefabs/UI/Dialogue/Portraits/Strawberry Portrait") as GameObject, instance.characterPortraits),
                AllyType.Lemon => Instantiate(Resources.Load("Prefabs/UI/Dialogue/Portraits/Lemon Portrait") as GameObject, instance.characterPortraits),
                AllyType.Blueberry => Instantiate(Resources.Load("Prefabs/UI/Dialogue/Portraits/Blueberry Portrait") as GameObject, instance.characterPortraits),
                _ => Instantiate(Resources.Load("Prefabs/UI/Dialogue/Portraits/Unknown Portrait") as GameObject, instance.characterPortraits),
            };
            Anim = GO.GetComponent<Animator>();
            if (Anim == null)
                Debug.LogWarning("No animator was attached to the participant gameObject");
            AudioInfo = instance.GetAudioInfo(name);
        }

        public Participant(string name, GameObject gO)
        {
            Name = name;
            GO = gO;
            Anim = GO.GetComponent<Animator>();
            if (Anim == null)
                Debug.LogWarning("No animator was attached to the participant gameObject");
            Type = instance.GetAllyType(name);
            AudioInfo = instance.GetAudioInfo(name);
        }

        public Participant(string name, GameObject gO, AllyType? type)
        {
            Name = name;
            GO = gO;
            Anim = GO.GetComponent<Animator>();
            if (Anim == null)
                Debug.LogWarning("No animator was attached to the participant gameObject");
            Type = type;
            AudioInfo = instance.GetAudioInfo(name);
        }

        public Participant(string name, int index) : this(name)
        {
            GO.transform.SetSiblingIndex(index);
        }
        public Participant(string name, AllyType? type, int index) : this (name, type)
        {
            GO.transform.SetSiblingIndex(index);
        }
        public Participant(string name, GameObject gO, int index) : this (name, gO)
        {
            GO.transform.SetSiblingIndex(index);
        }
        public Participant(string name, GameObject gO, AllyType? type, int index) : this (name, gO, type)
        {
            GO.transform.SetSiblingIndex(index);
        }

        public override string ToString() => $"{Name} of type {Type}. GameObject is {GO}";
        public bool Equals(Participant obj)
        {
            return Name.Equals(obj.Name) && GO.Equals(obj.GO) && Anim.Equals(obj.Anim) && Type.Equals(obj.Type);
        }
    }

    private int GetDeterministicHashCode(char c)
    {
        unchecked
        {
            int hash1 = (5381 << 16) + 5381;
            int hash2 = hash1;

            hash1 = ((hash1 << 5) + hash1) ^ c;
            hash2 = ((hash2 << 5) + hash2) ^ (c << 1);

            return Mathf.Abs(hash1 + (hash2 * 1566083941));
        }
    }

}
