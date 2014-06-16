namespace Sharparam.SwitchBladeSteam.Compatibility
{
    public class AppWrapper
    {
        public SteamLib.App App { get; private set; }

        public string Name { get { return App.Name; } }

        public bool Installed
        {
            get
            {
                return App.Installed;
            }
        }

        public bool Playable
        {
            get
            {
                return App.Playable;
            }
        }

        public bool IsGame
        {
            get
            {
                return App.IsGame;
            }
        }
    }
}
