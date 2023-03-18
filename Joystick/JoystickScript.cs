using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JoystickScript : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
	RectTransform rectTransform;
	RectTransform background;
	RectTransform handle;
	Vector2 originalPosition;
	Image handleImage;
	Color originalColor;

	enum JoystickModeEnum { Fixed, Dynamic, Following };
	[Tooltip("Fixed: It doesn't move. " +
		"\nDynamic: Every time the joystick area is pressed, the joystick position is set on the touched position. " +
		"\nFollowing: If the finger moves outside the joystick background, the joystick follows it.")]
	JoystickModeEnum joystickMode = JoystickModeEnum.Fixed;

	enum VectorModeEnum { Real, Normal };
	[Tooltip("Real: the output is a vector with magnitude beetween 0 and 1." +
		"\nNormal: the output is normalized.")]
	[SerializeField] VectorModeEnum vectorMode = VectorModeEnum.Real;

	[Tooltip("Color of the Handle when it's pressed.")]
	[SerializeField] Color pressedColor = new Color(0.5f, 0.5f, 0.5f, 0.7f);

	[Tooltip("If the handle is inside this range, in proportion to the background size, the output is zero.")]
	[Range(0f, 0.5f)]
	[SerializeField] float deadZone = 0.2f;

	[Tooltip("The max distance of the handle, in proportion to the background size.")]
	[Range(0.5f, 2f)]
	[SerializeField] float clampZone = 1f;

	[Header("directional joystick")]

	[Tooltip("Number of directions of the joystick. \nKeep at 0 for a free joystick.")]
	[Range(0, 12)]
	[SerializeField] int directions = 0;

	[Tooltip("Angle, in degrees, of the simmetry of the directions.")]
	[Range(-180f, 180f)]
	[SerializeField] float simmetryAngle = 90f;

	public bool IsWorking { get; private set; } = false;
	public Vector2 Output { get; private set; } = Vector2.zero;

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		Transform backgroundTransform = transform.GetChild(0);
		background = backgroundTransform.GetComponent<RectTransform>();
		handle = backgroundTransform.GetChild(0).GetComponent<RectTransform>();
		handleImage = handle.GetComponent<Image>();
	}

	private void Start()
	{
		IsWorking = false;
		Output = Vector2.zero;
		background.anchoredPosition = Vector2.zero;
		handle.anchoredPosition = Vector2.zero;
		originalPosition = background.anchoredPosition;
		originalColor = handleImage.color;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (joystickMode != JoystickModeEnum.Fixed)
		{
			background.position = eventData.position;
			handle.anchoredPosition = Vector2.zero;
		}
	}

	public void OnBackgroundPointerDown(BaseEventData baseEventData)
	{
		OnPointerDown(baseEventData as PointerEventData);
		handleImage.color = pressedColor;
	}

	public void OnBackgroundDrag(BaseEventData baseEventData)
	{
		PointerEventData eventData = baseEventData as PointerEventData;

		Vector2 vector = eventData.position - (Vector2) background.position;

		float magnitude = vector.magnitude;

		float deadSize = deadZone * background.sizeDelta.x * 0.5f;

		if (magnitude < deadSize)
		{
			Output = Vector2.zero;
			handle.anchoredPosition = Vector2.zero;
			IsWorking = false;
			return;
		}

		if (joystickMode == JoystickModeEnum.Following)
			Following(vector);

		if (directions > 0)
			vector = DirectionalVector(vector, directions, simmetryAngle * Mathf.Deg2Rad);

		float clampSize = clampZone * background.sizeDelta.x * 0.5f;

		Output = vector.normalized;
		if (vectorMode == VectorModeEnum.Real && magnitude < clampSize)
			Output *= (magnitude - deadSize) / (clampSize - deadSize);

		handle.anchoredPosition = Output * clampSize;

		IsWorking = true;
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (IsWorking || RectTransformUtility.RectangleContainsScreenPoint(background, eventData.position, eventData.pressEventCamera))
			OnBackgroundDrag(eventData);
	}

	public void OnBackgroundPointerUp(BaseEventData baseEventData)
	{
		IsWorking = false;
		Output = Vector2.zero;
		handle.anchoredPosition = Vector2.zero;
		if (joystickMode != JoystickModeEnum.Fixed)
			background.anchoredPosition = originalPosition;
		handleImage.color = originalColor;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		OnBackgroundPointerUp(eventData);
	}

	private Vector2 DirectionalVector(Vector2 vector, int directions, float simmetryAngle)
	{
		Vector2 simmetry = new Vector2(Mathf.Cos(simmetryAngle), Mathf.Sin(simmetryAngle));
		float angle = Vector2.SignedAngle(simmetry, vector);

		angle /= 180f / directions;

		angle = (angle >= 0f) ? Mathf.Floor(angle) : Mathf.Ceil(angle);
		if ((int) Mathf.Abs(angle) % 2 == 1)
			angle += (angle >= 0f) ? 1 : -1;

		angle *= 180f / directions;

		angle *= Mathf.Deg2Rad;
		Vector2 result = new Vector2(Mathf.Cos(angle + simmetryAngle), Mathf.Sin(angle + simmetryAngle));
		result *= vector.magnitude;
		return result;
	}

	private void Following(Vector2 vector)
	{
		float clampSize = clampZone * background.sizeDelta.x * 0.5f;
		if (vector.magnitude > clampSize)
		{
			Vector2 radius = vector.normalized * clampSize;
			Vector2 delta = vector - radius;
			Vector2 newPos = background.anchoredPosition + delta;

			float xMax = rectTransform.sizeDelta.x * 0.5f;
			newPos.x = Mathf.Clamp(newPos.x, -xMax, xMax);

			float yMax = rectTransform.sizeDelta.y * 0.5f;
			newPos.y = Mathf.Clamp(newPos.y, -yMax, yMax);

			background.anchoredPosition = newPos;
		}
	}
}
