using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XDeploy.Changes;

namespace XDeploy.Data.Mapping
{
    class FileChecksumMap : ClassMapping<FileChecksum>
    {
        public FileChecksumMap()
        {
            Id(c => c.VirtualPath, m => m.Generator(Generators.Assigned));

            Property(c => c.Checksum, m => m.Column("`Checksum`"));
            Property(c => c.LastUpdatedAtUtc);
        }
    }
}
