using UnityEngine;
using UnityEngine.Assertions;


public class CameraController : MonoBehaviour {
    private new Camera camera;
    private int pixelsPerUnit = 1;
    private int verticalUnitsOnScreen = 270;

    private float finalUnitSize;
    private Vector3 playerOffset;

    public GameObject player;


    void Awake() {
        camera = gameObject.GetComponent<Camera>();
        Assert.IsNotNull(camera);

        SetOrthographicSize(verticalUnitsOnScreen);
    }

    void Start() {
        playerOffset = transform.position - player.transform.position;
    }

    void LateUpdate() {
        transform.position = player.transform.position + playerOffset;
    }

    void SetOrthographicSize(int unitsOnScreen) {
        var tempUnitSize = Screen.height / unitsOnScreen;
        finalUnitSize = GetNearestMultiple(tempUnitSize, pixelsPerUnit);
        camera.orthographicSize = Screen.height / (finalUnitSize * 2.0f);
    }

    int GetNearestMultiple(int value, int multiple) {
        int rem = value % multiple;
        int result = value - rem;

        if (rem > (multiple / 2))
            result += multiple;

        return result;
    }
}
