using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

/// <summary>
/// Class for listening to the voice which coverts the voice to a integer and passes it on to the bridge behaviour class.
/// If the recognized text is not a integer it ignores and restarts the listener
/// </summary>
public class SpeechToTextManager : MonoBehaviour
{

    private DictationRecognizer dictationRecognizer;

    // Use this string to cache the text currently displayed in the text box.
    private StringBuilder textSoFar;

    private bool hasRecordingStarted;

    // Using an empty string specifies the default microphone. 
    private static string deviceName = string.Empty;
    private int samplingRate=44000;
    private const int messageLength = 5;

    void Awake()
    {

        // Create a new DictationRecognizer and assign it to dictationRecognizer variable.
        dictationRecognizer = new DictationRecognizer();

        // This event is fired after the user pauses, typically at the end of a sentence. The full recognized string is returned here.
        dictationRecognizer.DictationResult += DictationRecognizer_DictationResult;

        // This event is fired when the recognizer stops, whether from Stop() being called, a timeout occurring, or some other error.
        dictationRecognizer.DictationComplete += DictationRecognizer_DictationComplete;

        // This event is fired when an error occurs.
        dictationRecognizer.DictationError += DictationRecognizer_DictationError;

        // Query the maximum frequency of the default microphone. Use 'unused' to ignore the minimum frequency.
        int unused;
        Microphone.GetDeviceCaps(deviceName, out unused, out samplingRate);

        // Use this string to cache the text currently displayed in the text box.
        textSoFar = new StringBuilder();

        hasRecordingStarted = false;

    }

    void Update()
    {
        // Add condition to check if dictationRecognizer.Status is Running
        if (hasRecordingStarted && !Microphone.IsRecording(deviceName) && dictationRecognizer.Status == SpeechSystemStatus.Running)
        {
            hasRecordingStarted = false;
            StopRecording();
            // This acts like pressing the Stop button and sends the message to the Communicator.
            // If the microphone stops as a result of timing out, make sure to manually stop the dictation recognizer.
            // Look at the StopRecording function.
            GameObject.FindGameObjectWithTag("Bridge").SendMessage("RecordStop");
            StartCoroutine(RestartSpeechSystem());
            
        }
       
    }

    /// <summary>
    /// Turns on the dictation recognizer and begins recording audio from the default microphone.
    /// </summary>
    /// <returns>The audio clip recorded from the microphone.</returns>
    public AudioClip StartRecording()
    {
        Debug.Log("start Recording");
        // 3.a Shutdown the PhraseRecognitionSystem. This controls the KeywordRecognizers
        PhraseRecognitionSystem.Shutdown();

        // 3.a: Start dictationRecognizer
        dictationRecognizer.Start();

        // Set the flag that we've started recording.
        hasRecordingStarted = true;

        // Start recording from the microphone for 5 seconds.
        return Microphone.Start(deviceName, false, messageLength, samplingRate);
    }

    /// <summary>
    /// Ends the recording session.
    /// </summary>
    public void StopRecording()
    {
       

        // 3.a: Check if dictationRecognizer.Status is Running and stop it if so
        if (dictationRecognizer.Status == SpeechSystemStatus.Running)
        {
            dictationRecognizer.Stop();
            Debug.Log(dictationRecognizer.Status);

        }
        Microphone.End(deviceName);

        if (dictationRecognizer.Status == SpeechSystemStatus.Stopped)
        {
            PhraseRecognitionSystem.Restart();
            Debug.Log(PhraseRecognitionSystem.Status);
        }
        else
        {
            Debug.Log(dictationRecognizer.Status+" & "+ PhraseRecognitionSystem.Status);
        }



    }

    /// <summary>
    /// This event is fired after the user pauses, typically at the end of a sentence. The full recognized string is returned here.
    /// </summary>
    /// <param name="text">The text that was heard by the recognizer.</param>
    /// <param name="confidence">A representation of how confident (rejected, low, medium, high) the recognizer is of this recognition.</param>
    private void DictationRecognizer_DictationResult(string text, ConfidenceLevel confidence)
    {
        // 3.a: Append textSoFar with latest text
        textSoFar.Append(text);
        Debug.Log(textSoFar.ToString());

    }

    /// <summary>
    /// This event is fired when the recognizer stops, whether from Stop() being called, a timeout occurring, or some other error.
    /// Typically, this will simply return "Complete". In this case, we check to see if the recognizer timed out.
    /// </summary>
    /// <param name="cause">An enumerated reason for the session completing.</param>
    private void DictationRecognizer_DictationComplete(DictationCompletionCause cause)
    {
        // If Timeout occurs, the user has been silent for too long.
        // With dictation, the default timeout after a recognition is 20 seconds.
        // The default timeout with initial silence is 5 seconds.
        if (cause == DictationCompletionCause.TimeoutExceeded)
        {
            Microphone.End(deviceName);
            SendMessage("ResetAfterTimeout");
        }
    }

    /// <summary>
    /// This event is fired when an error occurs.
    /// </summary>
    /// <param name="error">The string representation of the error reason.</param>
    /// <param name="hresult">The int representation of the hresult.</param>
    private void DictationRecognizer_DictationError(string error, int hresult)
    {
        Debug.Log("Error");
    }


    private IEnumerator RestartSpeechSystem()
    {
        Debug.Log("Restarting speech system");
        while (dictationRecognizer != null && dictationRecognizer.Status == SpeechSystemStatus.Running)
        {
            yield return null;
        }
        PhraseRecognitionSystem.Restart();
        
    }

    public int StepNumber()
    {
        int temp;
        //StopRecording();

        if (Int32.TryParse(textSoFar.ToString(),out temp))
        {
            Debug.Log(temp);
            textSoFar.Length = 0;
            return temp;
        } else
        {
            Debug.Log("not an int");
            textSoFar.Length = 0;
            return -1;
        }
        
    }
}