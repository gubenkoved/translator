using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parser.Core;

namespace CodeGeneration
{
    public abstract class GeneratorBase
    {
        abstract public IntermediateCode GenerateIntermediateCode(SyntaxTree tree);
    }
}
