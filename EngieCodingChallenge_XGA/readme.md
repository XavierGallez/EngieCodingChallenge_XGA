# How to run the program

The code is provided as a standard Visual Studio 2019 solution.

## Testing of the API

To start and test the API, proceed as follows:

1. Set the *EngieCCWebApi* project as startup project and run its *EngieCCWebApi* profile. This will launch your web browser and open the swagger for the api.

1. Select the "/planproduction" post request. Click on "Try it out", copy the content of your json payload file in the *Request body* field.

1. Click on "Execute". The resulting json is available in the *Response body* field.

## Testing in a console (algorithm only)

If, for any reason the web server does not run (this is really my first time creating a web api & services from scratch) one may at least test the algorithm using the provided console application. To do so:

1. Set the *EngieCCTestProgram* project as startup project.

1. Add your json payload file to that project, in directory *example_payloads* (next to the 3 sample payload files). The build target for this file should be *None* (default).

1. Run the project.

1. The json result file should be created in directory *results*. A summary of the production plan as well as its cost is also displayed in the console for each payload file found in directory *example_payloads*.

# Main algorithm

The coding challenge documentation refers to the *merit order* approach that yields to a linearized Lagrangian-based optimization system of equations. The later could be solved using a standard LU-decomposition method, among others.

However I wanted to try something different, and solve the problem based on a more pragmatic approach based on common sense and intuition.

After all, our goal is to have a maximum of low-cost plans and a minimum of higher-cost plans running. So, I created an algorithm that does just that. To match a given load, the algorithm works as follows:

1. Switch on plants, one at a time, starting from the lowest-cost one to the highest-cost one, in ascending cost order until the required load is reached.

1. If, at any moment the *pmin* of the next plant to be switched on is higher than the remaining power to produce (*load - power of plants already running*), we switch on that plant with *p = pmin* and remove the extra power from the previous plant(s).

The three sample payloads tests seem to pass... although we should compare the results to those obtained from the complete (non-)linearized problem.

# Extras

## CO2 emissions

CO2 emissions are taken into account in the algorithm. However some assumptions were made as the coding challenge documentation raised some questions:

* I did not find any information on the CO2 emissions of the kerosine plants.

* It is not clear whether CO2 emissions are per thermal MWh (energy extracted from the fuel) or per electrical MWh (net electrical output: *MWh_elec = efficiency \* MHh_ther*)

Therefore, the following assumptions were made:

1. Kerosine plants do emit the same CO2 as gas plants. This seems a fair choice. Otherwise kerosine plants would be put to the same level as wind turbines.

1. The CO2 emissions are given in tons per *electrical MWh*. This is my understanding of "MWh *generated*".

These assumptions can be dismissed in code - look for **CO2_ASSUMPTION** (2 files).

## Websockets & docker

I decided to not implement these as I do not have any experience with these technologies and did not wish to spend more than 10 hours on this project.

# Legal rights

The code provided herein is the property of ArChiOm SPRL and subject to copyright and author rights. It is provided to Engie/GEM for the sole purpose of evaluation of a potential collaboration between ArChiOm SPRL and Engie. It cannot be used by Engie, any of its subsidiaries, or any other company, neither for internal use of any kind and at any time, neither for commercial purposes of any kind and at any time.
