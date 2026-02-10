using OSDC.DotnetLibraries.Drilling.DrillingProperties;
using OSDC.DotnetLibraries.General.DataManagement;
using System;
using System.Collections.Generic;
using Model.DrillStringSensorTypes;
namespace NORCE.Drilling.DrillString.Model
{
    public class DrillStringSensor
    {    
        /// <summary>
        /// Sensor distance from bit
        /// </summary>
        public double? DistanceFromBit { get; set; }
         /// <summary>
        /// Range of the sensor measurement. 0 for in-point measurement
        /// </summary>
        public double? MeasurementRange { get; set; } = 0;
        public List<double>? SensorAngles { get; set; } = new();
        /// <summary>
        /// Type of the sensor integer which referes ti a bitwise enum of the sensor types. This is used for storage and processing purposes        
        /// </summary>
        public int SensorTypeInt { get; set; }
        /// <summary>
        /// Type of the sensor. It is kept here mostly for swagger purposes        
        /// </summary>
        public DrillStringSensorTypes SensorType { get; set; } 

    }
}
