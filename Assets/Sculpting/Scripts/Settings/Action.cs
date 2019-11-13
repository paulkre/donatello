﻿namespace VRSculpting.Settings
{

    public class Action
    {

        public delegate void OnDoneHandler();

        public event OnDoneHandler OnDone;

        public void Do() { OnDone?.Invoke(); }

    }

}

