using OSDC.DotnetLibraries.Drilling.DrillingProperties;
using OSDC.DotnetLibraries.General.DataManagement;
using System;

namespace NORCE.Drilling.DrillString.Model
{
    public class DrillStringComponentPart
    {    
        private const double PI = 3.1415926535897931;
        private double calculateCrossSectionArea(double Di, double Do)
        {
            return PI * 0.25 * (Do*Do - Di*Di);
        }    
        private double calculateCrossSectionTorsionalInertia(double Di, double Do)
        {
            return 0.5 * 0.25 * (Do*Do + Di*Di) * calculateCrossSectionArea(Di, Do);
        }    
        public Guid ID {get; set;}
        public string? Name {get; set;}
        // Geometric values    
        public double TotalLength {get; set;}    
        public double OuterDiameter {get; set;}
        public double InnerDiameter {get; set;}
        public double? OuterDiameterState2 {get; set;}
        public bool? OuterDiameterStateBoolean { get; set; }
        // Friction values
        public double? FrictionFactorRotation { get; set;}
        public double? FrictionFactorAxialDisplacement { get; set;}
        
        public double? PressureLossConstantLowFlowRate {get; set;}
        public double? PressureLossConstantHighFlowRate {get; set;}       
        // Imbalance information
        public double? EccentricityDistance { get; set; }
        public double? EccentricityAngle { get; set; }
        
        // Flow rate related properties
        public double? TotalFlowAreaCondition1 { get; set; }
        public double? TotalFlowAreaCondition2 { get; set; }
        public bool? TotalFlowAreaConditionBoolean { get; set; }
        public double? FlowrateThresholdValue { get; set; }

        public double? InnerCoatingThickness { get; set; }
        public double? InnerCoatingDensity { get; set; }
        public double? InnerCoatingThermalCondutivity { get; set; }
        public double? InnerCoatingHeatCapacity { get; set; }
        public double? OuterCoatingThickness { get; set; }
        public double? OuterCoatingDensity { get; set; }
        public double? OuterCoatingThermalCondutivity { get; set; }
        public double? OuterCoatingHeatCapacity { get; set; }
        
        public double? YieldStrength { get; set; }
        public double? UltimateStrength { get; set; }
        public double? SecondCrossSectionTorsionalInertia { get; set; }


        // Calulated properties (default to steel)
        private double inertia = 0.0;
        private double area = 0.0;
        private double youngModulus = 0.0;
        private double mass = 0.0;
        private double poissonRatio = 0.0;
        private double density = 0.0;
        private double heatCapacity = 0.0;
        private double thermalCondutivity = 0.0;
    
        public double FirstCrossSectionTorsionalInertia
        {
            get 
            {
                if (inertia == 0.0)
                {
                     return calculateCrossSectionTorsionalInertia(InnerDiameter, OuterDiameter);
                }
                else
                {
                    return inertia;
                };                
            } 
            set {inertia = value;}
        }
        public double CrossSectionArea 
        {
            get 
            {
                if (area == 0.0)
                {
                     return calculateCrossSectionArea(InnerDiameter, OuterDiameter);
                }
                else
                {
                    return area;
                };                
            } 
            set {area = value;}
        }
        public double YoungModulus
        {
            get 
            {
                if (youngModulus == 0.0)
                {
                     return 220E9;
                }
                else
                {
                    return youngModulus;
                };                
            } 
            set {youngModulus = value;}
        }
   
        public double PoissonRatio
        {
            get 
            {
                if (poissonRatio == 0.0)
                {
                     return 0.28;
                }
                else
                {
                    return poissonRatio;
                };                
            } 
            set {poissonRatio = value;}
        }
        public double MaterialDensity 
        {
            get 
            {
                if (density == 0.0)
                {
                     // If no mass is provided, default to steel
                     return 7800.0;
                }
                else
                {
                    return density;
                };                
            } 
            set {density = value;}
        }
        public double AveragePartDensity 
        {
            get 
            {
                if (density == 0.0)
                {
                     // If no mass is provided, default to steel
                     return 7800.0;
                }
                else
                {
                    return density;
                };                
            } 
            set {density = value;}
        }
        public double Mass 
        {
            get 
            {
                if (mass == 0.0)
                {
                     // If no mass is provided, default to steel
                     return AveragePartDensity * TotalLength * calculateCrossSectionArea(InnerDiameter, OuterDiameter);
                }
                else
                {
                    return mass;
                };                
            } 
            set {mass = value;}
        }     
        public double HeatCapacity
        {
            get
            {
                if (heatCapacity == 0.0)
                {
                    // If no mass is provided, default to steel
                    return 510.0;
                }
                else
                {
                    return heatCapacity;
                }
                ;
            }
            set { heatCapacity = value; }
        }
        public double ThermalCondutivity 
        {
            get 
            {
                if (thermalCondutivity == 0.0)
                {
                     // If no mass is provided, default to steel
                     return 54.0;
                }
                else
                {
                    return thermalCondutivity;
                };                
            } 
            set {thermalCondutivity = value;}
        }
        public DrillStringComponentPart() : base()
        {
        }
    }
}
