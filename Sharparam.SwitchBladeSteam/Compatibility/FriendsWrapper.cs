using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Sharparam.SteamLib;

namespace Sharparam.SwitchBladeSteam.Compatibility
{
    public class FriendsWrapper : ReadOnlyObservableCollection<FriendWrapper>
    {
        private readonly ObservableCollection<FriendWrapper> _wrapper;

        private readonly Friends _friends;

        private bool _updating;

        private FriendsWrapper(Friends friends, ObservableCollection<FriendWrapper> wrapper) : base(wrapper)
        {
            _friends = friends;
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

            foreach (var friend in _friends)
                _wrapper.Add(new FriendWrapper(friend));

            _updating = false;
        }
    }
}
