using System;
using System.Collections.Generic;
using System.Text;

namespace HuH.Communication.Common.Components
{

    [AttributeUsage(
AttributeTargets.Constructor | AttributeTargets.Method,
AllowMultiple = false, Inherited = true)]
    internal sealed class StringFormatMethodAttribute : Attribute
    {
        /// <param name="formatParameterName">
        /// Specifies which parameter of an annotated method should be treated as format-string
        /// </param>
        public StringFormatMethodAttribute(string formatParameterName)
        {
            FormatParameterName = formatParameterName;
        }

        public string FormatParameterName { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ComponentAttribute : Attribute
    {
        /// <summary>The lifetime of the component.
        /// </summary>
        public LifeStyle LifeStyle { get; private set; }
        /// <summary>Default constructor.
        /// </summary>
        public ComponentAttribute() : this(LifeStyle.Singleton) { }
        /// <summary>Parameterized constructor.
        /// </summary>
        /// <param name="lifeStyle"></param>
        public ComponentAttribute(LifeStyle lifeStyle)
        {
            LifeStyle = lifeStyle;
        }
    }



}
