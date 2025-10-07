using OSDC.DotnetLibraries.Drilling.DrillingProperties;
using OSDC.DotnetLibraries.General.DataManagement;
using System;
using System.Collections.Generic;

/* Every component carries base info. The eccentricity is expressed in polar cordinated.
I.e.: the distance from the mass center + an angle.
*/

namespace NORCE.Drilling.DrillString.Model
{
    public class DrillStringSection
    {
        public string? Name { get; set; }
        public int Count { get; set; }
        public List<DrillStringComponent> SectionComponentList { get; set; }     
        public DrillStringSection()
        {
            this.SectionComponentList = new List<DrillStringComponent>();
        }   
    }
}
