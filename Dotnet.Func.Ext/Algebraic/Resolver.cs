namespace Dotnet.Func.Ext.Algebraic
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Reflection;
    using static Signatures;

    [AttributeUsage(AttributeTargets.Class)]
    public class ResolvableAttribute : Attribute { }

    public static class Resolver
    {

        static ConcurrentDictionary<Type, object> _dict = new ConcurrentDictionary<Type, object>();

        static Resolver()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.GetCustomAttributes<ResolvableAttribute>().Any())
                {
                    if (!_dict.TryAdd(type, Activator.CreateInstance(type)))
                        throw new InvalidOperationException($"Type {type.Name} has been already added");
                }
            }
        }

        public static structure Resolve<structure>()
        {
            var type = typeof(structure);

            object resolved;

            _dict.TryGetValue(type, out resolved);

            if (resolved != null)
                return (structure)resolved;

            return FallBack<structure>();
        }

        private static structure FallBack<structure>()
        {
            //if (typeof(structure).IsAssignableFrom(typeof(ANeutral)))
            //{
            //    var targs = typeof(structure).GenericTypeArguments;

            //    return Activator.CreateInstance()
            //}

            return default(structure); // TODO
        }

        [Resolvable]
        private class Neutral<T, M> : SNullOp<T, M>
        {
            public T NullOp() => default(T);
        }

        public class Resolvable<T>
            where T : class
        {
            internal T _ { get; set; }

            public Resolvable(T value)
            {
                _ = value;
            }
        }

        public static T Value<T>(this Resolvable<T> that)
            where T : class
        {
            if (that == null)
                return Resolve<T>();

            return that._ = that._ ?? Resolve<T>();
        }
    }
}
