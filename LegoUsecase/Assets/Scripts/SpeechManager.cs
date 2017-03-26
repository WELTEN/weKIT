using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;


public class SpeechManager : MonoBehaviour
{
    KeywordRecognizer keywordRecognizer;
    Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();


    Camera cameraObject;
   
    // Use this for initialization
    void Start()
    {

        Debug.Log("Speech Recognition started");
        cameraObject = Camera.main;

            keywords.Add("Next", () =>
            {
                Debug.Log("keyword added");
                //GameObject.FindGameObjectWithTag("DisplayManual").SendMessage("AssignNext");
                try
                {
                    GameObject.FindGameObjectWithTag("Bridge").SendMessage("AssignNext");
                }
                catch
                {
                    return;
                }

            });

            keywords.Add("Previous", () =>
            {
                Debug.Log("keyword added");
                try
                {
                    GameObject.FindGameObjectWithTag("Bridge").SendMessage("AssignPrevious");
                }
                catch
                {
                    return;
                }

                //GameObject.FindGameObjectWithTag("DisplayManual").SendMessage("AssignPrevious");

            });

            keywords.Add("now", () =>
            {
                Debug.Log("keyword added");
                try
                {
                    GameObject.FindGameObjectWithTag("Bridge").SendMessage("ExpandModel");
                }
                catch
                {
                    return;
                }

            });

            keywords.Add("Step", () =>
            {
                Debug.Log("keyword added");
                try
                {
                    cameraObject.GetComponent<SpeechToTextManager>().SendMessage("StartRecording");
                }
                catch
                {
                    return;
                }

            });

            //keywords.Add("Snap", () =>
            //{
            //   Debug.Log("keyword added");
            //   cameraObject.GetComponent<CapturePhoto>().SendMessage("Capture");

            //});    

        // Tell the KeywordRecognizer about our keywords.
        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());

        // Register a callback for the KeywordRecognizer and start recognizing!
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();
        
    }

    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        System.Action keywordAction;
        if (keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }
}
