using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class PlayerMovementManager : MonoBehaviour
{

		public Camera _camera;
		public float _clampMin = -10f;
		public float _clampMax = 15f;

		public Transform _gun;
		public Rigidbody _projectile;
		public float _fireRate = .5f;
		public float _projectileVelocity = 30f;
		public float _moveSpeed;
		public float _lookSpeed;

		private Transform _barrel;
		private bool _canFire = true;

		private PlayerControls _controls;
    private Vector2 _movementAxis;
    private Vector2 _lookAxis;
    private Vector2 _lookRot = Vector2.zero;

    private void OnEnable()
    {
    	_controls = new PlayerControls();
    	_barrel = _gun.Find("Barrel");
    	_controls.Player.Move.performed += HandleMovement;
    	_controls.Player.Move.Enable();
    	_controls.Player.Look.performed += HandleLook;
    	_controls.Player.Look.Enable();
    	_controls.Player.ActionButtons.performed += HandleActionButtons;
    	_controls.Player.ActionButtons.Enable();
    }

    private void OnDisable()
    {
    	_controls.Player.Move.performed -= HandleMovement;
    	_controls.Player.Move.Disable();
    	_controls.Player.Look.performed -= HandleLook;
    	_controls.Player.Look.Disable();
    	_controls.Player.ActionButtons.performed -= HandleActionButtons;
    	_controls.Player.ActionButtons.Disable();
    }

    private void Update()
    {
    	if (Mathf.Abs(_movementAxis.x) > .3 || Mathf.Abs(_movementAxis.y) > .3)
    	{
    		transform.position += _movementAxis.x * Time.deltaTime * _moveSpeed * Vector3.ProjectOnPlane(_camera.transform.right, Vector3.up);
    		transform.position += _movementAxis.y * Time.deltaTime * _moveSpeed * Vector3.ProjectOnPlane(_camera.transform.forward, Vector3.up);

    		// Necessary for kb as action only triggered on press
    		_movementAxis = Vector2.zero;
    	}
    }

    private void HandleMovement(InputAction.CallbackContext c)
    {
    	_movementAxis = c.ReadValue<Vector2>();
    }

    private void HandleLook(InputAction.CallbackContext c)
    {
    	_lookAxis = c.ReadValue<Vector2>();
    	_lookRot.x += - _lookAxis.y;
    	_lookRot.y += _lookAxis.x;
    	_lookRot.x = Mathf.Clamp(_lookRot.x, _clampMin, _clampMax);
    	transform.eulerAngles = new Vector2(0, _lookRot.y) * _lookSpeed;
    	_camera.transform.localRotation = Quaternion.Euler(_lookRot.x * _lookSpeed, 0, 0);

    	// Adjust angle of gun object
    	_gun.up = _camera.transform.forward;
    }

  	private void HandleActionButtons(InputAction.CallbackContext c)
  	{
  		string key = c.control.name;

  		// Lame switches...
    	switch(key)
    	{
    		case "space":
    			GetComponent<Rigidbody>().AddForce(transform.up * 100f);
    			break;
    		case "buttonSouth":
    			GetComponent<Rigidbody>().AddForce(transform.up * 100f);
    			break;
    		case "leftCtrl":
    			if (_canFire)
    			{
    				Rigidbody proj = Instantiate(_projectile, _barrel.position, _barrel.rotation);
    				proj.velocity = _barrel.TransformDirection(Vector3.forward * _projectileVelocity);
    				StartCoroutine(DisableShooting());
    			}
    			break;
    		default:
    			Debug.Log(key);
    			break;
    	}
  	}

  	private IEnumerator DisableShooting()
  	{
    	_canFire = false;
  		yield return new WaitForSeconds(_fireRate);
  		_canFire = true;
  	}
}
