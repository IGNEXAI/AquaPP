using System;
using System.Globalization;

namespace H2Optim.Services.WaterTreatment.PreliminaryTreatment;

using UnitsNet;


public class Sedimentation
{
    CultureInfo _usEnglish = new CultureInfo("en-US");
    
    private Length ParticleDiameter;
    private Density ParticleDensity;
    private Density WaterGra
    
    private double CalculateTerminalSettlingVelocity(
        double diameter, double viscosity)
    {
        // Placeholder code
        return 0.5 * diameter * viscosity;
        Console.WriteLine();
    }
}

public class