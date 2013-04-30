using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace XDeploy
{
    public class ProjectLoader
    {
        public DeploymentProject LoadFrom(string path)
        {
            Require.NotNull(path, "path");

            using (var reader = new StreamReader(path, true))
            {
                var serializer = new XmlSerializer(
                    typeof(DeploymentProject),
                    IgnorantRuleFactory.AllIgnorantRuleTypes.ToArray());

                var project = (DeploymentProject)serializer.Deserialize(reader);
                project.Path = path;
                project.Name = Path.GetFileNameWithoutExtension(path);

                return project;
            }
        }
    }
}
