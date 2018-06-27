using System.IO;
using System.Runtime.Serialization;
using System.Text;

using Newtonsoft.Json;

namespace CSharpSampleApp.Util
{

    /// <summary>
    /// Serialization helper class.
    /// </summary>
    public class Serialization
    {
        #region Public Static Methods

        /// <summary>
        /// Serizalize object to XML string.
        /// </summary>
        /// <typeparam name="T">The type of serialization object.</typeparam>
        /// <param name="entity">The serialization object.</param>
        /// <returns></returns>
        public static string XmlSerizalize<T>(T entity) where T : class
        {
            if (entity != null)
            {
                using (var ms = new MemoryStream())
                {
                    new DataContractSerializer(typeof (T)).WriteObject(ms, entity);
                    ms.Position = 0;
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }

            return null;
        }

        public static string JSONSerizalize<T>(T entity) where T : class
        {
            if (entity != null)
            {
                return JsonConvert.SerializeObject(entity);
            }

            return null;
        }

        /// <summary>
        /// Deserialize the input XML string into the object.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="serialized">The XML input string.</param>
        /// <returns></returns>
        public static T Deserizalize<T>(string serialized) where T : class
        {
            if (!string.IsNullOrWhiteSpace(serialized))
            {
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(serialized)))
                {
                    return (T) new DataContractSerializer(typeof (T)).ReadObject(ms);
                }
            }

            return default(T);
        }

        #endregion Public Static Methods
    }
}