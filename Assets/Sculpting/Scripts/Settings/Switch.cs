namespace VRSculpting.Settings
{

    public class Switch
    {

        public delegate void OnChangeHandler(bool value);

        public event OnChangeHandler OnChange;

        private bool value;
        public bool Value
        {
            get { return value; }
            set
            {
                this.value = value;
                OnChange?.Invoke(value);
            }
        }

        public void Toggle() { Value = !value; }

        public Switch(bool value = false)
        {
            this.value = value;
        }

    }

}

