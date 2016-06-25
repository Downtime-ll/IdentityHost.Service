//using System;
//using System.Collections.Generic;

//namespace Nature.Core.DependencyInjection
//{
//    public static class ContainerRegistry
//    {
//        private static readonly IList<Weak<IShim>> Shims = new List<Weak<IShim>>();
//        private static readonly object SyncLock = new object();
//        private static IServiceProvider _serviceProvider;

//        public static void RegisterShim(IShim shim)
//        {
//            lock (SyncLock)
//            {
//                CleanupShims();

//                Shims.Add(new Weak<IShim>(shim));
//                shim.ServiceProvider = _serviceProvider;
//            }
//        }

//        public static void RegisterContainer(IServiceProvider serviceProvider)
//        {
//            lock (SyncLock)
//            {
//                CleanupShims();

//                _serviceProvider = serviceProvider;
//                RegisterContainerInShims();
//            }
//        }

//        private static void RegisterContainerInShims()
//        {
//            foreach (var shim in Shims)
//            {
//                var target = shim.Target;
//                if (target != null)
//                {
//                    target.ServiceProvider = _serviceProvider;
//                }
//            }
//        }

//        private static void CleanupShims()
//        {
//            for (int i = Shims.Count - 1; i >= 0; i--)
//            {
//                if (Shims[i].Target == null)
//                    Shims.RemoveAt(i);
//            }
//        }
//    }
//}