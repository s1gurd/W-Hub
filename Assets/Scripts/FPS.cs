// FPS Controller
// 1. Create a Parent Object like a 3D model
// 2. Make the Camera the user is going to use as a child and move it to the height you wish. 
// 3. Attach a Rigidbody to the parent
// 4. Drag the Camera into the m_Camera public variable slot in the inspector
// Escape Key: Escapes the mouse lock
// Mouse click after pressing escape will lock the mouse again


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using V3D.Controllers;

[RequireComponent(typeof(Rigidbody))]
public class FPS : MonoBehaviour
{

    private float speed = 5.0f;
    private float m_MovX;
    private float m_MovY;
    private Vector3 m_moveHorizontal;
    private Vector3 m_movVertical;
    private Vector3 m_velocity;
    private Rigidbody m_Rigid;
    private float m_yRot;
    private float m_xRot;
    private Vector3 m_rotation;
    private Vector3 m_cameraRotation;
    private float m_lookSensitivity = 3.0f;
   

    private Vector3 originalMousePos;
   

    [Header("The Camera the player looks through")]
    public Camera m_Camera;

    public float MouseReleaseSensMultiply = 0.2f;

    public float verticalLookThreshold = 75;

    // Use this for initialization
    private void Start()
    {
        m_Rigid = GetComponent<Rigidbody>();
        GameState.cursorLock = true;
        originalMousePos = new Vector3(Screen.width / 2f, Screen.height / 2f,0);
       
    }

    // Update is called once per frame
    public void Update()
    {
        

        m_MovX = Input.GetAxis("Horizontal");
        m_MovY = Input.GetAxis("Vertical");

        m_moveHorizontal = transform.right * m_MovX;
        m_movVertical = transform.forward * m_MovY;

        m_velocity = (m_moveHorizontal + m_movVertical).normalized * speed;

        
            //mouse movement 
            m_yRot = Input.GetAxis("Mouse X");
            m_rotation = new Vector3(0, m_yRot, 0) * m_lookSensitivity;

            m_xRot = Input.GetAxis("Mouse Y");
            m_cameraRotation = new Vector3(m_xRot, 0, 0) * m_lookSensitivity;
       

        

        if (!GameState.cursorLock)
        {
            m_rotation *= MouseReleaseSensMultiply;
            m_cameraRotation *= MouseReleaseSensMultiply;
        }

        //apply camera rotation

        //move the actual player here
        if (m_velocity != Vector3.zero)
        {
            m_Rigid.MovePosition(m_Rigid.position + m_velocity * Time.fixedDeltaTime);
        }

        if (m_rotation != Vector3.zero )
        {
            //rotate the camera of the player
            m_Rigid.MoveRotation(m_Rigid.rotation * Quaternion.Euler(m_rotation));
            
        }

        if (m_Camera )
        {
            //negate this value so it rotates like a FPS not like a plane
            Transform transform1;
            (transform1 = m_Camera.transform).Rotate(-m_cameraRotation);
            var e = transform1.localRotation.eulerAngles;
            
            if (e.x > verticalLookThreshold && e.x < 180) m_Camera.transform.localRotation = Quaternion.Euler(verticalLookThreshold,0, 0);
            if (e.x < 360 - verticalLookThreshold && e.x > 180) m_Camera.transform.localRotation = Quaternion.Euler(360 - verticalLookThreshold, 0, 0);
            
            
        }

        InternalLockUpdate();
        

    }

    //controls the locking and unlocking of the mouse
    private void InternalLockUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            GameState.cursorLock = false;
        }
        else if (Input.GetMouseButtonUp(0) && !GameState._currentPopup && !GameState._currentSlider)
        {
            GameState.cursorLock = true;
        }

        if (GameState.cursorLock)
        {
            UnlockCursor();
        }
        else if (!GameState.cursorLock)
        {
            LockCursor();
        }
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
    }

}