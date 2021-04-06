using TMPro;
using UnityEngine;

/** 
 * In charge of managing the tutorial.
 * 
 * @author Maxfield Barden
 */

public class TutorialScript : MonoBehaviour
{
    private bool deflected = false;
    private bool moved = false;
    private bool slid = false;
    private bool attacked = false;
    private bool deflectStamina = false;
    private bool moveDisplayed = false;
    private bool slideDisplayed = false;
    private bool attackDisplayed = false;
    private bool deflectDisplayed = false;
    private bool deflectStaminaDisplayed = false;
    private bool thankyou = false;
    private bool tutorialActive = false;
    private float time = 0;
    public Canvas inGameCanvas;
    public TextMeshProUGUI tutorialText;
    private TextMeshProUGUI activeText;
    public PlayerAnimationController attackCont;
    private int currAttacks = 0;
  

    /**
     * Starts the tutorial upon the script being enabled.
     */
    private void OnEnable()
    {
        StartTutorial();
    }

    /**
     * Starts the tutorial.
     */
    public void StartTutorial()
    {
        
        tutorialActive = true;
        
        Invoke("DisplayMoveText", 2f);
    }

    // Update is called once per frame
    void Update()
    {
        if (tutorialActive)
        {
            if (!moved)
            {
                Moved();
            }

            else if (!slid)
            {
                Slid();
            }

            else if (!attacked)
            {
                Attacked();
            }

            else if (!deflected)
            {
                Deflected();
            }

            else if (!deflectStamina)
            {
                DeflectStaminaRead();
            }

            else
            {
                HandleMessagesAtEnd();
            }

        }
    }

    /**
     * Detects if the player moved right or left.
     */
    private void Moved()
    {
        if (moveDisplayed)
        {
            // check to see if player has moved or not
            if (SwipeInput.Instance.SwipeLeft || SwipeInput.Instance.SwipeRight ||
                Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
            {
                // if player has moved, ask for player to slide
                FadeText(activeText);
                Invoke("DisplaySlideText", 1.5f);
                moved = true;

            }
        }
    }

    /**
     * Detects if the player has slid or not.
     */
    private void Slid()
    {
        if (slideDisplayed)
        {
            // check to see if the player has slid
            if (SwipeInput.Instance.SwipeDown || Input.GetKeyDown(KeyCode.S))
            {
                // if the player has slid, ask for the player to attack
                FadeText(activeText);
                Invoke("DisplayAttackText", 1.5f);
                slid = true;
            }
        }
    }

    /**
     * Detects if the player has attacked an enemy.
     */
    private void Attacked()
    {
        if (attackDisplayed)
        {
            if (attackCont.GetAttacks() > currAttacks)
            {
                FadeText(activeText);
                Invoke("DisplayDeflectText", 1.5f);
                attacked = true;
            }
        }
    }

    /**
     * Detects if the player has deflected.
     */
    private void Deflected()
    {
        if (deflectDisplayed)
        {
            if ((SwipeInput.Instance.SwipeUp || Input.GetKeyDown(KeyCode.W)))
            {
                FadeText(activeText);
                Invoke("DisplayDeflectStaminaText", 2f);
                deflected = true;
            }
        }
    }

    /**
     * Detects if player has read through deflect stamina or not.
     */
    private void DeflectStaminaRead()
    {
        if (deflectStaminaDisplayed)
        {
            time += Time.unscaledDeltaTime;
            if (time > 4f)
            {
                time = 0;
                FadeText(activeText);
                deflectStamina = true;
                Invoke("DisplayTutorialCompleted", 2f);
            }
        }
    }

    /**
     * Handles reading the messages at the end.
     */
    private void HandleMessagesAtEnd()
    {
        time += Time.unscaledDeltaTime;
        if (time > 7f)
        {
            FadeText(activeText);
            PlayerPrefs.SetString("tutorialCompleted", "true");
            if (thankyou)
            {
                this.enabled = false;
            }
            time = 0;
        }
    }


    /**
     * Fades out the current active text.
     */
    private void FadeText(TextMeshProUGUI activeText)
    {
        activeText.GetComponent<Animator>().SetTrigger("fadeText");
        Destroy(activeText, 1f);
    }



    /**
     * Displays the swipe to move text.
     */
    private void DisplayMoveText()
    {
        TextMeshProUGUI tempText = Instantiate(tutorialText, inGameCanvas.transform) as TextMeshProUGUI;
        tempText.transform.SetParent(inGameCanvas.transform, false);
        tempText.text = "Swipe Left and Right to Move!";
        tempText.GetComponent<Animator>().SetTrigger("displayMove");
        activeText = tempText;
        moveDisplayed = true;
    }

    /**
     * Displays the swipe down to slide text.
     */
    private void DisplaySlideText()
    {
        TextMeshProUGUI tempText = Instantiate(tutorialText, inGameCanvas.transform) as TextMeshProUGUI;
        tempText.transform.SetParent(inGameCanvas.transform, false);
        tempText.text = "Swipe Down to Slide!";
        tempText.GetComponent<Animator>().SetTrigger("displayMove");
        activeText = tempText;
        slideDisplayed = true;
    }

    /**
     * Displays the attack text.
     */
    private void DisplayAttackText()
    {
        TextMeshProUGUI tempText = Instantiate(tutorialText, inGameCanvas.transform) as TextMeshProUGUI;
        tempText.transform.SetParent(inGameCanvas.transform, false);
        tempText.text = "Run into an Enemy to Attack!";
        tempText.GetComponent<Animator>().SetTrigger("displayMove");
        activeText = tempText;
        attackDisplayed = true;
        currAttacks = attackCont.GetAttacks();
    }

    /**
     * Displays the deflect text.
     */
    private void DisplayDeflectText()
    {
        TextMeshProUGUI tempText = Instantiate(tutorialText, inGameCanvas.transform) as TextMeshProUGUI;
        tempText.transform.SetParent(inGameCanvas.transform, false);
        tempText.text = "Swipe Up to Deflect!";
        tempText.GetComponent<Animator>().SetTrigger("displayMove");
        activeText = tempText;
        deflectDisplayed = true;
    }

    /**
     * Displays the deflect stamina.
     */
    private void DisplayDeflectStaminaText()
    {
        TextMeshProUGUI tempText = Instantiate(tutorialText, inGameCanvas.transform) as TextMeshProUGUI;
        tempText.transform.SetParent(inGameCanvas.transform, false);
        
        tempText.text = ("The lower bar in the top left is your deflect stamina");
        tempText.GetComponent<Animator>().SetTrigger("displayMove");
        deflectStaminaDisplayed = true;
        activeText = tempText;
    }

    /**
     * Displays the completed tutorial message.
     */
    private void DisplayTutorialCompleted()
    {
        TextMeshProUGUI tempText = Instantiate(tutorialText, inGameCanvas.transform) as TextMeshProUGUI;
        tempText.transform.SetParent(inGameCanvas.transform, false);

        tempText.text = ("Once the bar is empty, " +
            "you stop deflecting and the stamina bar will recharge");
        tempText.GetComponent<Animator>().SetTrigger("displayMove");
        activeText = tempText;
        Invoke("DisplayThankYou", 7.15f);
    }

    /**
     * Displays the thank you message :)
     */
    private void DisplayThankYou()
    {
        
        TextMeshProUGUI tempText = Instantiate(tutorialText, inGameCanvas.transform) as TextMeshProUGUI;
        tempText.transform.SetParent(inGameCanvas.transform, false);

        tempText.text = ("You are now ready to take on the cyber gangs!\nGood luck!");
        tempText.GetComponent<Animator>().SetTrigger("displayMove");
        activeText = tempText;
        thankyou = true;
    }
}
