namespace Backend.Manager.Helpers.Extension
{
    using System;
    using System.ComponentModel;

    public static class EnumsExtensions
    {
        /// <summary>
        /// Extension method for the defined enums to convert and get the {int} value
        /// of the current object.
        /// </summary>
        /// <typeparam name="T">{T:Enum:Int} The Defined Enumerator.</typeparam>
        /// <param name="self">{Defined Enumerator}.</param>
        /// <returns>{int} Value of the Enum object.</returns>
        public static int GetValue<T>(this T self)
            where T : struct
        {
            return Convert.ToInt32(self);
        }

        /// <summary>
        /// Extension method for the defined enusms o convert and get the {short} value.
        /// </summary>
        /// <typeparam name="T">{T:Enum:Int} The Defined Enumerator.</typeparam>
        /// <param name="self">{Int} The Int value to convert.</param>
        /// <returns>{Short} The converted value.</returns>
        public static short ToShort<T>(this T self)
            where T : struct
        {
            return Convert.ToInt16(self);
        }

        /// <summary>
        /// Extension method for the defined enums to get the {String} Description of the current enmum object.
        /// </summary>
        /// <typeparam name="T">{T:Enum} The Defined Enumerator.</typeparam>
        /// <param name="self">{T:Enum} The Defined Enumerator value.</param>
        /// <returns>{String} Enumerator Description.</returns>
        public static string GetDescription<T>(this T self)
            where T : struct
        {
            var fi = self.GetType().GetField(self.ToString());

            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
            {
                return attributes[0].Description;
            }

            return self.ToString();
        }

        /// <summary>
        /// Extension method for the defined enums to convert and get the {string} value
        /// of the current object.
        /// </summary>
        /// <typeparam name="T">{Enum} The Enum Value.</typeparam>
        /// <param name="self">{Defined Enumerator}.</param>
        /// <returns>{string} Value of the Enum object.</returns>
        public static string GetName<T>(this T self)
            where T : struct
        {
            return self.ToString();
        }

        /// <summary>
        /// Extension method to convert an int to the defined enumerator.
        /// </summary>
        /// <typeparam name="T">{Defined Enum}.</typeparam>
        /// <param name="self">{The int object}.</param>
        /// <returns>{Defined Enum} Enum.</returns>
        public static T ToEnum<T>(this int self)
        {
            var info = typeof(T);
            if (info.IsEnum)
            {
                T result = (T)Enum.Parse(typeof(T), self.ToString(), true);
                return result;
            }

            return default(T);
        }

        /// <summary>
        /// Extension used to check if the value exists is the specified Enumerator.
        /// </summary>
        /// <typeparam name="T">the source enumerator.</typeparam>
        /// <param name="self">The {int} value to check.</param>
        /// <returns>{True} if the value exists in the enumerator, {False} Otherwise.</returns>
        public static bool ValueInEnum<T>(this int self)
            where T : struct
        {
            return Enum.IsDefined(typeof(T), self);
        }
    }
}
