import csharp
import Common

query predicate edges(
  SourceControlFlowNode node, SourceControlFlowNode successor, string attr, string val
) {
  not node.getElement().fromLibrary() and
  exists(ControlFlow::SuccessorType t | successor = node.getASuccessorByType(t) |
    attr = "semmle.label" and
    val = t.toString()
  )
}
