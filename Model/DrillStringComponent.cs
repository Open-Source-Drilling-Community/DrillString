using OSDC.DotnetLibraries.Drilling.DrillingProperties;
using OSDC.DotnetLibraries.General.DataManagement;
using System;
using System.Collections.Generic;
using Model.DrillStringComponentTypes;
/* Every component carries base info. The eccentricity is expressed in polar cordinated.
I.e.: the distance from the mass center + an angle.
*/

namespace NORCE.Drilling.DrillString.Model
{
    public class DrillStringComponent
    {
        public MetaInfo? MetaInfo { get; set; }
        private string name;
        public string Name 
            {   get 
                {
                    if (name == "" || name == null){return "Component";} 
                    else {return name;}
                } 
                set {name = value;} 
            }
            
        /// <summary>
        /// the date when the data was created
        /// </summary>
        public DateTimeOffset? CreationDate { get; set; }
        /// <summary>
        /// the date when the data was last modified
        /// </summary>
        public DateTimeOffset? LastModificationDate { get; set; }
        public string? Description { get; set; }
        /// <summary>
        /// the Field which this component belongs to
        /// </summary>
        public Guid? FieldID { get; set; } = null;
        
        public DrillStringComponentTypes Type { get; set; }

        public List<DrillStringComponentPart> PartList { get; set; }
        private double calcComponentLength(List<DrillStringComponentPart> myPartList)
        {
            //Calculate total length form the list of mwd equipments
            double finalValue = 0.0;
            if (myPartList.Count > 0){
                foreach (DrillStringComponentPart part in myPartList)
                {
                    finalValue += part.TotalLength;
                }
            }            
            return finalValue;
        }
        public double Length {get {return calcComponentLength(PartList);}}        
        public DrillStringComponent() : base()
        {
            this.PartList = new List<DrillStringComponentPart>();
        }
    }

}
