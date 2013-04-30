using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy
{
    public static class ISessionExtensions
    {
        public static void Commit(this ISession session)
        {
            using (var tran = session.BeginTransaction())
            {
                tran.Commit();
            }
        }
    }
}
