using UnityEngine;

namespace UnityStandardAssets.Characters.FirstPerson
{


    public class MouseLook : MonoBehaviour
    {
        private const float defaultMinX = -360F;
        private const float defaultMaxX = 360F;
        private const float defaultMinY = -60F;
        private const float defaultMaxY = 60F;

        [SerializeField]
        private enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }

        [SerializeField]
        private RotationAxes axes = RotationAxes.MouseXAndY;

        [SerializeField]
        private float sensitivityX = 15F;

        [SerializeField]
        private float sensitivityY = 15F;

        [SerializeField]
        private float minimumX = defaultMinX;

        [SerializeField]
        private float maximumX = defaultMaxX;

        [SerializeField]
        private float minimumY = defaultMinY;

        [SerializeField]
        private float maximumY = defaultMaxY;

        private float rotationY = 0F;
        private bool m_cursorIsLocked = true;
        private bool isMovemementLocked = true;

        private void Update()
        {
            if (isMovemementLocked || Input.GetKey(KeyCode.F)) return;
            if (axes == RotationAxes.MouseXAndY )
            {
                float rotationX = transform.rotation.eulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
                transform.rotation = Quaternion.Euler(new Vector3(-rotationY, rotationX, 0));
            }
            else if (axes == RotationAxes.MouseX)
            {
                transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
            }
            else
            {
                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
                transform.rotation = Quaternion.Euler(new Vector3(-rotationY, transform.localEulerAngles.y, 0));
            }
            InternalLockUpdate();
        }

        private void Start()
        {
            // Make the rigid body not change rotation
            if (GetComponent<Rigidbody>())
                GetComponent<Rigidbody>().freezeRotation = true;
        }

        public void setMovementLocked(bool isLocked)
        {
            isMovemementLocked = isLocked;
        }

        public void setMouseLookLimits(float minX, float maxX, float minY, float maxY)
        {
            Vector3 currentAngle = transform.rotation.eulerAngles;
            minimumX = currentAngle.x - minX;
            maximumX = currentAngle.x + maxX;
            minimumY = currentAngle.y - minY;
            maximumY = currentAngle.y + maxY;
        }

        public void resetMouseLookLimits()
        {
            minimumX = defaultMinX;
            minimumY = defaultMinY;
            maximumX = defaultMaxX;
            maximumY = defaultMaxY;
        }

        private void InternalLockUpdate()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                m_cursorIsLocked = false;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                m_cursorIsLocked = true;
            }

            if (m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else if (!m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}
