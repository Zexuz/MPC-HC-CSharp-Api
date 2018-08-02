using System;

namespace MPC_HC.Domain
{
    public delegate void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);

    public class PropertyChangedEventArgs : EventArgs
    {
        public Info     OldInfo  { get; }
        public Info     NewInfo  { get; }
        public Property Property { get; }

        public PropertyChangedEventArgs(Info oldInfo, Info newInfo, Property property)
        {
            OldInfo = oldInfo;
            NewInfo = newInfo;
            Property = property;
        }
    }
}