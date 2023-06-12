using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StoryMenu : MonoBehaviour
{
    [Header("Params")]
    [SerializeField] private float typingSpeed = 0.04f;

    [Header("Dialogue UI")]
    [SerializeField] private TextMeshProUGUI dialogueTextBox;
    [SerializeField] private GameObject continueButton;

    [Header("Audio")]
    [SerializeField] private DialogueAudioInfoSO audioInfo;
    [SerializeField] private bool makePredictable;
    private AudioSource audioSource;

    [Header("Dialogue")]
    [SerializeField] [TextArea(3, 20)] private string[] dialogueTexts;
    private int dialogueTextIndex = 0;

    [Header("Images")]
    [SerializeField] private Image appleImage;
    [SerializeField] private Image strawberryImage;
    [SerializeField] private Image lemonImage;
    [SerializeField] private Image blueberryImage;
    [SerializeField] private GameObject actionIcons;
    [SerializeField] private Image rotationGif;
    [SerializeField] private Image basicEnemyImage;
    [SerializeField] private Image knightEnemyImage;
    [SerializeField] private Image copyEnemyImage;

    [Header("Other")]
    [SerializeField] private PauseManager pausem;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        appleImage.gameObject.SetActive(false);
        strawberryImage.gameObject.SetActive(false);
        lemonImage.gameObject.SetActive(false);
        blueberryImage.gameObject.SetActive(false);
        actionIcons.SetActive(false);
        rotationGif.gameObject.SetActive(false);
        basicEnemyImage.gameObject.SetActive(false);
        knightEnemyImage.gameObject.SetActive(false);
        copyEnemyImage.gameObject.SetActive(false);
    }

    public void StartHelp()
    {
        dialogueTextIndex = 0;
        dialogueTextBox.text = "";
        continueButton.SetActive(false);
        appleImage.gameObject.SetActive(false);
        strawberryImage.gameObject.SetActive(false);
        lemonImage.gameObject.SetActive(false);
        blueberryImage.gameObject.SetActive(false);
        actionIcons.SetActive(false);
        rotationGif.gameObject.SetActive(false);
        basicEnemyImage.gameObject.SetActive(false);
        knightEnemyImage.gameObject.SetActive(false);
        copyEnemyImage.gameObject.SetActive(false);
        StartCoroutine(StartStory());
    }

    private IEnumerator StartStory()
    {
        yield return new WaitForSeconds(0.5f);
        CharacterControl.instance.SubToAllDialogueActions();
        StartCoroutine(DisplayLine(dialogueTexts[dialogueTextIndex]));
    }
    private IEnumerator DisplayLine(string line)
    {
        // set the text to the full line, but set the visible characters to 0
        dialogueTextBox.text = line;
        dialogueTextBox.maxVisibleCharacters = 0;
        // hide items while text is typing
        continueButton.SetActive(false);

        bool isAddingRichTextTag = false;

        // display each letter one at a time
        foreach (char letter in line.ToCharArray())
        {
            // if the submit button is pressed, finish up displaying the line right away
            if (CharacterControl.GetProgressDialoguePressed())
            {
                dialogueTextBox.maxVisibleCharacters = line.Length;
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
                dialogueTextBox.maxVisibleCharacters++;
                PlayDialogueSound(dialogueTextBox.maxVisibleCharacters, dialogueTextBox.text[dialogueTextBox.maxVisibleCharacters - 1]);
                yield return new WaitForSeconds(typingSpeed);
            }
        }

        // actions to take after the entire line has finished displaying
        continueButton.SetActive(true);
    }

    private void PlayDialogueSound(int currentDisplayedCharacterCount, char currentCharacter)
    {
        // set variables for the below based on our config
        AudioClip[] dialogueTypingSoundClips = audioInfo.dialogueTypingSoundClips;
        int frequencyLevel = audioInfo.frequencyLevel;
        float minPitch = audioInfo.minPitch;
        float maxPitch = audioInfo.maxPitch;
        bool stopAudioSource = audioInfo.stopAudioSource;

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

    public void ContinueStory()
    {
        AudioManager.instance.Play("Button");
        dialogueTextIndex++;
        if (dialogueTextIndex >= dialogueTexts.Length)
        {
            CharacterControl.instance.UnsubFromAllDialogueActions();
            transform.parent.gameObject.SetActive(false);
            CharacterControl.instance.SubToPause();
            if (pausem.isActiveAndEnabled)
            {
                Time.timeScale = 0f;
            }
            else
            {
                AudioManager.instance.Play("Gameplay Music");
                GameStateManager.instance.StartCoroutine(GameStateManager.instance.AmbientSounds());
                CharacterControl.instance.SubToAllGameplayActions();
            }
        }
        else
        {
            CharacterControl.instance.SubToAllDialogueActions();
            StartCoroutine(DisplayLine(dialogueTexts[dialogueTextIndex]));
            appleImage.gameObject.SetActive(false);
            strawberryImage.gameObject.SetActive(false);
            lemonImage.gameObject.SetActive(false);
            blueberryImage.gameObject.SetActive(false);
            actionIcons.SetActive(false);
            rotationGif.gameObject.SetActive(false);
            basicEnemyImage.gameObject.SetActive(false);
            knightEnemyImage.gameObject.SetActive(false);
            copyEnemyImage.gameObject.SetActive(false);
            switch (dialogueTextIndex)
            {
                case 1:
                    appleImage.gameObject.SetActive(true);
                    break;
                case 2:
                    strawberryImage.gameObject.SetActive(true);
                    break;
                case 3:
                    lemonImage.gameObject.SetActive(true);
                    break;
                case 4:
                    blueberryImage.gameObject.SetActive(true);
                    break;
                case 5:
                    actionIcons.SetActive(true);
                    break;
                case 6:
                    rotationGif.gameObject.SetActive(true);
                    break;
                case 7:
                    basicEnemyImage.gameObject.SetActive(true);
                    break;
                case 8:
                    knightEnemyImage.gameObject.SetActive(true);
                    break;
                case 9:
                    copyEnemyImage.gameObject.SetActive(true);
                    break;
            }
        }
    }
}
