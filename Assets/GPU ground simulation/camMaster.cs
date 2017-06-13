using UnityEngine;
using System.Collections;

public class camMaster : MonoBehaviour {
	GameObject imageControl;
	bool moveImage;
	void Start(){
		// first of all, your videocard must support shader model 5.0 to work
		// run this if not sure:
		// Debug.Log(SystemInfo.supportsComputeShaders);

		calcs.init();
		imageControl = GameObject.Find("mainCanvas/image");
		moveImage = false;
	}
	void Update(){
		// calcs class is not MonoBehavior class, so it doesn have its own Update() method, so we run it from here
		calcs.update();
		controls();
	}
	void OnDestroy(){
		calcs.destroy();
	}
	void controls(){	// wheel - zoom; hold wheel + move mouse -> move simulation area; left mouse button - heat ground; right mouse button - explosion
		Vector2 imageSize;
		float kPixel;
		Vector3 centeredClickCoord, textureCoord;
		imageSize.x = imageControl.GetComponent<RectTransform>().rect.width;
		imageSize.y = imageControl.GetComponent<RectTransform>().rect.height;
		if (Input.mouseScrollDelta.y != 0) {
			imageSize.x += 0.1f * imageSize.x * Input.mouseScrollDelta.y;
			imageSize.y += 0.1f * imageSize.y * Input.mouseScrollDelta.y;
			imageControl.GetComponent<RectTransform>().sizeDelta = imageSize;
			imageControl.GetComponent<RectTransform>().anchoredPosition += 0.05f * Input.mouseScrollDelta.y * imageControl.GetComponent<RectTransform>().anchoredPosition / (1080 / imageSize.x);
		}
		if (Input.GetMouseButtonDown(2)) {		// holding wheel allows to move simulation area relatvely to the screen
			moveImage = true;
		}
		if (Input.GetMouseButtonUp(2)) {
			moveImage = false;
		}
		if (moveImage) {
			imageControl.GetComponent<RectTransform>().anchoredPosition += 16 * new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
		}
		if (Input.GetMouseButtonDown(0)) {
			kPixel = (imageControl.GetComponent<RectTransform>().sizeDelta.y / 1080.0f) * (Camera.main.ViewportToScreenPoint(new Vector3(1.0f, 1.0f, 0)).y / 1024.0f);
			centeredClickCoord = Input.mousePosition - Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0));
			centeredClickCoord = centeredClickCoord / kPixel;
			textureCoord = centeredClickCoord + new Vector3(512, 512, 0) - (Vector3)imageControl.GetComponent<RectTransform>().anchoredPosition / (imageControl.GetComponent<RectTransform>().sizeDelta.y / 1024.0f);

			// here we customize a kind of explosion, that actually just heats the ground, because it has explosion force = zero
			calcs.explContainer.addExpl(textureCoord.x, textureCoord.y, 72, 0);
		}
		if (Input.GetMouseButtonDown(1)) {
			kPixel = (imageControl.GetComponent<RectTransform>().sizeDelta.y / 1080.0f) * (Camera.main.ViewportToScreenPoint(new Vector3(1.0f, 1.0f, 0)).y / 1024.0f);
			centeredClickCoord = Input.mousePosition - Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0));
			centeredClickCoord = centeredClickCoord / kPixel;
			textureCoord = centeredClickCoord + new Vector3(512, 512, 0) - (Vector3)imageControl.GetComponent<RectTransform>().anchoredPosition / (imageControl.GetComponent<RectTransform>().sizeDelta.y / 1024.0f);

			// here we customize the explosion
			calcs.explContainer.addExpl(textureCoord.x, textureCoord.y, 80, 2.5f);
		}
	}
}
