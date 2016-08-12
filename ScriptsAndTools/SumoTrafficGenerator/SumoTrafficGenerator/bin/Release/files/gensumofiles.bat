@echo off

REM SUMO TRAFFIC GENERATOR BATCH
REM ----------------------------------------------
REM
REM This batch generates the files needed to run an instance on SUMO within the settings
REM given in the 6 following arguments (all of them mandatory):
REM
REM Argument 1: min latitude
REM Argument 2: min longitude
REM Argument 3: max latitude
REM Argument 4: max longitude
REM Argument 5: periodicity
REM Argument 6: output directory

REM - Download OSM data:

echo Running osmGet.py...

python "%SUMO_HOME%\tools\import\osm\osmGet.py" --bbox="%~1","%~2","%~3","%~4" --prefix=map --output-dir="%~6" 2>> "%~6"/map.log

echo osmGet.py completed

REM - Build the network:

echo Running osmBuild.py...

python "%SUMO_HOME%\tools\import\osm\osmBuild.py" --prefix=map --osm-file="%~6"/map_bbox.osm.xml --tiles=1 --vehicle-classes=road --output-directory="%~6" 2>> "%~6"/map.log

echo osmBuild.py completed

REM - Generate random trips:

echo Running randomTrips.py...

python "%SUMO_HOME%\tools\trip\randomTrips.py" --net-file="%~6"/map.net.xml --begin=0 --end=5000  --prefix=veh --period="%~5" --vclass=passenger --intermediate=300 --output-trip-file="%~6"/map.trips.xml 2>> "%~6"/map.log

echo randomTrips.py completed

REM - Generate the routes: 

echo Running duarouter...

duarouter --net-file="%~6"/map.net.xml --trip-files="%~6"/map.trips.xml --ignore-errors --repair --begin=0 --end=5000 --output-file="%~6"/map.rou.xml 2>> "%~6"/map.log

echo duarouter completed
