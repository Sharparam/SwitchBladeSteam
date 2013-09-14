using Sharparam.SwitchBladeSteam.Compatibility;

namespace Sharparam.SwitchBladeSteam.ViewModels
{
    public class MessagesViewModel
    {
        public MessagesWrapper Messages { get; private set; }

        public MessagesViewModel(MessagesWrapper wrapper)
        {
            Messages = wrapper;
        }
    }
}
