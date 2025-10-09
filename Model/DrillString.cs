using OSDC.DotnetLibraries.General.DataManagement;
using System;
using System.Collections.Generic;
using System.Collections;

namespace NORCE.Drilling.DrillString.Model
{
    public class DrillString : DrillStringLight
    {
        /// <summary>
        /// Section of the drill-string (drill-pipes, heavy weight drill pipes, BHA...)
        /// </summary>
        public List<DrillStringSection>? DrillStringSectionList { get; set; } = null;

        /// <summary>
        /// default constructor required for parsing the data model as a json file
        /// </summary>
        public DrillString() : base()
        {
        }
    }
}
