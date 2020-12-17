using System;
using System.Linq;

using SimpleContainer.Activators;
using SimpleContainer.Exceptions;
using SimpleContainer.Interfaces;

namespace SimpleContainer
{
    public sealed partial class Container
    {
        public TContract Resolve<TContract>()
        {
            var result = Resolve(typeof(TContract));

            try
            {
                return (TContract)result;
            }
            catch (InvalidCastException exception)
            {
                throw new InvalidCastException($"Cannot cast '{result?.GetType().Name}' to '{typeof(TContract).Name}'!", exception);
            }
        }

        public TContract[] ResolveAll<TContract>()
        {
            return ResolveAll(new CycleCounter(), typeof(TContract)).Cast<TContract>().ToArray();
        }

        public object Resolve(Type contractType)
        {
            return Resolve(new CycleCounter(), contractType, new object[0]);
        }

        public object Resolve(Type contractType, params object[] args)
        {
            var contractIsArray = contractType.IsArray;
            var elementType = contractIsArray ? contractType.GetElementType() : contractType;

            var instances = ResolveAll(new CycleCounter(), elementType, args);

            if (contractIsArray)
                return instances;

            return instances[0];
        }

        public TContract GetCached<TContract>()
        {
            var contractType = typeof(TContract);

            if (!bindings.TryGetValue(contractType, out var resolver))
                throw new TypeNotRegisteredException(contractType, GetBindingsString(bindings));

            return (TContract)resolver.GetCachedInstances().First()?.Value;
        }

        public TContract[] GetCachedMultiple<TContract>()
        {
            var contractType = typeof(TContract);

            if (!bindings.TryGetValue(contractType, out var resolver))
                throw new TypeNotRegisteredException(contractType, GetBindingsString(bindings));

            return resolver.GetCachedInstances().Select(wrapper => (TContract)wrapper.Value).ToArray();
        }

        public void InjectInto(object instance)
        {
            foreach (var binding in bindings.Values)
                binding.InjectIntoInstance(new CycleCounter(), instance);
        }

        internal object Resolve(CycleCounter cycleCounter, Type contractType)
        {
            return Resolve(cycleCounter, contractType, new object[0]);
        }

        internal object Resolve(CycleCounter cycleCounter, Type contractType, params object[] args)
        {
            var contractIsArray = contractType.IsArray;
            var elementType = contractIsArray ? contractType.GetElementType() : contractType;

            var instances = ResolveAll(cycleCounter, elementType, args);

            if (contractIsArray)
                return instances;

            return instances[0];
        }

        internal object[] ResolveAll(CycleCounter cycleCounter, Type contractType, params object[] args)
        {
            cycleCounter.LogType(contractType);

            if (!bindings.TryGetValue(contractType, out var resolver))
            {
                throw new TypeNotRegisteredException(contractType, $"Resolving graph:{Environment.NewLine}{cycleCounter.Pop()}{Environment.NewLine}{Environment.NewLine}" +
                                                                   $"Bindings:{Environment.NewLine}{GetBindingsString(bindings)}");
            }

            var instances = resolver.GetInstances(cycleCounter, args);

            InitializeInstances(instances);

            return instances.Select(instance => instance.Value).ToArray();
        }

        internal object[] ResolveMultiple(CycleCounter cycleCounter, Type contractType)
        {
            var contractIsArray = contractType.IsArray;
            var elementType = contractIsArray ? contractType.GetElementType() : contractType;
            var instances = ResolveAll(cycleCounter, elementType);

            return instances;
        }

        private IInstaller ResolveInstaller(Type installerType)
        {
            IActivator activator = new ActivatorReflection();
            var constructor = installerType.GetConstructor(new Type[0]);

            return (IInstaller)activator.CreateInstance(constructor, new object[0]);
        }
    }
}