using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MARC.EHRS.Visualization.Core.Model;

namespace MARC.EHRS.Visualization.Core.Services
{
    /// <summary>
    /// Audit policy enforcement
    /// </summary>
    public interface IAuditPolicyEnforcement
    {

        /// <summary>
        /// Called when a service needs the policy enforcement service to apply policies against an audit
        /// </summary>
        AuditMessageInfo ApplyPolicies(AuditMessageInfo info);

    }
}
