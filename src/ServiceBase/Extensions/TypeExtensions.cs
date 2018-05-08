namespace ServiceBase.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Offers extensions for types.
    /// </summary>
    public static class TypeExtensions
    {
        #region Public Static Methods

        /// <summary>
        /// Checks whether the passed type contains a public, parameterless
        /// constructor
        /// </summary>
        /// <param name="t">
        /// The type whose constructors should be checked.
        /// </param>
        /// <param name="bindingFlags">
        /// The binding flags which should be used for retrieving the
        /// constructor.
        /// </param>
        /// <returns>
        /// A flag indicating whether the current type has a parameterless
        /// constructor.
        /// </returns>
        public static bool HasParameterlessConstructor(
            this Type t,
            BindingFlags bindingFlags = BindingFlags.Default)
        {
            if (t == null)
            {
                throw new ArgumentNullException("t");
            }

            // So, msdn is all like: ctors are static. But actually, just
            // instance works Oo
            bindingFlags |= BindingFlags.Instance;

            return null != t.GetConstructor(
                bindingFlags,
                null,
                Type.EmptyTypes,
                new ParameterModifier[] { });
        }

        /// <summary>
        /// Checks whether the current type is a number type.
        /// </summary>
        /// <param name="t">
        /// The current type instance.
        /// </param>
        /// <returns>
        /// A flag indicating whether the current type is a numerical type.
        /// </returns>
        public static bool IsNumber(this Type t)
        {
            return t == typeof(UInt16) ||
                   t == typeof(UInt32) ||
                   t == typeof(UInt64) ||
                   t == typeof(Int16) ||
                   t == typeof(Int32) ||
                   t == typeof(Int64) ||
                   t == typeof(float) ||
                   t == typeof(decimal) ||
                   t == typeof(double);
        }

        /// <summary>
        /// Gets a enumerable over all inherited types.
        /// </summary>
        /// <param name="t">
        /// The current type for which all inherited types should be returned.
        /// </param>
        /// <param name="includeInterfaces">
        /// Optional, includes interfaces into the result - defaults to true.
        /// </param>
        /// <returns>
        /// All inherited types (and optional: interfaces)
        /// </returns>
        public static IEnumerable<Type> GetAllInheritedTypes(
            this Type t, bool includeInterfaces = true)
        {
            for (Type c = t; c != null; c = c.BaseType)
            {
                yield return c;
            }

            if (includeInterfaces)
            {
                foreach (Type @interface in t.GetInterfaces())
                {
                    yield return @interface;
                }
            }
        }

        /// <summary>
        /// Checks whether the current type is a tuple type.
        /// </summary>
        /// <remarks>
        /// Doesn't check if t just inherits from the <see cref="Tuple" />
        /// class.
        /// </remarks>
        /// <param name="t">
        /// The current type instance.
        /// </param>
        /// <returns>
        /// A flag indicating whether the current type is a tuple type.
        /// </returns>
        public static bool IsTuple(this Type t)
        {
            if (t == null)
            {
                throw new ArgumentNullException("t");
            }

            if (t == typeof(Tuple))
            {
                return true;
            }

            if (!t.IsGenericType)
            {
                return false;
            }

            Type typeDef = t.GetGenericTypeDefinition();

            return typeDef == typeof(Tuple<>) ||
                   typeDef == typeof(Tuple<,>) ||
                   typeDef == typeof(Tuple<,,>) ||
                   typeDef == typeof(Tuple<,,,>) ||
                   typeDef == typeof(Tuple<,,,,>) ||
                   typeDef == typeof(Tuple<,,,,,>) ||
                   typeDef == typeof(Tuple<,,,,,,>) ||
                   typeDef == typeof(Tuple<,,,,,,,>);
        }

        /// <summary>
        /// Gets a enumeration over all (loaded!) types implementing the
        /// passed type.
        /// </summary>
        /// <param name="t">
        /// The type the resulting types should implement.
        /// </param>
        /// <param name="ignoreInterfaces">
        /// Ignore types which are interfaces.
        /// </param>
        /// <param name="ignoreAbstract">
        /// Ignore types which are abstracts.
        /// </param>
        /// <param name="justAssemblyOfT">
        /// A flag indicating whether only types of the assembly of the
        /// current type should be checked.
        /// </param>
        /// <returns>
        /// All types implementing the passed type.
        /// </returns>
        public static IEnumerable<Type> GetAllInheritingTypes(
            this Type t,
            bool ignoreInterfaces = false,
            bool ignoreAbstract = false,
            bool justAssemblyOfT = false)
        {
            Assembly[] asms;

            if (justAssemblyOfT)
            {
                asms = new Assembly[] { t.Assembly };
            }
            else
            {
                asms = AppDomain.CurrentDomain.GetAssemblies();
            }

            Type[] tArgs = null;

            if (t.IsGenericType)
            {
                tArgs = t.GetGenericArguments();
            }

            foreach (Assembly asm in asms)
            {
                Type[] types;

                try
                {
                    types = asm.GetTypes();
                }
                catch (ReflectionTypeLoadException)
                {
                    continue;
                }

                foreach (Type iterator in types)
                {
                    Type type = iterator;

                    if ((ignoreInterfaces && type.IsInterface) ||
                        (ignoreAbstract && type.IsAbstract && !type.IsInterface))
                    {
                        continue;
                    }

                    if (t.IsGenericType &&
                        type.IsGenericTypeDefinition)
                    {
                        try
                        {
                            type = type.MakeGenericType(tArgs);
                        }
                        catch (Exception)
                        {
                        }
                    }

                    if (t.IsAssignableFrom(type) ||
                        (type.IsInterface && type.GetInterfaces().Contains(t)))
                    {
                        yield return type;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the default value for the passed type.
        /// </summary>
        /// <param name="t">
        /// The type for which the default value should be returned.
        /// </param>
        /// <returns>
        /// The default value for the passed type.
        /// </returns>
        public static object GetDefaultValue(this Type t)
        {
            if (t == null)
            {
                throw new ArgumentNullException("t");
            }

            Func<object> f = TypeExtensions.GetDefault<object>;

            return f.Method.GetGenericMethodDefinition().MakeGenericMethod(t)
                .Invoke(null, null);
        }

        /// <summary>
        /// Checks whether the current type implements the passed interface
        /// type.
        /// </summary>
        /// <typeparam name="TInterface">
        /// The interface type for which type should be checked.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        /// type or @interface is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// @interface isn't a interface.
        /// </exception>
        /// <param name="type">
        /// The type which should be checked for implementing the passed type.
        /// </param>
        /// <returns>
        /// A flag indicating whether the current type implements the passed
        /// interface.
        /// </returns>
        public static bool ImplementsInterface<TInterface>(this Type type)
        {
            return type.ImplementsInterface(typeof(TInterface));
        }

        /// <summary>
        /// Checks whether the current type implements the passed interface
        /// type.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// type or @interface is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// @interface isn't a interface.
        /// </exception>
        /// <param name="type">
        /// The type which should be checked for implementing the passed type.
        /// </param>
        /// <param name="interface">
        /// The interface type for which type should be checked.
        /// </param>
        /// <returns>
        /// A flag indicating whether the current type implements the passed
        /// interface.
        /// </returns>
        public static bool ImplementsInterface(this Type type, Type @interface)
        {
            if (!@interface.IsInterface)
            {
                throw new ArgumentException("!@interface is not an interface.");
            }

            return type.GetInterfaces().Contains(@interface);
        }

        /// <summary>
        /// Checks whether the current type instance implemented the passed
        /// open generic type.
        /// </summary>
        /// <param name="type">
        /// The type which should be checked for implementing the passed type.
        /// </param>
        /// <param name="openType">
        /// The open, generic type
        /// </param>
        /// <returns>
        /// A flag indicating whether the current type implements the passed
        /// open generic type.
        /// </returns>
        public static bool ImplementsOpenGenericType(
            this Type type, Type openType)
        {
            return type.TypeImplementingOpenGenericType(openType) != null;
        }

        /// <summary>
        /// Gets the closed generic type (e.g. IFoo&lt;Bar&gt;) of the
        /// currentType for the passed open generic type (e.g. IFoo&lt;&gt;).
        /// Checks the current type, all implemented interfaces and inherited
        /// base types.
        /// </summary>
        /// <param name="currentType">
        /// The type which should be checked for implementing the passed type.
        /// </param>
        /// <param name="openType">
        /// The open, generic type
        /// </param>
        /// &gt;
        /// <returns>
        /// Null or the closed type which is implemented in the current type
        /// for the passed open type.
        /// </returns>
        public static Type TypeImplementingOpenGenericType(
            this Type currentType, Type openType)
        {
            if (currentType == null)
            {
                throw new ArgumentNullException("currentType");
            }

            if (openType == null)
            {
                throw new ArgumentNullException("openType");
            }

            if (currentType.IsGenericType &&
                currentType.GetGenericTypeDefinition() == openType)
            {
                return currentType;
            }

            Type tmpType = currentType.GetInterfaces().FirstOrDefault(x =>
                    x.IsGenericType &&
                    x.GetGenericTypeDefinition() == openType);

            if (tmpType != null)
            {
                return tmpType;
            }

            for (Type t = currentType; t.BaseType != null; t = t.BaseType)
            {
                if (t.IsGenericType &&
                    t.GetGenericTypeDefinition() == openType)
                {
                    return t;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the attribute for the passed type.
        /// </summary>
        /// <param name="type">
        /// Type whose attribute should be returned.
        /// </param>
        /// <returns>
        /// Map attribute for the passed type.
        /// </returns>
        public static T GetAttribute<T>(this Type type)
        {
            object[] customAttrs = type.GetCustomAttributes(true);

            foreach (object attr in customAttrs)
            {
                if (typeof(T).IsAssignableFrom(attr.GetType()))
                {
                    return (T)attr;
                }
            }

            return default(T);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Returns the default value for the type T.
        /// </summary>
        /// <typeparam name="T">
        /// The type for which the default value should be returned.
        /// </typeparam>
        /// <returns>
        /// default(T)
        /// </returns>
        private static T GetDefault<T>()
        {
            return default(T);
        }

        #endregion
    }
}
