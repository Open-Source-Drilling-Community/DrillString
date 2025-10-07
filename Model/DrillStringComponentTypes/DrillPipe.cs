using OSDC.DotnetLibraries.Drilling.DrillingProperties;
using OSDC.DotnetLibraries.General.DataManagement;
using System;
namespace Model.DrillStringComponentTypes
{
    /// <summary>
    /// a class deriving from base class
    /// </summary>
    public class DrillPipe
    {
        /// <summary>
        /// a parameter for the MyDerivedData defined as a Gaussian distribution 
        /// </summary>
        public ScalarDrillingProperty? DrillPipeParam { get; set; }
    }
}
