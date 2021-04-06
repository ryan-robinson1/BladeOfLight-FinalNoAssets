using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeManagerScript : MonoBehaviour
{
    public TextMeshProUGUI mainPauseText;
    public TextMeshProUGUI subPauseText;

    private float _slowdownFactor = 0.1f;
    private float _slowMotionLength = 0.15f;
    private float _reentraceSpeed = 1.3f;
    private bool paused = false;


    float _slowMotionTimer = float.PositiveInfinity;
    
    /**
     * Pauses the game.
     */
    public void PauseGame()
    {

        if (paused)
        { 
            subPauseText.transform.gameObject.SetActive(false);
            UnpauseGame();
        }

        else
        {
            paused = true;
            mainPauseText.transform.gameObject.SetActive(true);
            Time.timeScale = 0;
        }
    }

    /**
     * Unpauses the game
     */
    private IEnumerator UnpauseGame(int startTimer, TextMeshProUGUI text)
    {
        text.text = startTimer.ToString();
        yield return new WaitForSecondsRealtime(0.75f);

        startTimer--;
        text.text = startTimer.ToString();
        yield return new WaitForSecondsRealtime(0.75f);

        startTimer--;
        text.text = startTimer.ToString();
        yield return new WaitForSecondsRealtime(0.75f);

        Time.timeScale = 1;

        subPauseText.gameObject.SetActive(true);
        text.text = "PAUSED";
        mainPauseText.gameObject.SetActive(false);
        paused = false;
        
        
    }

    private void UnpauseGame()
    {
        StartCoroutine(UnpauseGame(3, mainPauseText));
    }

    private void DoSlowMotion()
    {

        Time.timeScale = _slowdownFactor;
        Time.fixedDeltaTime = Time.timeScale * 0.01f;
        _slowMotionTimer = Time.time;
        //camera.followSpeed = 40f;

    }

    private void UndoSlowMotion()
    {
        Time.timeScale += _reentraceSpeed * Time.unscaledDeltaTime;
        Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
        Time.fixedDeltaTime = Time.deltaTime / 2f;
       
        if (Time.timeScale == 1)
        {
            _slowMotionTimer = float.PositiveInfinity;
            Time.fixedDeltaTime = 0.01f;
            //camera.followSpeed = 10f;
        }
    }
    private void ResetSlowMotionTimer()
    {
        if (Time.time - _slowMotionTimer > _slowMotionLength)
        {
            UndoSlowMotion();
        }
    }
    public void SlowMotion()
    {
        DoSlowMotion();
    }
    public void SlowMotion(float slowDownFactor,float slowMotionLength,float reentranceSpeed)
    {
        _slowdownFactor = slowDownFactor;
        _slowMotionLength = slowMotionLength;
        _reentraceSpeed = reentranceSpeed;
        DoSlowMotion();
    }
  
    
}
