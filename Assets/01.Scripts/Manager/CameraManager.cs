using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    [SerializeField] private Camera mainCamera;

    [Header("카메라 이동")]
    [SerializeField] private float moveSpeed = 10f;    // 카메라 이동속도
    [SerializeField] private float edgeSize = 20f;     // 가장자리 감지

    [Header("맵 경계")]
    [SerializeField] private Vector2 mapMin = new Vector2(-25f, -25f);
    [SerializeField] private Vector2 mapMax = new Vector2(25f, 25f);

    [SerializeField] private Transform startPosition;

    private int focusIndex;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        if (mainCamera == null)
            mainCamera = Camera.main;
    }
    private void Start()
    {
        mainCamera.transform.position = new Vector3(startPosition.position.x, startPosition.position.y, mainCamera.transform.position.z);
    }
    // Update is called once per frame
    void Update()
    {
        HandleKeyMove();
        HandleEdgeMove();
        HandleSelection();
    }
    private void HandleKeyMove()
    {
        Vector2 input = Vector2.zero;

        if (Keyboard.current.upArrowKey.isPressed)
            input.y += 1f;
        if (Keyboard.current.downArrowKey.isPressed)
            input.y += -1f;
        if (Keyboard.current.leftArrowKey.isPressed)
            input.x += -1f;
        if (Keyboard.current.rightArrowKey.isPressed)
            input.x += 1f;

        if (input == Vector2.zero)
            return;

        MoveCamera(input.normalized * moveSpeed * Time.deltaTime);
    }
    private void HandleEdgeMove()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        if (mousePosition.x < 0 || mousePosition.x > Screen.width || mousePosition.y < 0 || mousePosition.y > Screen.height)
            return;

        Vector2 dir = Vector2.zero;

        if(mousePosition.x <= edgeSize)
            dir.x = -1f;
        else if(mousePosition.x >= Screen.width - edgeSize)
            dir.x = 1f;

        if(mousePosition.y <= edgeSize)
            dir.y = -1f;
        else if(mousePosition.y >= Screen.height - edgeSize)
            dir.y = 1f;

        if(dir == Vector2.zero)
            return;

        MoveCamera(dir.normalized * moveSpeed * Time.deltaTime);
    }
    private void HandleSelection()
    {
        if (!Keyboard.current.spaceKey.wasPressedThisFrame)
            return;

        IReadOnlyList<Unit> selectUnits = Player.instance.SelectUnitList;
        if(selectUnits != null && selectUnits.Count > 0)
        {
            if (focusIndex >= selectUnits.Count)
                focusIndex = 0;

            for(int i = 0; i < selectUnits.Count; i++)
            {
                Unit unit = selectUnits[focusIndex];
                focusIndex = (focusIndex + 1) % selectUnits.Count;  // 인덱스만큼 순환

                if(unit != null && unit.IsAlive)
                {
                    SetCameraPosition(unit.transform.position);
                    return;
                }
            }
            return;
        }

        Building selectBuilding = Player.instance.SelectBuilding;
        if (selectBuilding != null && selectBuilding.IsAlive)
            SetCameraPosition(selectBuilding.transform.position);

    }
    private void MoveCamera(Vector2 delta)
    {
        SetCameraPosition((Vector2)mainCamera.transform.position + delta);
    }
    private void SetCameraPosition(Vector2 position)
    {
        float cameraHeight = mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        float minX = mapMin.x + cameraWidth;
        float maxX = mapMax.x - cameraWidth;
        float minY = mapMin.y + cameraHeight;
        float maxY = mapMax.y - cameraHeight;

        // 맵이 카메라보다 작을 경우 중앙 고정
        float x = minX > maxX ? (mapMin.x + mapMax.x) * 0.5f : Mathf.Clamp(position.x, minX, maxX);
        float y = minY > maxY ? (mapMin.y + mapMax.y) * 0.5f : Mathf.Clamp(position.y, minY, maxY);

        mainCamera.transform.position = new Vector3(x, y, mainCamera.transform.position.z);
    }
    public void ResetFocusIndex()
    {
        focusIndex = 0;
    }
}
