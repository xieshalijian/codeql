/**
 * @name Spring url redirection from remote source
 * @description Spring url redirection based on unvalidated user-input
 *              may cause redirection to malicious web sites.
 * @kind path-problem
 * @problem.severity error
 * @precision high
 * @id java/spring-unvalidated-url-redirection
 * @tags security
 *       external/cwe-601
 */

import java
import SpringUrlRedirect
import semmle.code.java.dataflow.FlowSources
import DataFlow::PathGraph

class SpringUrlRedirectFlowConfig extends TaintTracking::Configuration {
  SpringUrlRedirectFlowConfig() { this = "SpringUrlRedirectFlowConfig" }

  override predicate isSource(DataFlow::Node source) { source instanceof RemoteFlowSource }

  override predicate isSink(DataFlow::Node sink) { sink instanceof SpringUrlRedirectSink }

  override predicate isSanitizerGuard(DataFlow::BarrierGuard guard) {
    guard instanceof StartsWithSanitizer
  }

  override predicate isSanitizer(DataFlow::Node node) {
    // Exclude the case where the left side of the concatenated string is not `redirect:`.
    // E.g: `String url = "/path?token=" + request.getParameter("token");`
    exists(AddExpr ae |
      ae.getRightOperand() = node.asExpr() and
      not ae instanceof RedirectBuilderExpr
    )
  }
}

from DataFlow::PathNode source, DataFlow::PathNode sink, SpringUrlRedirectFlowConfig conf
where conf.hasFlowPath(source, sink)
select sink.getNode(), source, sink, "Potentially untrusted URL redirection due to $@.",
  source.getNode(), "user-provided value"
