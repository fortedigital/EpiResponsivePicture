using System;
using System.Collections.Generic;
using EPiServer.ServiceLocation;

namespace Forte.EpiResponsivePicture.Tests
{
    public class DummyServiceLocator : IServiceLocator
    {
        private readonly IReadOnlyDictionary<Type, object> services;

        public DummyServiceLocator(IReadOnlyDictionary<Type, object> services)
        {
            this.services = services;
        }

        public object GetService(Type serviceType)
        {
            throw new NotImplementedException();
        }

        public object GetInstance(Type serviceType)
        {
            return this.services[serviceType];
        }

        public TService GetInstance<TService>()
        {
            return (TService) this.GetInstance(typeof(TService));
        }

        public bool TryGetExistingInstance(Type serviceType, out object instance)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            throw new NotImplementedException();
        }
    }
}