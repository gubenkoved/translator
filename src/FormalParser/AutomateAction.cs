using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parser.Core;

namespace FormalParser
{
    public enum AutomateActionKind
    {
        /// <summary>
        /// Disclose left part of production to rigth symbols chain
        /// </summary>
        UseProduction,
        HandleError
    }

    public class AutomateAction
    {
        /// <summary>
        /// Production rule, error handler or another
        /// </summary>
        private object _data;

        public AutomateActionKind Kind { get; private set; }

        private AutomateAction(AutomateActionKind action, object data)
        {
            Kind = action;

            _data = data;
        }

        public static AutomateAction FromProduction(Production production)
        {
            return new AutomateAction(AutomateActionKind.UseProduction, production);
        }

        public Production GetProduction()
        {
            if (Kind == AutomateActionKind.UseProduction)
                return _data as Production;
            else
                throw new NotSupportedException("This action does not contains production");
        }

        public override string ToString()
        {
            return string.Format("{0} : {1}", Kind, _data);
        }
    }
}
