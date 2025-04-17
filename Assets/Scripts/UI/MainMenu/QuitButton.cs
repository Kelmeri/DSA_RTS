using UnityEngine;
using UnityEngine.UI;

namespace RTS.Runtime
{
    [RequireComponent(typeof(Button))]
    public class QuitButton : MonoBehaviour
    {
        private Button _quitButton;
        
        private void Start()
        {
            _quitButton = GetComponent<Button>();
            UnityEngine.Assertions.Assert.IsNotNull(_quitButton, "QuitButton component is missing a Button component.");
            _quitButton.onClick.AddListener(() => Application.Quit());
        }
    }
}
