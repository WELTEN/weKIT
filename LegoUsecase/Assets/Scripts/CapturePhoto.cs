using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.WebCam;
using System.Linq;
using System;
using System.IO;
using UnityEngine.Networking;
using SimpleJSON;
using System.Text;

public enum CloudProvider
{
    GoogleCloudPlatform,
    MicrosoftCognitiveServices
}

public class CapturePhoto : MonoBehaviour {
    PhotoCapture photoCaptureObject = null;
    Resolution cameraResolution;

    public CloudProvider m_cloudProvider;
    public string m_apiKey;
    //public float m_pictureInterval;
    //public LayerMask m_raycastLayer;
    //public GameObject m_annotationParent;
    //public GameObject m_annotationTemplate;

    /// <summary>
    /// Entry Method for the class
    /// </summary>
    void Capture()
    {
        PhotoCapture.CreateAsync(true, OnPhotoCaptureCreated);
    }

    /// <summary>
    /// Creates a instance of locatable camera to open photo stream
    /// </summary>
    /// <param name="captureObject"></param>
    void OnPhotoCaptureCreated(PhotoCapture captureObject)
    {
        photoCaptureObject = captureObject;
        cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();

        CameraParameters c = new CameraParameters();
        c.hologramOpacity = 0.1f;
        c.cameraResolutionWidth = cameraResolution.width;
        c.cameraResolutionHeight = cameraResolution.height;
        c.pixelFormat = CapturePixelFormat.BGRA32;
        captureObject.StartPhotoModeAsync(c, OnPhotoModeStarted);
        
    }

    //Take a photo and save it to texture
    private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {
        Debug.Log("entered On Photo mode started");
        if (result.success)
        {
            Debug.Log("OnPhotoModeStarted = "+result.success);
            photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
            //photoCaptureObject.TakePhotoAsync(delegate (PhotoCapture.PhotoCaptureResult result1, PhotoCaptureFrame photoCaptureFrame)
            // {
            //     List<byte> buffer = new List<byte>();
            //     Matrix4x4 cameraToWorldMatrix;

            //     photoCaptureFrame.CopyRawImageDataIntoBuffer(buffer);

            //     //Check if we can receive the position where the photo was taken
            //     if (!photoCaptureFrame.TryGetCameraToWorldMatrix(out cameraToWorldMatrix))
            //     {
            //         return;
            //     }

            //     //Start a coroutine to handle the server request
            //     StartCoroutine(UploadAndHandlePhoto(buffer.ToArray(), cameraToWorldMatrix));

            // });
        }
        else
        {
            Debug.LogError("Unable to start photo mode!");
        }
    }

    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        if (result.success)
        {
            Debug.Log("OnCapturedPhotoToMemory = "+ result.success);
            List <byte> buffer = new List<byte>();
            // Create our Texture2D for use and set the correct resolution
            Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width/8 * res.height/8).First();
            Texture2D targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);
            // Copy the raw image data into our target texture
            photoCaptureFrame.UploadImageDataToTexture(targetTexture);
            Renderer renderer = GameObject.FindGameObjectWithTag("DisplayCube").GetComponent<Renderer>();
            renderer.material.mainTexture = targetTexture;
            Debug.Log("Photo Uploaded to Texture");

            Matrix4x4 cameraToWorldMatrix;

            photoCaptureFrame.CopyRawImageDataIntoBuffer(buffer);
            Debug.Log("Raw Image copied into buffer");
            //Check if we can receive the position where the photo was taken
            if (!photoCaptureFrame.TryGetCameraToWorldMatrix(out cameraToWorldMatrix))
            {
                return;
            }
            Debug.Log("past if");

            //Start a coroutine to handle the server request
            StartCoroutine(UploadAndHandlePhoto(buffer.ToArray(), cameraToWorldMatrix));

            Debug.Log("Photo saved to texture");
            
        }
        // Clean up
        photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }

    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        photoCaptureObject.Dispose();
        photoCaptureObject = null;
    }

    
    //private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    //{
    //    if (result.success)
    //    {
    //        string filename = string.Format(@"CapturedImage{0}_n.jpg", Time.time);
    //        string filePath = System.IO.Path.Combine(Application.persistentDataPath, filename);

    //        photoCaptureObject.TakePhotoAsync(filePath, PhotoCaptureFileOutputFormat.JPG, OnCapturedPhotoToDisk);
    //    }
    //    else
    //    {
    //        Debug.LogError("Unable to start photo mode!");
    //    }
    //}

    //void OnCapturedPhotoToDisk(PhotoCapture.PhotoCaptureResult result)
    //{
    //    if (result.success)
    //    {
    //        Debug.Log("Saved Photo to disk!");
    //        photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    //    }
    //    else
    //    {
    //        Debug.Log("Failed to save Photo to disk");
    //    }
    //}

    /// <summary>
    /// Start the upload and pass the response to the handling function
    /// </summary>
    IEnumerator UploadAndHandlePhoto(byte[] photo, Matrix4x4 cameraToWorldMatrix)
    {
        using (UnityWebRequest www = CreateRequest(photo))
        {
            //Send the request to the cloud
            yield return www.Send();

            //if (www.isError)
            //{
            //    Debug.Log(www.error);
            //}
            //else
            //{
            //    //Get JSON as string
            //    string jsonString = www.downloadHandler.text;

            //    //Remove all old annotations
            //    foreach (Transform child in m_annotationParent.transform)
            //    {
            //        Destroy(child.gameObject);
            //    }

            //    //Position the camera for raycasting
            //    Vector3 position = cameraToWorldMatrix.MultiplyPoint(Vector3.zero);
            //    Quaternion rotation = Quaternion.LookRotation(-cameraToWorldMatrix.GetColumn(2), cameraToWorldMatrix.GetColumn(1));
            //    Camera raycastCamera = this.gameObject.GetComponentInChildren<Camera>();
            //    raycastCamera.transform.position = position;
            //    raycastCamera.transform.rotation = rotation;

            //    JSONNode jsonResponse = JSON.Parse(jsonString);
            //    DrawFrames(raycastCamera, jsonResponse);
           // }
        }
    }

    /// <summary>
    /// Create a request for the current selected platform
    /// </summary>
    UnityWebRequest CreateRequest(byte[] photo)
    {
        DownloadHandler download = new DownloadHandlerBuffer();

        if (m_cloudProvider == CloudProvider.GoogleCloudPlatform)
        {
            string base64image = Convert.ToBase64String(photo);
            string json = "{\"requests\": [{\"image\": {\"content\": \"" + base64image + "\"},\"features\": [{\"type\": \"FACE_DETECTION\",\"maxResults\": 5}]}]}";
            byte[] content = Encoding.UTF8.GetBytes(json);
            UploadHandler upload = new UploadHandlerRaw(content);
            string url = "https://vision.googleapis.com/v1/images:annotate?key=" + m_apiKey;
            UnityWebRequest www = new UnityWebRequest(url, "POST", download, upload);
            www.SetRequestHeader("Content-Type", "application/json");
            return www;
        }
        else
        {
            //List<IMultipartFormSection> multipartFormSections = new List<IMultipartFormSection>();
            //multipartFormSections.Add(new MultipartFormFileSection("img", photo, "test.jpg", "image/jpeg"));
            //byte[] boundary = UnityWebRequest.GenerateBoundary();
            //UploadHandler upload = new UploadHandlerRaw(UnityWebRequest.SerializeFormSections(multipartFormSections, boundary));
            /*string url = "https://api.projectoxford.ai/vision/v1.0/analyze?visualFeatures=Faces";*/
            //string url = "https://api.projectoxford.ai/vision/v1.0/analyze?visualFeatures=Faces";
            //UnityWebRequest www = new UnityWebRequest(url, "POST", download, upload);
            //www.SetRequestHeader("Content-Type", "multipart/form-data; boundary=" + Encoding.UTF8.GetString(boundary));
            //www.SetRequestHeader("Ocp-Apim-Subscription-Key", m_apiKey);
            //return www;

            return null;
        }
    }


//    /// <summary>
//    /// Draw the annotations in the scene
//    /// </summary>
//    void DrawFrames(Camera raycastCamera, JSONNode jsonResponse)
//    {
//        JSONArray faces;
//        if (m_cloudProvider == CloudProvider.GoogleCloudPlatform)
//        {
//            faces = jsonResponse["responses"][0]["faceAnnotations"].AsArray;
//        }
//        else
//        {
//            faces = jsonResponse["faces"].AsArray;
//        }

//        foreach (JSONNode face in faces)
//        {
//            Vector3 topLeft = CalcTopLeftVector(face);
//            Vector3 topRight = CalcTopRightVector(face);
//            Vector3 bottomRight = CalcBottomRightVector(face);
//            Vector3 bottomLeft = CalcBottomLeftVector(face);
//            Vector3 raycastPoint = (topLeft + topRight + bottomRight + bottomLeft) / 4;
//            Ray ray = raycastCamera.ScreenPointToRay(raycastPoint);

//            RaycastHit centerHit;
//            if (Physics.Raycast(ray, out centerHit, 15.0f, m_raycastLayer))
//            {
//                Ray topLeftRay = raycastCamera.ScreenPointToRay(topLeft);
//                Ray topRightRay = raycastCamera.ScreenPointToRay(topRight);
//                Ray bottomLeftRay = raycastCamera.ScreenPointToRay(bottomLeft);

//                float distance = centerHit.distance;
//                float goScaleX = Vector3.Distance(topLeftRay.GetPoint(distance), topRightRay.GetPoint(distance));
//                float goScaleY = Vector3.Distance(topLeftRay.GetPoint(distance), bottomLeftRay.GetPoint(distance));

//                GameObject go = Instantiate(m_annotationTemplate) as GameObject;
//                go.transform.SetParent(m_annotationParent.transform);
//                go.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Vector3.up);
//                go.transform.position = centerHit.point;
//                go.transform.localScale = new Vector3(0.1f, goScaleX, goScaleY);
//            }
//        }
//    }

//    /// <summary>
//    /// Create the top left vector
//    /// </summary>
//    Vector3 CalcTopLeftVector(JSONNode node)
//    {
//        Vector3 vector;
//        if (m_cloudProvider == CloudProvider.GoogleCloudPlatform)
//        {
//            vector = new Vector3(node["boundingPoly"]["vertices"][0]["x"].AsFloat, node["boundingPoly"]["vertices"][0]["y"].AsFloat, 0);
//        }
//        else
//        {
//            JSONNode rect = node["faceRectangle"];
//            vector = new Vector3(rect["left"].AsFloat, rect["top"].AsFloat, 0);
//        }
//        return ScaleVector(vector);
//    }

//    /// <summary>
//    /// Create the top right vector
//    /// </summary>
//    Vector3 CalcTopRightVector(JSONNode node)
//    {
//        Vector3 vector;
//        if (m_cloudProvider == CloudProvider.GoogleCloudPlatform)
//        {
//            vector = new Vector3(node["boundingPoly"]["vertices"][1]["x"].AsFloat, node["boundingPoly"]["vertices"][1]["y"].AsFloat, 0);
//        }
//        else
//        {
//            JSONNode rect = node["faceRectangle"];
//            vector = new Vector3(rect["left"].AsFloat + rect["width"].AsFloat, rect["top"].AsFloat, 0);
//        }
//        return ScaleVector(vector);
//    }

//    /// <summary>
//    /// Create the bottom right vector
//    /// </summary>
//    Vector3 CalcBottomRightVector(JSONNode node)
//    {
//        Vector3 vector;
//        if (m_cloudProvider == CloudProvider.GoogleCloudPlatform)
//        {
//            vector = new Vector3(node["boundingPoly"]["vertices"][2]["x"].AsFloat, node["boundingPoly"]["vertices"][2]["y"].AsFloat, 0);
//        }
//        else
//        {
//            JSONNode rect = node["faceRectangle"];
//            vector = new Vector3(rect["left"].AsFloat + rect["width"].AsFloat, rect["top"].AsFloat + rect["height"].AsFloat, 0);
//        }
//        return ScaleVector(vector);
//    }

//    /// <summary>
//    /// Create the bottom left vector
//    /// </summary>
//    Vector3 CalcBottomLeftVector(JSONNode node)
//    {
//        Vector3 vector;
//        if (m_cloudProvider == CloudProvider.GoogleCloudPlatform)
//        {
//            vector = new Vector3(node["boundingPoly"]["vertices"][3]["x"].AsFloat, node["boundingPoly"]["vertices"][3]["y"].AsFloat, 0);
//        }
//        else
//        {
//            JSONNode rect = node["faceRectangle"];
//            vector = new Vector3(rect["left"].AsFloat, rect["top"].AsFloat + rect["height"].AsFloat, 0);
//        }
//        return ScaleVector(vector);
//    }

//    /// <summary>
//    /// Scale the vector
//    /// </summary>
//    Vector3 ScaleVector(Vector3 vector)
//    {
//        float scaleX = (float)Screen.width / (float)cameraResolution.width;
//        float scaleY = (float)Screen.height / (float)cameraResolution.height;

//        return vector * Math.Max(scaleX, scaleY);
//    }
}
