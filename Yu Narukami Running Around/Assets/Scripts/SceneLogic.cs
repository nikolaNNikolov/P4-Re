using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SceneLogic : MonoBehaviour
{
	private enum CurrentActiveCamera
	{
		Follow_Camera = 0, 
		Path_Camera = 1, 
		OTS_Camera = 2,
		MAX
	};

	[Header ("\tScene Cameras")]
	public GameObject followCamera;
	public GameObject pathCamera;
	public GameObject otsCamera;
	private Camera followCameraCamera;
	private Camera pathCameraCamera;
	private Camera otsCameraCamera;
	private Camera currentActivedCamera;
	[SerializeField]
	private CinemachineVirtualCamera dollyPathCamera;
	private CinemachineTrackedDolly dolly;
	[SerializeField]
	private CinemachineSmoothPath dollyPath;
	private Vector3[] dollyCameraNewPathPositions, dollyCameraOldPathPositions;
	private Vector3 newPathOffset, lastPathOffset;
	private float lastDollyPathPosition;
	private float currentFarClipPlaneFloat;
	[SerializeField]
	private float farClipPlaneFloat;
	private float farClipPlaneOffset = 3.5f, targetFOVDolly = 15.0f, 
					currentFOVDolly, timeToLerpDolly = 2.0f;
	private Coroutine loopSkyboxCoroutine;
	[SerializeField] 
	private Material dialogueSkybox;
	private Material currentSkybox;

	[Header ("\tScene Items")]
	public GameObject uiCanvas;
	public GameObject playerCharacter;
	public Light sceneLight;
	public Light[] scenePointLights;
	[Header ("\tMisc/Test Items")]
	[Range (0,4)]
	public int timeOfDay;
	public int prevTimeOfDay;
	public string calendarDate = "03/05";

	private string currentLocation, previousLocation;
	private Vector3 otsCameraOffset;
	private int flag = 1;
	private int currentCullingMask;
	private bool areLightsTurnedOn = false;
	private Color earlyMorningLightColor, dayLightColor, nightLightColor, dayAmbientLightColor, nightAmbientLightColor;

//*********************************************************************************************
//						SceneLogic General Methods
//*********************************************************************************************

    void Start()
	{
		earlyMorningLightColor = new Color(1.0f, 0.973f, 0.808f); 			//#F1CD88
		dayLightColor = new Color(1.0f, 0.953f, 0.655f); 					//#FFF3A7
		nightLightColor  = new Color(0.094f, 0.024f, 0.227f); 				//#18063A
		dayAmbientLightColor = new Color (0.180f, 0.129f, 0.102f); 		//#2E2117
		nightAmbientLightColor = new Color (0.114f, 0.114f, 0.196f, 255f); 	//#1D1D32

		Cursor.lockState = CursorLockMode.Locked; //Lock The Mouse
		Cursor.visible = false;
		
		otsCameraOffset = playerCharacter.transform.position - otsCamera.transform.position; 
		uiCanvas.GetComponent<UI_Canvas_Controller>().hideAll();
		this.dolly = this.dollyPathCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
		//this.dollyPath = this.dolly.m_Path;
		foreach(Light light in scenePointLights)
			light.gameObject.SetActive(false);

		dollyCameraNewPathPositions = new Vector3[3];
		dollyCameraOldPathPositions = new Vector3[3];

		followCameraCamera = followCamera.GetComponent<Transform>().Find("Follow Camera").GetComponent<Camera>();
		pathCameraCamera = pathCamera.GetComponent<Transform>().Find("Path Camera").GetComponent<Camera>();
		otsCameraCamera = otsCamera.GetComponent<Transform>().Find("OTS Camera").GetComponent<Camera>();

		timeOfDay = 2;
		switchCameraAndUI();
		updateTimeOfDay();
    }
		
    void Update()
    {
		if(Input.GetKeyDown("z"))
		{
			flag++;
			if(flag >= 3){flag = 0;}
			switchCameraAndUI();
		}

		if(prevTimeOfDay != timeOfDay)
		{
			updateTimeOfDay();
			prevTimeOfDay = timeOfDay;
		}

    }

	public void updateTimeOfDay()
	{
		UI_Canvas_Controller.TimeOfDay eNewTimeOfDay = (UI_Canvas_Controller.TimeOfDay)timeOfDay;
		uiCanvas.GetComponent<UI_Canvas_Controller>().updateTimeOfDay(eNewTimeOfDay);
		uiCanvas.GetComponent<UI_Canvas_Controller>().updateCalendarDate(calendarDate);
		updateLightning();
	}

	private void updateLightning()
	{
		if(areLightsTurnedOn)
		{
			areLightsTurnedOn = false;
			foreach(Light light in scenePointLights)
				light.gameObject.SetActive(false);
		}
		//NOTE: Light values work from in a 0-1 scale instead of 0-255
		//Early_Morning = 0 - XRot:  15  
		//Morning 		= 1 - XRot:  30  
		//After_School 	= 2 - XRot:  60 
		//Afternoon 	= 3 - XRot:  60  
		//Evening 		= 4 - XRot: -20 
		switch(timeOfDay)
		{
			case 0:
			RenderSettings.ambientLight = dayAmbientLightColor;
			sceneLight.gameObject.transform.eulerAngles = new Vector3(15,0,0);
			sceneLight.color = earlyMorningLightColor;
			break;
			case 1:
			RenderSettings.ambientLight = dayAmbientLightColor;
			sceneLight.gameObject.transform.eulerAngles = new Vector3(30,0,0);
			sceneLight.color = dayLightColor;
			break;
			case 2:
			RenderSettings.ambientLight = dayAmbientLightColor;
			sceneLight.gameObject.transform.eulerAngles = new Vector3(60,0,0);
			sceneLight.color = dayLightColor;
			break;
			case 3:
			RenderSettings.ambientLight = dayAmbientLightColor;
			sceneLight.gameObject.transform.eulerAngles = new Vector3(60,0,0);
			sceneLight.color = dayLightColor;
			break;
			case 4:
			areLightsTurnedOn = true;
			foreach(Light light in scenePointLights)
				light.gameObject.SetActive(true);
			
			RenderSettings.ambientLight = nightAmbientLightColor;
			sceneLight.gameObject.transform.eulerAngles = new Vector3(-20,0,0);
			sceneLight.color = nightAmbientLightColor;
			break;
			default:
			//newColor = new Color(255, 249, 206);
			//currentColor = newColor;
			//sceneLight.transform.rotation = Quaternion.Euler(15, sceneLight.transform.eulerAngles.y, sceneLight.transform.eulerAngles.z);
			//sceneLight.color = newColor;
			break;
		}
	}

//*********************************************************************************************
//						Camera and UI Related Methods
//*********************************************************************************************

	public void moveCameraToDialoguePosition(bool isInDaily, Transform lookAtObject)
	{
		if(isInDaily)
		{
			currentCullingMask = pathCameraCamera.cullingMask;
			currentFarClipPlaneFloat = pathCameraCamera.farClipPlane;
			currentSkybox = RenderSettings.skybox;
			currentFOVDolly = dollyPathCamera.m_Lens.FieldOfView;

			//show only MainCharacter & Interactable layers
			int newCullingMask = 	currentCullingMask &  (1 << LayerMask.NameToLayer("MainCharacter")); 
			newCullingMask = 		newCullingMask 		| (1 << LayerMask.NameToLayer("Interactable"));
			newCullingMask = 		newCullingMask 		| (1 << LayerMask.NameToLayer("CinemachineCameras"));
			pathCameraCamera.cullingMask = newCullingMask;

			RenderSettings.skybox = dialogueSkybox;
			RenderSettings.fog = true;
			loopSkyboxCoroutine = StartCoroutine(rotateSkybox(5.0f));

			lookAtObject.gameObject.GetComponent<Light>().enabled = true;			

			StartCoroutine(lerpValuesForCinemachineFOV(dollyPathCamera, targetFOVDolly, timeToLerpDolly));
			StartCoroutine(lerpValuesForCinemachineDutch(dollyPathCamera, 10, timeToLerpDolly));
			farClipPlaneFloat = Vector3.Distance(lookAtObject.position, dollyPathCamera.transform.position) + farClipPlaneOffset;
			dollyPathCamera.m_Lens.FarClipPlane = farClipPlaneFloat;

			dollyPathCamera.m_LookAt  = lookAtObject;
			dollyPathCamera.m_Follow = lookAtObject;
		}
	}

	public void resetCameraFromDialoguePosition(bool isInDaily, Transform lookAtObject)
	{
		if(isInDaily)
		{
			//add every layer back
			pathCameraCamera.cullingMask = currentCullingMask;

			RenderSettings.skybox = currentSkybox; 
			RenderSettings.fog = false;
			StopCoroutine(loopSkyboxCoroutine);
			
			lookAtObject.gameObject.GetComponent<Light>().enabled = false;

			StartCoroutine(lerpValuesForCinemachineFOV(dollyPathCamera, currentFOVDolly, timeToLerpDolly));
			StartCoroutine(lerpValuesForCinemachineDutch(dollyPathCamera, 0, timeToLerpDolly));
			dollyPathCamera.m_Lens.FarClipPlane = currentFarClipPlaneFloat;
			
			dollyPathCamera.m_LookAt  = playerCharacter.transform.Find("LookAtMeObject");
			dollyPathCamera.m_Follow = playerCharacter.transform;
		}
	}

	private IEnumerator lerpValuesForCinemachineFOV(CinemachineVirtualCamera camera, float targetFOV, float time)
	{
		float elapsedTime = 0;
		while (elapsedTime < time)
		{
			camera.m_Lens.FieldOfView = Mathf.Lerp(camera.m_Lens.FieldOfView, targetFOV, (elapsedTime / time));
			elapsedTime += Time.deltaTime;
			yield return null;
		}  
		//camera.m_Lens.FieldOfView = targetFOV;
 		yield return null;
	}

	private IEnumerator lerpValuesForCinemachineDutch(CinemachineVirtualCamera camera, float targetFOV, float time)
	{
		float elapsedTime = 0;
		while (elapsedTime < time)
		{
			camera.m_Lens.Dutch = Mathf.Lerp(camera.m_Lens.Dutch, targetFOV, (elapsedTime / time));
			elapsedTime += Time.deltaTime;
			yield return null;
		}  
		//camera.m_Lens.FieldOfView = targetFOV;
 		yield return null;
	}

	private IEnumerator rotateSkybox(float valueToRotate)
	{
		int rotationReset = 0;
		float rotationValue;
		while(true)
		{
			rotationValue = Time.time * valueToRotate - (rotationReset * 360.0f);
			if(rotationValue >= 360.0f)
				rotationReset ++;
				
			RenderSettings.skybox.SetFloat("_Rotation", rotationValue); 
			yield return new WaitForSeconds (0.01f);
		}
	}

	private void switchCameraAndUI()
	{
		CurrentActiveCamera checkCamera = (CurrentActiveCamera)flag;
		displayProperUI(checkCamera);
		hideAllCameras();
		switch(checkCamera)
		{
		case CurrentActiveCamera.Follow_Camera:
			showFollowCamera();
			currentActivedCamera = followCameraCamera;
			break;
		case CurrentActiveCamera.Path_Camera:
			uiCanvas.GetComponent<UI_Canvas_Controller>().activateUIDailyDialogueManager();
			showPathCamera();
			currentActivedCamera = pathCameraCamera;
			break;
		case CurrentActiveCamera.OTS_Camera:
			showOTSCamera();
			currentActivedCamera = otsCameraCamera;
			break;
		default:
			showOTSCamera();
			break;
		}
	}

	private void displayProperUI(CurrentActiveCamera activeCamera)
	{
		uiCanvas.GetComponent<UI_Canvas_Controller>().isChangedCamera = true;
		switch(activeCamera)
		{
		case CurrentActiveCamera.Follow_Camera:
			uiCanvas.GetComponent<UI_Canvas_Controller>().hideAll();
			uiCanvas.GetComponent<UI_Canvas_Controller>().showUIBattleElements();
			uiCanvas.GetComponent<UI_Canvas_Controller>().eActiveUI = UI_Canvas_Controller.AvaivableUI.UI_Battle;

			break;
		case CurrentActiveCamera.Path_Camera:
			uiCanvas.GetComponent<UI_Canvas_Controller>().hideAll();
			uiCanvas.GetComponent<UI_Canvas_Controller>().showUIDailyElements();
			uiCanvas.GetComponent<UI_Canvas_Controller>().eActiveUI = UI_Canvas_Controller.AvaivableUI.UI_Daily;
			break;
		case CurrentActiveCamera.OTS_Camera:
			uiCanvas.GetComponent<UI_Canvas_Controller>().hideAll();
			uiCanvas.GetComponent<UI_Canvas_Controller>().showUIDungeonElements();
			uiCanvas.GetComponent<UI_Canvas_Controller>().eActiveUI = UI_Canvas_Controller.AvaivableUI.UI_Dungeon;
			break;
		default:
			uiCanvas.GetComponent<UI_Canvas_Controller>().eActiveUI = UI_Canvas_Controller.AvaivableUI.MAX;
			break;
		}
	}

	public void updateLocationName(string locationName)
	{
		currentLocation = locationName;
		if(previousLocation != currentLocation)
		{
			uiCanvas.GetComponent<UI_Canvas_Controller>().updateLocationText(currentLocation);
			previousLocation = currentLocation;
		}
	}

	private void hideAllCameras()
	{
		hideFollowCamera();
		hidePathCamera();
		hideOTSCamera();
	}

	private void showFollowCamera()		{followCamera.SetActive(true); 	RenderSettings.fog = false;}
	private void showPathCamera()		{pathCamera.SetActive(true);	RenderSettings.fog = false;}
	private void showOTSCamera()		{otsCamera.SetActive(true);		RenderSettings.fog = true;}

	private void hideFollowCamera()		{followCamera.SetActive(false);	}
	private void hidePathCamera()		{pathCamera.SetActive(false);	}
	private void hideOTSCamera()		{otsCamera.SetActive(false);	}

	public Vector3 getOffset()			{	return otsCameraOffset;			}
	public Camera getActivatedCamera()	{	return currentActivedCamera;	}

	public UI_Canvas_Controller.AvaivableUI getActivateUI()	{	return uiCanvas.GetComponent<UI_Canvas_Controller>().eActiveUI;	}

}
