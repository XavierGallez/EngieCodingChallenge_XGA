using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngieCCComputer
{
    public class LoadComputer
    {

        #region Nested classes
        class PowerPlantDecorator
        {
            public PowerPlantDecorator(PowerPlant ppIn, double fuelEfficiency, double fuelCostPerMWh, double co2CostPerMWh)
            {
                _pp = ppIn;
                _fuelEfficiency = fuelEfficiency;
                _fuelCostPerMWh = fuelCostPerMWh;
                _co2CostPerMWh = co2CostPerMWh;
            }

            // Forwarding methods
            public string name => _pp.name;
            public string type => _pp.type;
            public double efficiency => _pp.efficiency;

            // Augmented methods
            public double pmin => _pp.pmin * _fuelEfficiency;
            public double pmax => _pp.pmax * _fuelEfficiency;

            // New methods/properties

            // CO2_ASSUMPTION
            // 1. Tons of CO2 given per electrical MHh
            public double costPerMWh => _fuelCostPerMWh / efficiency + _co2CostPerMWh;
            // 2. Tons of CO2 given per thermal MHh
            //public double costPerMWh => (_fuelCostPerMWh + _co2CostPerMWh) / efficiency;

            public double power { get; set; }
            public bool balanced { get; set; }
            public double cost => power * costPerMWh;
            public bool isNewCandidate => power == 0 && pmax > 0 && !balanced;

            // Private stuff
            readonly PowerPlant _pp;
            readonly double _fuelEfficiency;
            readonly double _fuelCostPerMWh;
            readonly double _co2CostPerMWh;
        }
        #endregion

        #region Properties

        public double availablePower => _pps.Sum(p => p.pmax);
        public double load => _payload.load;

        #endregion

        #region Constructors

        public LoadComputer(Payload payload)
        {
            _payload = payload;
            foreach (var pp in _payload.powerplants)
            {
                var fuelEfficiency = pp.type=="windturbine" ? _payload.fuels["wind(%)"] * 0.01 : 1;
                var fuelCostPerMWh = pp.type=="windturbine" ? 0 : pp.type=="gasfired" ? _payload.fuels["gas(euro/MWh)"] : _payload.fuels["kerosine(euro/MWh)"];
                // CO2_ASSUMPTION
                // 1. No CO2 emissions
                //var co2CostPerMWh = 0;
                // 2. Gas plants only
                //var co2CostPerMWh = pp.type == "gasfired" ? 0.3 * _payload.fuels["co2(euro/ton)"] : 0;
                // 3. Gas & kerosine plants
                var co2CostPerMWh = pp.type != "windturbine" ? 0.3 * _payload.fuels["co2(euro/ton)"] : 0;
                // Replace line above 
                _pps.Add(new PowerPlantDecorator(pp, fuelEfficiency, fuelCostPerMWh, co2CostPerMWh));
            }
        }

        #endregion

        #region Compute methods & properties

        public Tuple<List<LoadResponse>, double> Optimize()
        {
            // Let us first get rid of nasty cases
            if (_pps.Sum(p => p.pmax) < _payload.load)
                throw new Exception("Not enough power available");
            if (_pps.Min(p => p.pmin) > _payload.load)
                throw new Exception("Too much power, even from the smallest powerplant");

            // Sort by ascending cost, then ascending pmin. So, with linq we need to sort by cost a collection pre-sorted by pmin :-)
            _pps = _pps.OrderBy(p => p.pmin).OrderBy(p => p.costPerMWh).ToList();
            // Power yet-to-be produced
            var missingPower = load;
            while (missingPower != 0)
            {
                bool newPPAdded = false;
                while (!newPPAdded)
                {
                    // Find first PP that is not in use and whose minimum power is smaller than the missing power
                    PowerPlantDecorator candidate = _pps.Find(p => p.isNewCandidate);
                    if (candidate.pmin <= missingPower)
                    {
                        candidate.power = Math.Min(missingPower, candidate.pmax);
                        newPPAdded = true;
                    }
                    else
                    {
                        candidate.power = candidate.pmin;
                        newPPAdded = true;
                        // Try to balance
                        _pps.ForEach(p => p.balanced = false);
                        candidate.balanced = true;
                        var deltaPower = candidate.pmin - missingPower;
                        while (deltaPower != 0)
                        {
                            // Remove and balance last added PPs
                            try
                            { 
                                var lastPP = _pps.Last(p => (p.power > 0 && !p.balanced));
                                lastPP.power = Math.Max(lastPP.pmin, lastPP.power - deltaPower);
                                lastPP.balanced = true;
                                deltaPower -= (lastPP.pmax - lastPP.power);
                            }
                            catch (Exception e)
                            {
                                break;
                            }
                        }
                    }
                    missingPower = _payload.load - CurrentPower;
                }
            }
            // Adjust to 0.1 MW
            {
                _pps.ForEach(p => p.power = Math.Round(10 * p.power) * 0.1);
                var deltaPower = load - _pps.Sum(p => p.power);
                _pps.Last(p => p.power > 0).power += deltaPower;
            }
            return new Tuple<List<LoadResponse>, double>(
                _pps.Select(pp => new LoadResponse() { name = pp.name, p = pp.power }).ToList(),
                _pps.Sum(p => p.cost) );
        }

        #endregion

        #region Private stuff, helpers

        readonly Payload _payload;
        List<PowerPlantDecorator> _pps = new List<PowerPlantDecorator>();

        double CurrentPower => _pps.Sum(p => p.power);

        #endregion

    }
}
