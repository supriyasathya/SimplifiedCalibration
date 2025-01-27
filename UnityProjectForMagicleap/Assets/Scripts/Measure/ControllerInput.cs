// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using UnityEngine;
using UnityEngine.XR.MagicLeap;
using System;

namespace MagicKit
{
    /// <summary>
    /// Wrapper around the controller and its calibration components.
    /// </summary>
    public class ControllerInput : MonoBehaviour
    {
        // ------ Public  Events ------

        public event Action OnTriggerDown;
        public event Action OnTriggerUp;
        public event Action OnBumperDown;
        public event Action OnBumperUp;
        public event Action OnTouchDown;
        public event Action OnTouchUp;

        // ------ Public  Members ------

        [HideInInspector] public float triggerValue;

        [HideInInspector] public Quaternion orientation;

        [HideInInspector] public Vector3 position;

        public Vector2 TouchPosition
        {
            get
            {
                return _controller.Touch1PosAndForce;
            }
        }

        public float TouchForce
        {
            get
            {
                return _controller.Touch1PosAndForce.z;
            }
        }

        public bool TouchActive
        {
            get
            {
                return _controller.Touch1Active;
            }
        }

        // ------ Private Members ------

        private MLInput.Controller _controller;
        private bool _triggerDown;
        private bool _bumperDown;
        private bool _touchDown;
        private const float TriggerThresh = 0.2f;

        // ------ MonoBehaviour Methods ------

        private void Start()
        {
            if (MLInput.Start().IsOk)
            {
                _controller = MLInput.GetController(MLInput.Hand.Left);
            }
        }

        private void Update()
        {
            UpdateTriggerState();
            UpdateTouchGesturesState();
            UpdateBumperState();
            Update3Dof();
            Update6DoF();
        }

        // ------ Private Methods ------

        private void UpdateTriggerState()
        {
            if (_controller == null)
            {
                return;
            }
            triggerValue = _controller.TriggerValue;
            if (_controller.TriggerValue > TriggerThresh)
            {
                // Trigger down
                if (!_triggerDown)
                {
                    _triggerDown = true;
                    var handler = OnTriggerDown;
                    if (handler != null)
                    {
                        handler();
                    }
                }
            }
            else
            {
                // Trigger up
                if (_triggerDown)
                {
                    _triggerDown = false;
                    var handler = OnTriggerUp;
                    if (handler != null)
                    {
                        handler();
                    }
                }
            }
        }

        private void UpdateTouchGesturesState()
        {
            if (_controller == null)
            {
                return;
            }
            if (_controller.Touch1Active)
            {
                if (!_touchDown)
                {
                    _touchDown = true;
                    var handler = OnTouchDown;
                    if (handler != null)
                    {
                        handler();
                    }
                }
            }
            else
            {
                if (_touchDown)
                {
                    _touchDown = false;
                    var handler = OnTouchUp;
                    if (handler != null)
                    {
                        handler();
                    }
                }
            }
        }

        private void UpdateBumperState()
        {
            if (_controller == null)
            {
                return;
            }
        //   if (_controller.State.ButtonState[(int)MLInputControllerButton.Bumper] != 0)
			if (_controller.TriggerValue > 0.2f && MLInput.Controller.Button.Bumper !=0)
			{
                if (!_bumperDown)
                {
                    _bumperDown = true;
                    var handler = OnBumperDown;
                    if (handler != null)
                    {
                        handler();
                    }
                }
            }
            else
            {
                if (_bumperDown)
                {
                    _bumperDown = false;
                    var handler = OnBumperUp;
                    if (handler != null)
                    {
                        handler();
                    }
                }
            }
        }

        private void Update3Dof()
        {
            orientation = transform.rotation;
        }

        private void Update6DoF()
        {
            position = transform.position;
        }
    }
}