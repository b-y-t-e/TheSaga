using System;
using TheSaga.Interfaces;
using TheSaga.Models;

namespace TheSaga.Registrator
{
    public interface ISagaRegistrator
    {
        void Register(string sagaName, ISagaModel model);

        ISagaModel FindModel(IEvent @event);
    }
}