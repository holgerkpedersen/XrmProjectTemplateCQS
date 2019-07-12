﻿namespace Xrm.Models.Interfaces
{
    public interface IHandleEvent<TEvent> where TEvent : IEvent
    {
        void Handle(IEvent command);
    }
}
