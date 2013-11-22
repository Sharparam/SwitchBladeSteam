using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Sharparam.SteamLib;
using Steam4NET;

namespace Sharparam.SwitchBladeSteam.Compatibility
{
    public class MessagesWrapper : ReadOnlyObservableCollection<MessageWrapper>
    {
        private readonly ObservableCollection<MessageWrapper> _wrapper;

        private bool _updating;

        public readonly ReadOnlyObservableCollection<Message> Messages;

        private MessagesWrapper(ReadOnlyObservableCollection<Message> messages, ObservableCollection<MessageWrapper> wrapper)
            : base(wrapper)
        {
            Messages = messages;
            _wrapper = wrapper;
            ((INotifyCollectionChanged) Messages).CollectionChanged += OnMessagesCollectionChanged;
            UpdateWrapper();
        }

        internal MessagesWrapper(ReadOnlyObservableCollection<Message> messages)
            : this(messages, new ObservableCollection<MessageWrapper>())
        {
            
        }

        private void OnMessagesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var newItem in from object newItem in e.NewItems
                                            let msg = (Message) newItem
                                            where msg.Type == EChatEntryType.k_EChatEntryTypeChatMsg
                                            select newItem)
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal,
                                                              (Action)
                                                              (() => _wrapper.Add(new MessageWrapper((Message) newItem))));
                    break;
                default:
                    UpdateWrapper();
                    break;
            }
        }

        private void UpdateWrapper()
        {
            if (_updating)
                return;

            _updating = true;

            _wrapper.Clear();

            foreach (var message in Messages.Where(m => m.Type == EChatEntryType.k_EChatEntryTypeChatMsg))
                _wrapper.Add(new MessageWrapper(message));

            _updating = false;
        }
    }
}
