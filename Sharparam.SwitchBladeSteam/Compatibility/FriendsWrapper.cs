using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Sharparam.SteamLib;

namespace Sharparam.SwitchBladeSteam.Compatibility
{
    public class FriendsWrapper : ReadOnlyObservableCollection<FriendWrapper>
    {
        private readonly ObservableCollection<FriendWrapper> _wrapper;

        private bool _updating;

        public readonly Friends Friends;

        private FriendsWrapper(Friends friends, ObservableCollection<FriendWrapper> wrapper) : base(wrapper)
        {
            Friends = friends;
            _wrapper = wrapper;
            ((INotifyCollectionChanged) friends).CollectionChanged += OnFriendsListChanged;
            UpdateWrapper();
        }

        public FriendsWrapper(Friends friends) : this(friends, new ObservableCollection<FriendWrapper>())
        {
            
        }

        private void OnFriendsListChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateWrapper();
        }

        private void UpdateWrapper()
        {
            if (_updating)
                return;

            _updating = true;

            _wrapper.Clear();

            foreach (var friend in Friends)
                _wrapper.Add(new FriendWrapper(friend));

            _updating = false;
        }
    }
}
