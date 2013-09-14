using System.Windows.Media;
using System.Windows.Media.Imaging;
using Sharparam.SteamLib;
using Sharparam.SwitchBladeSteam.Lib;
using Steam4NET;

namespace Sharparam.SwitchBladeSteam.Compatibility
{
    public class FriendWrapper
    {
        public Friend Friend { get; private set; }

        public string Name
        {
            get { return Friend.Name; }
        }

        public EPersonaState State
        {
            get { return Friend.State; }
        }

        public ImageSource SmallAvatar
        {
            get { return Helper.BitmapToSource(Friend.SmallAvatar); }
        }

        public ImageSource MediumAvatar
        {
            get { return Helper.BitmapToSource(Friend.MediumAvatar); }
        }

        public ImageSource LargeAvatar
        {
            get { return Helper.BitmapToSource(Friend.LargeAvatar); }
        }

        internal FriendWrapper(Friend friend)
        {
            Friend = friend;
        }
    }
}
