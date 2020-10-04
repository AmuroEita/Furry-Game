using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace JTUtility.UI
{
    [System.Serializable]
    class SwitchButtonData
    {
        public string Label = string.Empty;
        public Color ButtonColor = Color.white;
        public Color LabelColor = Color.black;
        public StringEvent OnSwitchOn = new StringEvent();
    }

    class SwitchButton : MonoBehaviour
    {
        [System.Serializable]
        class IntEvent : UnityEvent<int> { }

        [SerializeField]
        SwitchButtonData[] data = new SwitchButtonData[0];

        [SerializeField]
        int CurrentIndex = 0;

        [SerializeField]
        IntEvent OnSwitch = null;

        [SerializeField]
        Image ButtonImage = null;

        [SerializeField]
        Text ButtonLabel = null;

        bool disabledEvent;

        private void Awake()
        {
            ButtonImage = GetComponentInChildren<Image>();
            ButtonLabel = GetComponentInChildren<Text>();
            disabledEvent = true;
            SetTo(CurrentIndex);
            disabledEvent = false;
        }

        public void Switch()
        {
            CurrentIndex++;
            if (CurrentIndex >= data.Length) CurrentIndex = 0;

            setButton(CurrentIndex);
        }

        public void SetTo(int index)
        {
            setButton(index);
        }

        void setButton(int index)
        {
            CurrentIndex = index;
            if (ButtonImage != null) ButtonImage.color = data[index].ButtonColor;
            if (ButtonLabel != null) ButtonLabel.color = data[index].LabelColor;
            if (ButtonLabel != null) ButtonLabel.text = data[index].Label;

            if (disabledEvent)
                return;

            if (data[index].OnSwitchOn != null)
                data[index].OnSwitchOn.Invoke(data[index].Label);

            OnSwitch.Invoke(index);
        }
    }
}
