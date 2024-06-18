using System;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class WelcomeData
    {
        public WelcomePage[] pages;

        public WelcomeData()
        {
            this.pages = Array.Empty<WelcomePage>();
        }

        public WelcomeData(params WelcomePage[] pages) : this()
        {
            this.pages = new WelcomePage[pages.Length];
            for (int i = 0; i < pages.Length; ++i)
            {
                this.pages[i] = new WelcomePage(pages[i]);
            }
        }
        
        public WelcomeData(WelcomeData welcomeData) : this(welcomeData.pages)
        { }
    }
}