using Microsoft.CodeAnalysis.CSharp.Syntax;
using Semmle.Extraction.Kinds;

namespace Semmle.Extraction.CSharp.Entities.Expressions
{
    class Unknown : Expression
    {
        public Unknown(ExpressionNodeInfo info) : base(info.SetKind(ExprKind.UNKNOWN)) { }
    }
}
