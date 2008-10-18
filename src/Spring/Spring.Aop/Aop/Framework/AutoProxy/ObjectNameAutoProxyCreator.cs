#region License

/*
 * Copyright � 2002-2005 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion

#region Imports

using System;
using System.Collections;
using Spring.Objects.Factory;
using Spring.Util;

#endregion

namespace Spring.Aop.Framework.AutoProxy
{
    /// <summary>
    /// Object Auto Proxy Creator
    /// </summary>
    /// <remarks>
    /// <para>
    /// Auto proxy creator that identifies objects to proxy via a list of names.
    ///	Checks for direct, "xxx*", "*xxx" and "*xxx*" matches.
    /// </para>
    /// <para>In case of a IFactoryObject, only the objects created by the
    /// FactoryBean will get proxied.  If you intend to proxy a IFactoryObject instance itself
    /// specify the object name of the IFactoryObject including 
    /// the factory-object prefix "&amp;"  e.g. "&amp;MyFactoryObject".
    /// </para>
    /// </remarks>
    /// <seealso cref="Spring.Aop.Framework.AutoProxy.ObjectNameAutoProxyCreator.IsMatch"/>
    /// <author>Juergen Hoeller</author>
    /// <author>Adhari C Mahendra (.NET)</author>
    public class ObjectNameAutoProxyCreator : AbstractAutoProxyCreator
    {
        private IList objectNames;

        /// <summary>
        /// Set the names of the objects in IList fashioned way that should automatically 
        /// get wrapped with proxies.
        /// A name can specify a prefix to match by ending with "*", e.g. "myObject,tx*"
        /// will match the object named "myObject" and all objects whose name start with "tx".
        /// </summary>
        public IList ObjectNames
        {
            set { objectNames = value; }
            get { return objectNames; }
        }

        /// <summary>
        /// Determines, whether the given object shall be proxied.
        /// </summary>
        /// <returns>
        /// <see cref="AbstractAutoProxyCreator.PROXY_WITHOUT_ADDITIONAL_INTERCEPTORS"/> if the object shall be proxied.<br/>
        /// <see cref="AbstractAutoProxyCreator.DO_NOT_PROXY"/> otherwise.
        /// </returns>
        protected override object[] GetAdvicesAndAdvisorsForObject( Type objType, string name, ITargetSource customTargetSource )
        {
            if (ShallProxy( objType, name, customTargetSource ))
            {
                return PROXY_WITHOUT_ADDITIONAL_INTERCEPTORS;
            }
            return DO_NOT_PROXY;
        }

        /// <summary>
        /// Identify as object to proxy if the object name is in the configured list of names.
        /// </summary>
        protected virtual bool ShallProxy( Type objType, string name, ITargetSource customTargetSource )
        {
            if (objectNames != null)
            {
                for (int i = 0; i < objectNames.Count; i++)
                {
                    string mappedName = String.Copy( (string)objectNames[i] );
                    if (typeof( IFactoryObject ).IsAssignableFrom( objType ))
                    {
                        if (!name.StartsWith( ObjectFactoryUtils.FactoryObjectPrefix ))
                        {
                            continue;
                        }
                        mappedName = mappedName.Substring( ObjectFactoryUtils.FactoryObjectPrefix.Length );
                    }
                    if (IsMatch( name, mappedName ))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Return if the given object name matches the mapped name.
        /// </summary>
        /// <remarks>
        /// <p>
        /// The default implementation checks for "xxx*", "*xxx" and "*xxx*" matches,
        /// as well as direct equality. Can be overridden in subclasses.
        /// </p>
        /// </remarks>
        /// <param name="objectName">the object name to check</param>
        /// <param name="mappedName">the name in the configured list of names</param>
        /// <returns>if the names match</returns>
        protected virtual bool IsMatch( string objectName, string mappedName )
        {
            return PatternMatchUtils.SimpleMatch( mappedName, objectName );
        }
    }
}