using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sofiapi.net.Models
{
    public class ProcedureParameter
    {
        public string ROUTINE_NAME { get; set; }

        public int ORDINAL_POSITION { get; set; }

        public string PARAMETER_MODE { get; set; }

        public string PARAMETER_NAME { get; set; }

        public string DATA_TYPE { get; set; }

        public string VALUE { get; set; }
    }
}