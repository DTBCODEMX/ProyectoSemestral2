using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float followSpeed = 5f;
    public float deadZoneWidth = 2f;
    public float parallaxIntensity = 0.5f;
    public Vector2 margin = new Vector2(2f, 2f);
    public Vector2 maxOffset = new Vector2(5f, 5f);

    private Rigidbody2D rb;
    private Vector3 lastTargetPosition;
    private Vector3 currentVelocity;
    private Vector2 minCameraPos;
    private Vector2 maxCameraPos;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Obtener la mitad del tamaño de la cámara en unidades del mundo
        float halfHeight = Camera.main.orthographicSize;
        float halfWidth = halfHeight * Camera.main.aspect;

        // Calcular los límites de la cámara en unidades del mundo
        minCameraPos = new Vector2(halfWidth - margin.x, halfHeight - margin.y);
        maxCameraPos = new Vector2(LevelManager.instance.levelWidth - halfWidth + margin.x, LevelManager.instance.levelHeight - halfHeight + margin.y);
    }

    void FixedUpdate()
    {
        Vector3 targetPosition = target.position;
        Vector3 currentPosition = transform.position;

        // Comprobar si el objetivo está en la zona muerta
        if (Mathf.Abs(targetPosition.x - currentPosition.x) < deadZoneWidth)
        {
            targetPosition.x = currentPosition.x;
        }

        // Calcular la posición de la cámara sin paralaje
        Vector3 newPosition = Vector3.SmoothDamp(currentPosition, targetPosition, ref currentVelocity, 1 / followSpeed);

        // Calcular la posición de la cámara con paralaje
        float parallaxOffsetX = (newPosition.x - lastTargetPosition.x) * parallaxIntensity;
        float parallaxOffsetY = (newPosition.y - lastTargetPosition.y) * parallaxIntensity;
        Vector3 finalPosition = new Vector3(newPosition.x + parallaxOffsetX, newPosition.y + parallaxOffsetY, newPosition.z);

        // Limitar la posición de la cámara a los bordes del nivel
        finalPosition.x = Mathf.Clamp(finalPosition.x, minCameraPos.x, maxCameraPos.x);
        finalPosition.y = Mathf.Clamp(finalPosition.y, minCameraPos.y, maxCameraPos.y);

        // Aplicar la posición de la cámara
        transform.position = finalPosition;
        lastTargetPosition = targetPosition;

        // Aplicar la inercia de la cámara utilizando la física de Unity
        rb.MovePosition(finalPosition);

        // Limitar el movimiento de la cámara para que no se salga de los límites del nivel
        float x = Mathf.Clamp(rb.position.x, -maxOffset.x, maxOffset.x);
        float y = Mathf.Clamp(rb.position.y, -maxOffset.y, maxOffset.y);
        rb.MovePosition(new Vector2(x, y));
    }
}