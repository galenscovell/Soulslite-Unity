using UnityEngine;
using UnityEngine.Assertions;


public class CameraController : MonoBehaviour {
    private new Camera camera;
    private float pixelsPerUnit = 1f;
    private float verticalUnitsOnScreen = 270f;

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

    void SetOrthographicSize(float unitsOnScreen) {
        var tempUnitSize = Screen.height / unitsOnScreen;
        finalUnitSize = GetNearestMultiple(tempUnitSize, pixelsPerUnit);
        camera.orthographicSize = Screen.height / (finalUnitSize * 2.0f);
    }

    float GetNearestMultiple(float value, float multiple) {
        float rem = value % multiple;
        float result = value - rem;

        if (rem > (multiple / 2))
            result += multiple;

        return result;
    }
}
