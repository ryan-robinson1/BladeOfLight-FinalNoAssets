using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleToggleButton
{
    public class ToggleButtonClickedEventArgs : EventArgs
    {
        public bool IsOn { get; private set; }
        public ToggleButton Button { get; private set; }

        public ToggleButtonClickedEventArgs(bool isOn, ToggleButton button)
        {
            IsOn = isOn;
            Button = button;
        }
    }
}
