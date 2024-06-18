using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GameCreator.Runtime.Console
{
    internal class ConsoleUI : MonoBehaviour
    {
        private const HideFlags HIDE_FLAGS = HideFlags.HideInHierarchy |
                                             HideFlags.HideInInspector;
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] private GameObject m_PrefabLineSubmit;
        [SerializeField] private GameObject m_PrefabLineResponse;
        [SerializeField] private GameObject m_PrefabLineError;
        
        [SerializeField] private GameObject m_Panel;

        [SerializeField] private InputField m_Input;
        [SerializeField] private ScrollRect m_Scroll;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public bool IsOpen => this.m_Panel.activeInHierarchy;
        
        // INITIALIZERS: --------------------------------------------------------------------------

        private void Awake()
        {
            this.gameObject.hideFlags = HIDE_FLAGS;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Submit()
        {
            if (!this.IsOpen) return;
            
            string text = this.m_Input.text;
            if (string.IsNullOrEmpty(text)) return;
            
            Input input = new Input(text);

            this.Print(input.ToString(), this.m_PrefabLineSubmit);
            this.m_Input.text = string.Empty;
            
            Console.Submit(input);
        }

        public void Print(string text)
        {
            this.Print(text, this.m_PrefabLineResponse);
        }

        public void Clear()
        {
            int children = this.m_Scroll.content.childCount;
            for (int i = children - 1; i >= 0; --i)
            {
                GameObject child = this.m_Scroll.content.GetChild(i).gameObject;
                Destroy(child);
            }
        }

        public void Close()
        {
            this.m_Panel.SetActive(false);
            this.m_Input.Select();
        }

        public void Open()
        {
            this.m_Panel.SetActive(true);
            this.m_Input.Select();
        }
        
        // INTERNAL METHODS: ----------------------------------------------------------------------

        internal void Print(Output output)
        {
            GameObject prefab = output.IsError
                ? this.m_PrefabLineError
                : this.m_PrefabLineResponse;
            
            this.Print(output.Text, prefab);
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void Print(string text, GameObject prefab)
        {
            if (string.IsNullOrEmpty(text)) return;

            GameObject result = Instantiate(prefab, this.m_Scroll.content);
            result.GetComponent<Text>().text = text;
            
            this.m_Input.Select();
            this.m_Input.ActivateInputField();

            StartCoroutine(this.ScrollBottom());
        }

        private IEnumerator ScrollBottom()
        {
            yield return new WaitForEndOfFrame();
            this.m_Scroll.verticalNormalizedPosition = 0f;
        }
    }
}