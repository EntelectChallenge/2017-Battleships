using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotRunner.Util
{
    public class EnvironmentSettings
    {
        public string PathToMSBuild { get; set; }

        public string PathToXBuild { get; set; }

        public string PathToMaven { get; set; }

        public string PathToJava { get; set; }
        public string PathToGolang { get; set; }

        public string PathToNode { get; set; }
        public string PathToPython2 { get; set; }
        public string PathToPython3 { get; set; }
        public string PathToPythonPackageIndex { get; set; }
        public string PathToNpm { get; set; }
        public string PathToCargo { get; set; }
    }
}
