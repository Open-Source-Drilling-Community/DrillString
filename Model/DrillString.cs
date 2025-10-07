using OSDC.DotnetLibraries.General.DataManagement;
using System;
using System.Collections.Generic;
using System.Collections;

namespace NORCE.Drilling.DrillString.Model
{
    public class DrillString
    {
        /// <summary>
        /// a MetaInfo for the MyParentData
        /// </summary>
        public MetaInfo? MetaInfo { get; set; }

        /// <summary>
        /// name of the data
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// a description of the data
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// the date when the data was created
        /// </summary>
        public DateTimeOffset? CreationDate { get; set; }

        /// <summary>
        /// the date when the data was last modified
        /// </summary>
        public DateTimeOffset? LastModificationDate { get; set; }


        /// <summary>
        /// Carries the guid of the associated wellbore
        /// </summary>
        public Guid? WellBoreID { get; set; } = null;
        /// <summary>
        /// Section of the drill-string (drill-pipes, heavy weight drill pipes, BHA...)
        /// </summary>
        public List<DrillStringSection>? DrillStringSectionList { get; set; }

        /// <summary>
        /// default constructor required for parsing the data model as a json file
        /// </summary>
        public DrillString() : base()
        {
            this.DrillStringSectionList = new List<DrillStringSection>();
        }
    }
}
