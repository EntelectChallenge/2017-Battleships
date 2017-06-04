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
		public string PathToJulia { get; set; }
        public string PathToGolang { get; set; }

        public string PathToNode { get; set; }
        public string PathToPython2 { get; set; }
        public string PathToPython3 { get; set; }
        public string PathToPythonPackageIndex { get; set; }
        public string PathToNpm { get; set; }
        public string PathToCargo { get; set; }

        public string CalibrationPathToCSharp { get; set; }
        public string CalibrationPathToGolang { get; set; }
        public string CalibrationPathToJava { get; set; }
        public string CalibrationPathToScala { get; set; }
        public string CalibrationPathToJavaScript { get; set; }
        public string CalibrationPathToJulia { get; set; }
        public string CalibrationPathToPython2 { get; set; }
        public string CalibrationPathToPython3 { get; set; }
        public string CalibrationPathToRust { get; set; }
        public string CalibrationPathToCPlusPlus { get; set; }
    }
}
