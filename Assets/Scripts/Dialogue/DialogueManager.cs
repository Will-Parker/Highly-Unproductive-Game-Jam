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
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialogueMenu;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI displayNameText;
    [SerializeField] private RectTransform characterPortraits;

    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

    public Story currentStory { get; private set; }
    public bool dialogueIsPlaying { get; private set; }

    private static DialogueManager instance;

    private const string SPEAKER_TAG = "speaker"; // speaker:name,isInternal
                                                  // sets speaker text, enlarges speaker portrait, and sets text color to corresponding color
    public Participant? speaker { get; set; }
    private const string ENTER_TAG = "enter"; // enter:name,index[optional] 
                                              // instantiates portrait with name, enters with default animation, at index default last
    private const string EXIT_TAG = "exit"; // exit:name
                                            // exits with default animation, destroys object after animation.
    private const string PORTRAIT_TAG = "portrait"; // portrait:name,anim
                                                    // target with name name switches portrait mood via animation
    private const string SCENE_TAG = "scene"; // scene:anim
                                              // switches scene background via animation

    public List<Participant> participants { get; private set; }
    private bool isDialogueInternal = false;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager in the scene");
        }
        instance = this;
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

        participants = new List<Participant>();
    }

    private void Update()
    {
        // return right away if dialogue isn't playing
        if (!dialogueIsPlaying)
        {
            return;
        }

        // handle continuing to the next line in the dialogue when submit is pressed
        // -> IN Character Control.cs
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
            dialogueText.text = ItalicFormatText(ColorFormatTextBasedOnActiveSpeaker(unformattedDialogueText));
            // display choices, if any, for this dialogue line
            DisplayChoices();
        }
        else
        {
            ExitDialogueMode();
        }
    }

    private string ItalicFormatText(string unformattedDialogueText)
    {
        if (speaker == null)
        {
            Debug.LogError("Speaker was null");
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
            Debug.LogError("Speaker was null");
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
                case PORTRAIT_TAG:
                    HandlePortraitTag(tagKey, tagValues);
                    break;
                case SCENE_TAG:
                    Debug.LogWarning("Scene tags are currently unimplemented: " + ParsedTagLog(tagKey, tagValues));
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
                isDialogueInternal = false;
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
            isDialogueInternal = false;
        }
        if (tagValues.Length == 2)
        {
            if (!bool.TryParse(tagValues[1], out isDialogueInternal))
                Debug.LogWarning("Could not parse boolean value. Tag: " + ParsedTagLog(tagKey, tagValues));
        }
        else if (tagValues.Length > 2)
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
        }

        public Participant(string name, GameObject gO)
        {
            Name = name;
            GO = gO;
            Anim = GO.GetComponent<Animator>();
            if (Anim == null)
                Debug.LogWarning("No animator was attached to the participant gameObject");
            Type = instance.GetAllyType(name);
        }

        public Participant(string name, GameObject gO, AllyType? type)
        {
            Name = name;
            GO = gO;
            Anim = GO.GetComponent<Animator>();
            if (Anim == null)
                Debug.LogWarning("No animator was attached to the participant gameObject");
            Type = type;
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
}
